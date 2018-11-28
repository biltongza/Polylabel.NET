namespace Polylabel.NET.Tests
{
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using System.Reflection;
    using System.Text;
    using NetTopologySuite.IO;
    using Newtonsoft.Json;

    public static class FixtureHelper
    {
        public static Vector<double>[][] GetFixture(string name)
        {
            FileInfo currentAssemblyInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var fileString = Encoding.UTF8.GetString(File.ReadAllBytes(Path.Join(currentAssemblyInfo.Directory.FullName, "fixtures", name)));

            var coords = new GeoJsonReader().Read<double[][][]>(fileString);

            return coords.Select(r => r.Select(c => new Vector<double>(new double[] { c[0], c[1], 0, 0 })).ToArray()).ToArray();
        }
    }
}
