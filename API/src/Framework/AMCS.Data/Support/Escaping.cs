using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Support
{
  public static class Escaping
  {
    public static string HexEncode(byte[] bytes)
    {
      var sb = new StringBuilder(bytes.Length * 2);

      foreach (byte b in bytes)
      {
        sb.Append(b.ToString("x2"));
      }

      return sb.ToString();
    }

    public static byte[] HexDecode(string value)
    {
      if (value.Length % 2 != 0)
        throw new ArgumentException("Invalid hex length");

      byte[] result = new byte[value.Length / 2];

      for (int i = 0; i < result.Length; i++)
      {
        result[i] = (byte)(HexDecode(value[i * 2]) << 4 | HexDecode(value[i * 2 + 1]));
      }

      return result;
    }

    private static int HexDecode(char c)
    {
      if (c >= '0' && c <= '9')
        return c - '0';
      if (c >= 'a' && c <= 'f')
        return c - 'a' + 10;
      if (c >= 'A' && c <= 'F')
        return c - 'A' + 10;
      throw new ArgumentException("Invalid hex character");
    }
  }
}
