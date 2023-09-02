using System.Numerics;

namespace Benalo.interfaces
{
    public enum Tests
    {
        Ferma, SoloveyStrassen, 
        MillerRabin
    }

    public interface ISimplicityTests
    {
        public bool GetSimplicityTest(BigInteger number, double probability);
    }
}