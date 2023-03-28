using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Support
{
  internal static class ArrayUtils
  {
    public static bool Contains<T>(T[] array, T value)
    {
      return Array.IndexOf(array, value) != -1;
    }
  }
}
