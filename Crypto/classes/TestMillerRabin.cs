using System;
using System.Numerics;

namespace Benalo.classes
{
    public sealed class TestMillerRabin : interfaces.ISimplicityTests
    {
        public bool GetSimplicityTest(BigInteger number, double probability)
        {
            if (probability < 0.7 || probability >= 1)
                throw new ArgumentOutOfRangeException(nameof(probability), "minProbability must be [0.7, 1)");
            if (number == 1)
                return false;

            var d = number - 1;
            var s = 0;
            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            for (var i = 0; 1.0 - Math.Pow(4, -i) <= probability; ++i)
            {
                var a = extra.ExtraFunctional.GetRandomInteger(2, number - 1);
                var x = BigInteger.ModPow(a, d, number);
                if (x == 1 || x == number - 1)
                    continue;

                for (var r = 1; r < s; ++r)
                {
                    x = BigInteger.ModPow(x, 2, number);
                    if (x == 1)
                        return false;
                    if (x == number - 1)
                        break;
                }

                if (x != number - 1)
                    return false;
            }

            return true;
        }
    }
}