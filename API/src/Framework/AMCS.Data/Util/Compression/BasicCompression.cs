namespace AMCS.Data.Util.Compression
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.IO.Compression;
  using System.Linq;
  using System.Text;

  public static class BasicCompression
  {
    /**
     * Returns the compressed version of "uncompressed"
     */
    public static byte[] Compress(byte[] uncompressed)
    {
      byte[] compressed;
      using (MemoryStream compressedStream = new MemoryStream())
      {
        using (GZipStream zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
        {
          zipStream.Write(uncompressed, 0, uncompressed.Length);
          zipStream.Flush();
        }
        compressed = compressedStream.ToArray();
      }

      return compressed;
    }

    /**
     * Returns the uncompressed version of "compressed"
     */
    public static byte[] Decompress(byte[] compressed)
    {
      byte[] uncompressed;

      using (MemoryStream compressedStream = new MemoryStream(compressed))
      {
        using (MemoryStream uncompressedStream = new MemoryStream())
        {
          byte[] buffer = new byte[4096];
          using (GZipStream zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
          {

            int bytesRead = -1;
            while (bytesRead != 0)
            {
              bytesRead = zipStream.Read(buffer, 0, buffer.Length);
              uncompressedStream.Write(buffer, 0, bytesRead);
            }
            zipStream.Flush();
          }
          uncompressed = uncompressedStream.ToArray();
        }
      }
      return uncompressed;
    }
  }
}
