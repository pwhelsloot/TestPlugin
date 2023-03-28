using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Encryption.Test
{
  internal static class SharedRandom
  {
    private static readonly object syncRoot = new object();
    private static readonly Random random = new Random();

    public static Random GetRandom()
    {
      lock (syncRoot)
      {
        return new Random(random.Next());
      }
    }
  }
}
