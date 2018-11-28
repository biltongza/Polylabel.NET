namespace Polylabel.NET.Tests
{
    using Newtonsoft.Json;
    using NUnit.Framework;
    using System.Linq;
    using System.Numerics;

    [TestFixture]
    public class PolylabelTests
    {
        private static Vector<double>[][] water1 = FixtureHelper.GetFixture("water1.json");
        private static Vector<double>[][] water2 = FixtureHelper.GetFixture("water2.json");

        [Test]
        public void FindsPoleOfInaccessibilityForWater1AndPrecision1()
        {
            var result = Polylabel.CalculatePoleOfInaccessibility(water1, 1);
            Assert.AreEqual(new Vector<double>(new double[] { 3865.85009765625, 2124.87841796875, 0, 0 }), result);
        }

        [Test]
        public void FindsPoleOfInaccessibilityForWater1AndPrecision50()
        {
            var result = Polylabel.CalculatePoleOfInaccessibility(water1, 50);
            Assert.AreEqual(new Vector<double>(new double[] { 3854.296875, 2123.828125, 0, 0 }), result);
        }

        [Test]
        public void FindsPoleOfInaccessibilityForWater2AndDefaultPrecision1()
        {
            var result = Polylabel.CalculatePoleOfInaccessibility(water2);
            Assert.AreEqual(new Vector<double>(new double[] { 3263.5, 3263.5, 0, 0 }), result);
        }

        [Test]
        public void WorksOnDegeneratePolygons()
        {
            var first = JsonConvert.DeserializeObject<double[][][]>("[[[0, 0], [1, 0], [2, 0], [0, 0]]]");
            var firstResult = Polylabel.CalculatePoleOfInaccessibility(first.Select(r => r.Select(c => new Vector<double>(new[] { c[0], c[1], 0, 0 })).ToArray()).ToArray());
            Assert.AreEqual(Vector<double>.Zero, firstResult);

            var second = JsonConvert.DeserializeObject<double[][][]>("[[[0, 0], [1, 0], [1, 1], [1, 0], [0, 0]]]");
            var secondResult = Polylabel.CalculatePoleOfInaccessibility(second.Select(r => r.Select(c => new Vector<double>(new[] { c[0], c[1], 0, 0 })).ToArray()).ToArray());
            Assert.AreEqual(Vector<double>.Zero, secondResult);
        }
    }
}
