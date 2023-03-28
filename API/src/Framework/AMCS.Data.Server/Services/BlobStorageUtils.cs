using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Support;

namespace AMCS.Data.Server.Services
{
  internal static class BlobStorageUtils
  {
    private static readonly byte[] EmptyBytes = new byte[0];

    public static string ComputeHash(Stream stream)
    {
      using (var hasher = SHA256.Create())
      {
        return Escaping.HexEncode(hasher.ComputeHash(stream));
      }
    }

    public static string CopyStreamComputeHash(Stream source, Stream target)
    {
      using (var hasher = SHA256.Create())
      {
        hasher.Initialize();

        var buffer = new byte[4096];
        int read;

        while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
        {
          hasher.TransformBlock(buffer, 0, read, null, 0);

          target.Write(buffer, 0, read);
        }

        hasher.TransformFinalBlock(EmptyBytes, 0, 0);

        return Escaping.HexEncode(hasher.Hash);
      }
    }
  }
}
