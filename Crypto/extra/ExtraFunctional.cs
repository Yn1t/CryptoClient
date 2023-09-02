using System;
using System.Numerics;
using System.Security.Cryptography;

namespace Benalo.extra
{
    public static class ExtraFunctional
    {
        public static BigInteger GetRandomInteger(BigInteger a, BigInteger b)
        {
            BigInteger number = new BigInteger();
            var generated = RandomNumberGenerator.Create();
            byte[] bytesArray = b.ToByteArray();

            do
            {
                generated.GetBytes(bytesArray);
                number = new BigInteger(bytesArray);
                // number = (number & 0b1111111) | 1;
            } while (!(number >= a && number < b));
            
            return (number);
        }

        public static BigInteger GetJacobi(BigInteger a, BigInteger p)
        {
            if (BigInteger.GreatestCommonDivisor(a, p) != 1)
                return (0);
            int r = 1;
            
            if (a < 0)
            {
                a = (-1) * a;
                if (p % 4 == 3)
                    r = (-1) * r;
            }

            do
            {
                var t = 0;
                while (a % 2 == 0)
                {
                    t += 1;
                    a = a / 2;
                }

                if (t % 2 == 1)
                {
                    if (p % 8 == 3 || p % 8 == 5)
                        r = (-1) * r;
                }

                if (a % 4 == 3 && p % 4 == 3)
                {
                    r = (-1) * r;
                }
                
                var c = a;
                a = p % c;
                p = c;
                
            } while (a != 0) ;
            
            return (r);
        }
    }
}