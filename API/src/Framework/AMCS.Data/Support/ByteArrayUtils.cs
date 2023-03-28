using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Support
{
  public static class ByteArrayUtils
  {
    public static Stream GetDecompressedStream(byte[] data)
    {
      if (data == null)
        return null;

      Stream stream = new MemoryStream(data);

      if (IsGzip(data))
        stream = new GZipStream(stream, CompressionMode.Decompress);

      return stream;
    }

    public static byte[] GetDecompressed(byte[] data)
    {
      if (data == null)
        return null;

      if (!IsGzip(data))
        return data;

      using (var target = new MemoryStream())
      {
        using (var source = GetDecompressedStream(data))
        {
          source.CopyTo(target);
        }

        return target.ToArray();
      }
    }

    private static bool IsGzip(byte[] data)
    {
      if (data.Length < 3)
        return false;

      return
        data[0] == 0x1f &&
        data[1] == 0x8b &&
        data[2] == 0x08;
    }
  }
}
