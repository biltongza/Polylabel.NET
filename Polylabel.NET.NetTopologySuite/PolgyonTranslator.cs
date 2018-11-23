using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;

namespace Polylabel.NET.NetTopologySuite
{
    public static class PolgyonTranslator
    {
        public static double[][][] GetCoordinatesFromPolygon(MultiPolygon polygon)
        {
            List<double[][]> coords = new List<double[][]>();
            foreach(var sub in polygon)
            {
                coords.Add(sub.Coordinates.Select(c => new[] { c.X, c.Y }).ToArray());
            }

            return coords.ToArray();
        }
    }
}
