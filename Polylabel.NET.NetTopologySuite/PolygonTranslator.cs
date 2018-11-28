using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;

namespace Polylabel.NET.NetTopologySuite
{
    public static class PolygonTranslator
    {
        public static double[][][] GetCoordinatesFromPolygon(MultiPolygon polygon)
        {
            return polygon.Geometries.Select(p => p.Coordinates.Select(c => new[] { c.X, c.Y }).ToArray()).ToArray();
        }
    }
}
