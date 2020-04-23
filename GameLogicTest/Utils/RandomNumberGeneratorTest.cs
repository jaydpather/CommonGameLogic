using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdEyeSoftware.GameLogic.Utils;

namespace GameLogicTest.Utils
{
    [TestClass]
    public class RandomNumberGeneratorTest : TestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {

        }

        [TestMethod]
        public void ConstructorTest()
        {
            var generator = new RandomNumberGenerator(1);
            var minDiffPctFromPrev = GetPrivateMember<float?>("_minDiffPctFromPrev", generator);
            Assert.AreEqual(1, minDiffPctFromPrev);

            generator = new RandomNumberGenerator(null);
            minDiffPctFromPrev = GetPrivateMember<float?>("_minDiffPctFromPrev", generator);
            Assert.AreEqual(null, minDiffPctFromPrev);

        }

        [TestMethod]
        public void GetRandomValue_NoMinDiff()
        {
            var generator = new RandomNumberGenerator(null);
            var minValue = 0;
            var maxValue = 100;


            var results = new float[10];
            for(var i = 0; i < results.Length; i++)
            {
                results[i] = generator.GetRandomValue(minValue, maxValue);
            }

            //assert:
            for(var i=0; i<results.Length; i++)
            {
                Assert.IsTrue(results[i] >= minValue);
                Assert.IsTrue(results[i] <= maxValue);
            }
        }

        [TestMethod]
        public void GetRandomValue_WithMinDiff()
        {
            var minValue = 0;
            var maxValue = 100;
            var minDiffResults = 10;

            var generator = new RandomNumberGenerator(0.1f);

            var results = new float[10];
            for (var i = 0; i < results.Length; i++)
            {
                results[i] = generator.GetRandomValue(minValue, maxValue);
            }

            //assert:
            for (var i = 0; i < results.Length; i++)
            {
                Assert.IsTrue(results[i] >= minValue);
                Assert.IsTrue(results[i] <= maxValue);

                if(i < results.Length - 1)
                {
                    var diff = Math.Abs(results[i] - results[i + 1]);
                    Assert.IsTrue(diff >= minDiffResults);
                }
            }
        }

        [TestMethod]
        public void GetRandomPct_NoMinDiff()
        {
            var generator = new RandomNumberGenerator(null);
            var results = new float[10];

            for(var i=0; i<results.Length; i++)
            { 
                results[i] = generator.GetRandomPct();
            }

            //assert:
            for (var i = 0; i < results.Length; i++)
            {
                Assert.IsTrue(results[i] >= 0);
                Assert.IsTrue(results[i] <= 100);
            }
        }

        [TestMethod]
        public void GetRandomPct_WithMinDiff()
        {
            var minDiff = 0.1f;
            var generator = new RandomNumberGenerator(minDiff);
            var results = new float[10];

            for (var i = 0; i < results.Length; i++)
            {
                results[i] = generator.GetRandomPct();
            }

            //assert:
            for (var i = 0; i < results.Length; i++)
            {
                Assert.IsTrue(results[i] >= 0);
                Assert.IsTrue(results[i] <= 100);

                if(i < results.Length - 1)
                {
                    var diff = Math.Abs(results[i] - results[i + 1]);
                    Assert.IsTrue(diff >= minDiff);
                }
            }
        }
    }
}
