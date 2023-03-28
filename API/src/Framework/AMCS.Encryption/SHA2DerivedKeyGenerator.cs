using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AMCS.Encryption
{
  public static class SHA2DerivedKeyGenerator
  {
    public static byte[] GetDerivedKey(byte[] key, int bits)
    {
      switch (bits)
      {
        case 256:
          return GetDerivedKey(key, SHA256.Create());
        case 384:
          return GetDerivedKey(key, SHA384.Create());
        case 512:
          return GetDerivedKey(key, SHA512.Create());
        default:
          throw new ArgumentException("Unsupported key size");
      }
    }

    private static byte[] GetDerivedKey(byte[] key, HashAlgorithm algorithm)
    {
      using (algorithm)
      {
        return algorithm.ComputeHash(key, 0, key.Length);
      }
    }
  }
}
