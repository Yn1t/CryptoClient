using System.Numerics;

namespace Benalo.interfaces
{
    public interface IAsymmetricEncryptionDecription
    {
        public BigInteger Encryption(BigInteger block);
        
        public BigInteger Decryption(BigInteger block);

        public void GetPublicOpenKeys(BigInteger block);

    }
}