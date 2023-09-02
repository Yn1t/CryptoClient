using System;
using System.Numerics;

namespace Benalo.classes
{
    public sealed class TestSoloveyStrassen : interfaces.ISimplicityTests
    {
        public bool GetSimplicityTest(BigInteger number, double probability)
        {
            if (probability >= 1 || probability < 0.5)
                throw new ArgumentOutOfRangeException(nameof(probability), "Incorrect value of probability");

            if (number == 1)
                return (false);
            for (var i = 0; 1.0 - Math.Pow(2, -i) <= probability; i++)
            {
                var a = extra.ExtraFunctional.GetRandomInteger(2, number - 1);

                if (BigInteger.GreatestCommonDivisor(a, number) > 1)
                    return (false);
                if (BigInteger.ModPow(a, (number - 1) / 2, number) != extra.ExtraFunctional.GetJacobi(a, number))
                    return (false);
            }

            return (true);
        }
    }
}