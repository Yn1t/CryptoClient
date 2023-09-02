using System;
using System.Linq.Expressions;
using System.Numerics;
using Benalo.interfaces;

namespace Benalo.classes
{
    public sealed class KeyGenerator : interfaces.IKeyGenerator
    {
        public readonly Tests _nameTest;
        public readonly double _probability;
        public readonly ulong _size;

        public KeyGenerator(Tests nameTest, double probability, ulong size)
        {
            _size = size;
            _nameTest = nameTest;
            _probability = probability;
        }

        public Key GetPublicOpenKeys(BigInteger block)
        {
            Key paarKey = new Key();
            var p = GetRandomPrimeNumber();
            var q = GetRandomPrimeNumber();
            paarKey.n = BigInteger.Multiply(p, q);
            paarKey.fi = BigInteger.Multiply(p - 1, q - 1);
            paarKey.r = block; /* mess */

            while (true)
            {
                ++paarKey.r;
                if (BigInteger.GreatestCommonDivisor(paarKey.r, (p - 1) / paarKey.r) == 1
                    && BigInteger.GreatestCommonDivisor(paarKey.r, q - 1) == 1
                    && (p - 1) % paarKey.r == 0)
                    break;
            }

            while (true)
            {
                paarKey.y = extra.ExtraFunctional.GetRandomInteger(1, paarKey.n);
                paarKey.x = BigInteger.ModPow(paarKey.y, BigInteger.Divide(paarKey.fi, paarKey.r), paarKey.n);
                if (paarKey.x != 1)
                    break;
            }
            return (paarKey);
        }
        
        public BigInteger GetRandomPrimeNumber()
        {
            byte[] massBytes = new byte[_size];
            var random = new Random();

            while (true)
            {
                random.NextBytes(massBytes); /* haha */
                var primeNum = new BigInteger(massBytes);
                if (primeNum < 2)
                    continue;

                switch (_nameTest)
                {
                    case Tests.SoloveyStrassen:
                    {
                        var soloveyStrassen = new classes.TestSoloveyStrassen();
                        var boolPerem = soloveyStrassen.GetSimplicityTest(primeNum, _probability);
                        if (boolPerem == true)
                            return (primeNum);
                        break ;
                    }
                    case Tests.MillerRabin:
                    {
                        var rabinMiller = new classes.TestMillerRabin();
                        var boolPerem = rabinMiller.GetSimplicityTest(primeNum, _probability);
                        if (boolPerem == true)
                            return (primeNum);
                        break ;
                    }
                    case Tests.Ferma:
                    {
                        var ferma = new classes.TestFerma();
                        var boolPerem = ferma.GetSimplicityTest(primeNum, _probability);
                        if (boolPerem == true)
                            return (primeNum);
                        break ;
                    }
                    default:
                        throw new ArgumentException("There is only Miller-Rabin, Ferma and Solovey-Strassen tests!");
                }
            }
        }
        
    }
}