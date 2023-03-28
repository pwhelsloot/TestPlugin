using AMCS.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Fetch
{
  /// <summary>
  /// Compares two 2-value tuples by comparing the reference of each value.
  /// </summary>
  internal class TupleByReferenceComparer<T1, T2> : IEqualityComparer<(T1, T2)>
  {
    public static TupleByReferenceComparer<T1, T2> Instance = new TupleByReferenceComparer<T1, T2>();

    private TupleByReferenceComparer() { }

    public bool Equals((T1, T2) x, (T1, T2) y)
    {
      return ReferenceEquals(x.Item1, y.Item1)
        && ReferenceEquals(x.Item2, y.Item2);
    }

    public int GetHashCode((T1, T2) obj)
    {
      return RuntimeHelpers.GetHashCode(obj.Item1) * 800011
        + RuntimeHelpers.GetHashCode(obj.Item2);
    }
  }
}
