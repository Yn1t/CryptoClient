using System.Numerics;
using Benalo.interfaces;

namespace Benalo.classes
{
    public sealed class AsymmetricEncryptionDecryption : interfaces.IAsymmetricEncryptionDecription
    {
        private Key _keys;
        private readonly classes.KeyGenerator? _keyGenerator;

        public AsymmetricEncryptionDecryption(BigInteger block, Tests nameTest, double probability, ulong size)
        {
            _keyGenerator = new KeyGenerator(nameTest, probability, size);
            _keys = _keyGenerator.GetPublicOpenKeys(block);
        }

        public AsymmetricEncryptionDecryption(Key keys)
        {
            _keys = keys;
        }

        public BigInteger Encryption(BigInteger block)
        {
            BigInteger u;
            while (true)
            {
                u = extra.ExtraFunctional.GetRandomInteger(2, _keys.n - 1);
                if (BigInteger.GreatestCommonDivisor(u, _keys.n) == 1)
                    break;
            }

            var a = BigInteger.ModPow(_keys.y, block, _keys.n);
            var b = BigInteger.ModPow(u, _keys.r, _keys.n);
            return (BigInteger.Multiply(a, b) % _keys.n);
        }

        public BigInteger Decryption(BigInteger block)
        {
            BigInteger mRes = 0;
            var a = BigInteger.ModPow(block, _keys.fi / _keys.r, _keys.n);
            for (BigInteger m = 0; m < _keys.r; m++)
            {
                var value = BigInteger.ModPow(_keys.x, m, _keys.n);
                if (value == a)
                    mRes = m;
            }
            return (mRes);
        }

        public void GetPublicOpenKeys(BigInteger block)
        {
            if (_keyGenerator != null) 
            { 
                _keys = _keyGenerator.GetPublicOpenKeys(block);
            }
        }

        public Key GetOnlyOpenKey()
        {
            Key openKey = new Key();
            openKey.n = _keys.n;
            openKey.y = _keys.y;
            openKey.r = _keys.r;

            return openKey;
        }
    }
}