namespace Polylabel.NET
{
    using System;
    using System.Numerics;
    using System.Runtime.CompilerServices;

    public static class Polylabel
    {
        public static Vector<double> CalculatePoleOfInaccessibility(Vector<double>[][] polygon, double precision = 1.0d)
        {
            // find the bounding box of the outer ring
            double minX = double.PositiveInfinity, minY = double.PositiveInfinity, maxX = double.NegativeInfinity, maxY = double.NegativeInfinity;
            for (var i = 0; i < polygon[0].Length; i++)
            {
                var p = polygon[0][i];
                if (p[0] < minX) minX = p[0];
                if (p[1] < minY) minY = p[1];
                if (p[0] > maxX) maxX = p[0];
                if (p[1] > maxY) maxY = p[1];
            }

            var width = maxX - minX;
            var height = maxY - minY;
            var cellSize = Math.Min(width, height);
            var maxSize = Math.Max(width, height);
            var h = cellSize / 2;

            if (cellSize == 0) return ConstructVectorDouble(minX, minY);

            // a priority queue of cells in order of their "potential" (max distance to polygon)
            var cellQueue = new PriorityQueue<Cell>(null);

            var centroid = GetCentroidOfPolygon(polygon);

            Func<Vector<double>, double, double> fitnessFunc = (Vector<double> point, double distance) =>
            {
                if (distance <= 0)
                {
                    return distance;
                }
                var d = point - centroid;
                var distanceCentroid = Math.Sqrt(Vector.Dot(d, d));
                return distance * (1 - distanceCentroid / maxSize);
            };

            // cover polygon with initial cells
            for (var x = minX; x < maxX; x += cellSize)
            {
                for (var y = minY; y < maxY; y += cellSize)
                {
                    cellQueue.Enqueue(new Cell(ConstructVectorDouble(x + h, y + h), h, polygon, fitnessFunc));
                }
            }



            // take centroid as the first best guess
            var bestCell = new Cell(centroid, 0, polygon, fitnessFunc);


            while (cellQueue.Length > 0)
            {
                // pick the most promising cell from the queue
                var cell = cellQueue.Dequeue();

                // update the best cell if we found a better one
                if (cell.Fitness > bestCell.Fitness)
                {
                    bestCell = cell;
                }

                // do not drill down further if there's no chance of a better solution
                if (cell.MaxFitness - bestCell.Fitness <= precision)
                {
                    continue;
                }

                // split the cell into four cells
                h = cell.HalfCellSize / 2;
                cellQueue.Enqueue(new Cell(ConstructVectorDouble(cell.Center[0] - h, cell.Center[1] - h), h, polygon, fitnessFunc));
                cellQueue.Enqueue(new Cell(ConstructVectorDouble(cell.Center[0] + h, cell.Center[1] - h), h, polygon, fitnessFunc));
                cellQueue.Enqueue(new Cell(ConstructVectorDouble(cell.Center[0] - h, cell.Center[1] + h), h, polygon, fitnessFunc));
                cellQueue.Enqueue(new Cell(ConstructVectorDouble(cell.Center[0] + h, cell.Center[1] + h), h, polygon, fitnessFunc));
            }

            return bestCell.Center;
        }

        // signed distance from point to polygon outline (negative if point is outside)
        public static double GetDistanceFromPointToPolygonOutline(Vector<double> point, Vector<double>[][] polygon)
        {
            var inside = false;
            var minDistSq = double.PositiveInfinity;

            for (var k = 0; k < polygon.Length; k++)
            {
                var ring = polygon[k];
                for (int i = 0, len = ring.Length, j = len - 1; i < len; j = i++)
                {
                    var a = ring[i];
                    var b = ring[j];

                    if (IsPointInsidePolygon(point, a, b))
                    {
                        inside = !inside;
                    }

                    minDistSq = Math.Min(minDistSq, GetSquaredDistanceFromPointToSegment(point, a, b));
                }
            }

            return (inside ? 1 : -1) * Math.Sqrt(minDistSq);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsPointInsidePolygon(Vector<double> point, Vector<double> a, Vector<double> b)
        {
            return (a[1] > point[1] != b[1] > point[1]) && (point[0] < (b[0] - a[0]) * (point[1] - a[1]) / (b[1] - a[1]) + a[0]);
        }

        // get squared distance from a point to a segment
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double GetSquaredDistanceFromPointToSegment(Vector<double> point, Vector<double> a, Vector<double> b)
        {
            var ab = b - a;
            var ap = point - a;

            var c = Vector.Dot(ap, ab);

            if (c <= 0)
            {
                return Vector.Dot(ap, ap);
            }

            var bp = point - b;

            if(Vector.Dot(bp, ab) >= 0)
            {
                return Vector.Dot(bp, bp);
            }

            var e = ap - ab * (c / Vector.Dot(ab, ab));

            return Vector.Dot(e, e);
        }

        // get polygon centroid
        private static Vector<double> GetCentroidOfPolygon(Vector<double>[][] polygon)
        {
            var area = 0d;
            var x = 0d;
            var y = 0d;
            var points = polygon[0];

            for (int i = 0, len = points.Length, j = len - 1; i < len; j = i++)
            {
                var a = points[i];
                var b = points[j];
                var f = a[0] * b[1] - b[0] * a[1];
                x += (a[0] + b[0]) * f;
                y += (a[1] + b[1]) * f;
                area += f * 3;
            }

            if (area == 0)
            {
                return points[0];
            }

            return ConstructVectorDouble(x / area, y / area);
        }

        private static Vector<double> ConstructVectorDouble(double x, double y)
        {
            var doubles = new double[Vector<double>.Count];
            doubles[0] = x;
            doubles[1] = y;
            return new Vector<double>(doubles);
        }
    }
}
