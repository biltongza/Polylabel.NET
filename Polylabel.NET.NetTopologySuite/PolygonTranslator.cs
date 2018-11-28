using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NetTopologySuite.Geometries;

namespace Polylabel.NET.NetTopologySuite
{
    public static class PolygonTranslator
    {
        public static Vector<double>[][] GetCoordinatesFromPolygon(MultiPolygon polygon)
        {
            return polygon.Geometries.Select(p => p.Coordinates.Select(c => new Vector<double>(new[] { c.X, c.Y, 0, 0 })).ToArray()).ToArray();
        }
    }
}
