namespace Polylabel.NET
{
    using System;

    public static class Polylabel
    {
        public static (double, double) CalculatePoleOfInaccessibility(double[][][] polygon, double precision = 1.0d)
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
            var h = cellSize / 2;

            if (cellSize == 0) return (minX, minY);

            // a priority queue of cells in order of their "potential" (max distance to polygon)
            var cellQueue = new PriorityQueue<Cell>(null);

            // cover polygon with initial cells
            for (var x = minX; x < maxX; x += cellSize)
            {
                for (var y = minY; y < maxY; y += cellSize)
                {
                    cellQueue.Enqueue(new Cell(x + h, y + h, h, polygon));
                }
            }

            // take centroid as the first best guess
            var bestCell = GetCentroidCellOfPolygon(polygon);

            // special case for rectangular polygons
            var bboxCell = new Cell(minX + width / 2, minY + height / 2, 0, polygon);
            if (bboxCell.DistanceFromCenterToPolygon > bestCell.DistanceFromCenterToPolygon)
            {
                bestCell = bboxCell;
            }


            while (cellQueue.Length > 0)
            {
                // pick the most promising cell from the queue
                var cell = cellQueue.Dequeue();

                // update the best cell if we found a better one
                if (cell.DistanceFromCenterToPolygon > bestCell.DistanceFromCenterToPolygon)
                {
                    bestCell = cell;
                }

                // do not drill down further if there's no chance of a better solution
                if (cell.MaxDistanceToPolygonWithinCell - bestCell.DistanceFromCenterToPolygon <= precision)
                {
                    continue;
                }

                // split the cell into four cells
                h = cell.HalfCellSize / 2;
                cellQueue.Enqueue(new Cell(cell.CenterX - h, cell.CenterY - h, h, polygon));
                cellQueue.Enqueue(new Cell(cell.CenterX + h, cell.CenterY - h, h, polygon));
                cellQueue.Enqueue(new Cell(cell.CenterX - h, cell.CenterY + h, h, polygon));
                cellQueue.Enqueue(new Cell(cell.CenterX + h, cell.CenterY + h, h, polygon));
            }

            return (bestCell.CenterX, bestCell.CenterY);
        }

        // signed distance from point to polygon outline (negative if point is outside)
        public static double GetDistanceFromPointToPolygonOutline(double x, double y, double[][][] polygon)
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

                    if ((a[1] > y != b[1] > y) && (x < (b[0] - a[0]) * (y - a[1]) / (b[1] - a[1]) + a[0]))
                    {
                        inside = !inside;
                    }

                    minDistSq = Math.Min(minDistSq, GetSquaredDistanceFromPointToSegment(x, y, a, b));
                }
            }

            return (inside ? 1 : -1) * Math.Sqrt(minDistSq);
        }

        // get squared distance from a point to a segment
        private static double GetSquaredDistanceFromPointToSegment(double px, double py, double[] a, double[] b)
        {
            var x = a[0];
            var y = a[1];
            var dx = b[0] - x;
            var dy = b[1] - y;

            if (dx != 0 || dy != 0)
            {
                var t = ((px - x) * dx + (py - y) * dy) / (dx * dx + dy * dy);

                if (t > 1)
                {
                    x = b[0];
                    y = b[1];

                }
                else if (t > 0)
                {
                    x += dx * t;
                    y += dy * t;
                }
            }

            dx = px - x;
            dy = py - y;

            return dx * dx + dy * dy;
        }

        // get polygon centroid
        private static Cell GetCentroidCellOfPolygon(double[][][] polygon)
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
                return new Cell(points[0][0], points[0][1], 0, polygon);
            }

            return new Cell(x / area, y / area, 0, polygon);
        }
    }
}
