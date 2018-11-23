namespace Polylabel.NET.Tests
{
    using System.IO;
    using System.Reflection;
    using System.Text;
    using NetTopologySuite.IO;
    using Newtonsoft.Json;

    public static class FixtureHelper
    {
        public static T GetFixture<T>(string name)where T : class
        {
            FileInfo currentAssemblyInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var fileString = Encoding.UTF8.GetString(File.ReadAllBytes(Path.Join(currentAssemblyInfo.Directory.FullName, "fixtures", name)));

            return new GeoJsonReader().Read<T>(fileString);
        }
    }
}
