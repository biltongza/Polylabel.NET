namespace Polylabel.NET.Tests
{
    using Newtonsoft.Json;
    using NUnit.Framework;

    [TestFixture]
    public class PolylabelTests
    {
        private static double[][][] water1 = FixtureHelper.GetFixture<double[][][]>("water1.json");
        private static double[][][] water2 = FixtureHelper.GetFixture<double[][][]>("water2.json");

        [Test]
        public void FindsPoleOfInaccessibilityForWater1AndPrecision1()
        {
            var result = Polylabel.CalculatePoleOfInaccessibility(water1, 1);
            Assert.AreEqual((3865.85009765625, 2124.87841796875), result);
        }

        [Test]
        public void FindsPoleOfInaccessibilityForWater1AndPrecision50()
        {
            var result = Polylabel.CalculatePoleOfInaccessibility(water1, 50);
            Assert.AreEqual((3854.296875, 2123.828125), result);
        }

        [Test]
        public void FindsPoleOfInaccessibilityForWater2AndDefaultPrecision1()
        {
            var result = Polylabel.CalculatePoleOfInaccessibility(water2);
            Assert.AreEqual((3263.5, 3263.5), result);
        }

        [Test]
        public void WorksOnDegeneratePolygons()
        {
            var first = JsonConvert.DeserializeObject<double[][][]>("[[[0, 0], [1, 0], [2, 0], [0, 0]]]");
            var firstResult = Polylabel.CalculatePoleOfInaccessibility(first);
            Assert.AreEqual((0, 0), firstResult);

            var second = JsonConvert.DeserializeObject<double[][][]>("[[[0, 0], [1, 0], [1, 1], [1, 0], [0, 0]]]");
            var secondResult = Polylabel.CalculatePoleOfInaccessibility(second);
            Assert.AreEqual((0, 0), secondResult);
        }
    }
}
