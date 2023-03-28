using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Support
{
  public static class CollectionExtensions
  {
    public static void AddRange<T>(this ICollection<T> self, IEnumerable<T> values)
    {
      foreach (var value in values)
      {
        self.Add(value);
      }
    }
  }
}
