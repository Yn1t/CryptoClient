using System;
using System.Numerics;

namespace Benalo.classes
{
    public class TestFerma : interfaces.ISimplicityTests
    {
        public bool GetSimplicityTest(BigInteger number, double probability)
        {
            if (probability <= 0.5 || probability > 1)
                throw new ArgumentOutOfRangeException(nameof(probability), "Incorrect value of probability");
            
            if (number == 1)
                return (false);
            
            for (var i = 0; 1.0 - Math.Pow(2, -i) <= probability; i++)
            {
                var a = extra.ExtraFunctional.GetRandomInteger(2, number - 1);
                
                if (BigInteger.ModPow(a, number - 1, number) != 1)
                    return (false);
            }
            return (true);
        }
    }
}