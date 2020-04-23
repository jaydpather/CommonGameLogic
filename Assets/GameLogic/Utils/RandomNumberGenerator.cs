using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThirdEyeSoftware.GameLogic.Utils
{
    public class RandomNumberGenerator
    {
        private float? _prevRandomPct = null;
        private Random _random = new Random(DateTime.Now.Millisecond);
        private float? _minDiffPctFromPrev = null;

        public RandomNumberGenerator(float? minDiffPctFromPrev) //todo 2nd game: this should be changed to minDiffFromPrev, not Pct. (it should be the minimum difference between return values of GetRandomValue, not GetRandomPct
        {
            _minDiffPctFromPrev = minDiffPctFromPrev;
        }

        public float GetRandomValue(float min, float max)
        {
            var range = max - min;
            var randomPct = GetRandomPct();

            var retVal = min + randomPct * range;

            return retVal;
        }

        /// <summary>
        /// returns a random float between 0 and 1. if you specify a value for prev, then the return value will be minDiff greater or smaller than prev. (this controls the distribution - it prevents you from getting two numbers too close to each other in a row).
        /// </summary>
        /// <param name="minDiffFromPrev">the minimum difference between the prev and the current return value</param>
        /// <returns></returns>
        public float GetRandomPct()
        {
            var randomInt = _random.Next(0, 100);
            var randomFloat = randomInt / 100f;


            if (_prevRandomPct.HasValue && _minDiffPctFromPrev.HasValue)
            {
                while (Math.Abs(randomFloat - _prevRandomPct.Value) < _minDiffPctFromPrev.Value)
                {
                    randomInt = _random.Next(0, 100);
                    randomFloat = randomInt / 100f;
                }
            }

            _prevRandomPct = randomFloat;

            return randomFloat;
        }

    }
}
