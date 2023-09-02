using System.Numerics;

namespace Benalo.interfaces
{
    public struct Key
    {
        /* public key */
        public BigInteger y;
        public BigInteger r;
        public BigInteger n;
        
        /* secret key */
        public BigInteger fi;
        public BigInteger x;
    }

    public interface IKeyGenerator
    {
        public Key GetPublicOpenKeys(BigInteger block);
    }
}