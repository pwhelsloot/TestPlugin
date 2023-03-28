using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Encryption.Test
{
  internal class RandomText
  {
    private static string KeyChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static string CreateKey(int length)
    {
      return CreateKey(SharedRandom.GetRandom(), length);
    }

    public static string CreateKey(Random random, int length)
    {
      var sb = new StringBuilder(length);

      for (int i = 0; i < length; i++)
      {
        sb.Append(KeyChars[random.Next(0, KeyChars.Length)]);
      }

      return sb.ToString();
    }
  }
}
