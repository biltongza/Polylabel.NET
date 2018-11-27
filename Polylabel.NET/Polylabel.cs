namespace Polylabel.NET
{
    using System;
    

    public static class Polylabel
    {
        public static Point CalculatePoleOfInaccessibility(double[][][] polygon, double precision = 1.0d)
        {
            // find the bounding box of the outer ring
            double minX = 0d, minY = 0d, maxX = 0d, maxY = 0d;
            for (var i = 0; i < polygon[0].Length; i++)
            {
                var p = polygon[0][i];
                if (i == 0 || p[0] < minX) minX = p[0];
                if (i == 0 || p[1] < minY) minY = p[1];
                if (i == 0 || p[0] > maxX) maxX = p[0];
                if (i == 0 || p[1] > maxY) maxY = p[1];
            }

            var width = maxX - minX;
            var height = maxY - minY;
            var cellSize = Math.Min(width, height);
            var maxSize = Math.Max(width, height);
            var h = cellSize / 2;

            if (cellSize == 0) return new Point(minX, minY);

            // a priority queue of cells in order of their "potential" (max distance to polygon)
            var cellQueue = new PriorityQueue<Cell>(null);

            var centroid = GetCentroidOfPolygon(polygon);

            Func<Point, double, double> fitnessFunc = (Point point, double distance) =>
            {
                if (distance <= 0)
                {
                    return distance;
                }
                var d = point - centroid;
                var distanceCentroid = Math.Sqrt(d.X * d.X + d.Y * d.Y);
                return distance * (1 - distanceCentroid / maxSize);
            };

            // cover polygon with initial cells
            for (var x = minX; x < maxX; x += cellSize)
            {
                for (var y = minY; y < maxY; y += cellSize)
                {
                    cellQueue.Enqueue(new Cell(new Point(x + h, y + h), h, polygon, fitnessFunc));
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
                cellQueue.Enqueue(new Cell(new Point(cell.Center.X - h, cell.Center.Y - h), h, polygon, fitnessFunc));
                cellQueue.Enqueue(new Cell(new Point(cell.Center.X + h, cell.Center.Y - h), h, polygon, fitnessFunc));
                cellQueue.Enqueue(new Cell(new Point(cell.Center.X - h, cell.Center.Y + h), h, polygon, fitnessFunc));
                cellQueue.Enqueue(new Cell(new Point(cell.Center.X + h, cell.Center.Y + h), h, polygon, fitnessFunc));
            }

            return bestCell.Center;
        }

        // signed distance from point to polygon outline (negative if point is outside)
        public static double GetDistanceFromPointToPolygonOutline(Point point, double[][][] polygon)
        {
            var inside = false;
            var minDistSq = double.PositiveInfinity;

            for (var k = 0; k < polygon.Length; k++)
            {
                var ring = polygon[k];
                for (int i = 0, len = ring.Length, j = len - 1; i < len; j = i++)
                {
                    var a = new Point(ring[i][0], ring[i][1]);
                    var b = new Point(ring[j][0], ring[j][1]);

                    if ((a.Y > point.Y != b.Y > point.Y) && (point.X < (b.X - a.X) * (point.Y - a.Y) / (b.Y - a.Y) + a.X))
                    {
                        inside = !inside;
                    }

                    minDistSq = Math.Min(minDistSq, GetSquaredDistanceFromPointToSegment(point, a, b));
                }
            }

            return (inside ? 1 : -1) * Math.Sqrt(minDistSq);
        }

        // get squared distance from a point to a segment
        private static double GetSquaredDistanceFromPointToSegment(Point point, Point a, Point b)
        {
            var x = a.X;
            var y = a.Y;
            var dx = b.X - x;
            var dy = b.Y - y;

            if (dx != 0 || dy != 0)
            {
                var t = ((point.X - x) * dx + (point.Y - y) * dy) / (dx * dx + dy * dy);

                if (t > 1)
                {
                    x = b.X;
                    y = b.Y;

                }
                else if (t > 0)
                {
                    x += dx * t;
                    y += dy * t;
                }
            }

            dx = point.X - x;
            dy = point.Y - y;

            return dx * dx + dy * dy;
        }

        // get polygon centroid
        private static Point GetCentroidOfPolygon(double[][][] polygon)
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
                return new Point(points[0][0], points[0][1]);
            }

            return new Point(x / area, y / area);
        }
    }
}
