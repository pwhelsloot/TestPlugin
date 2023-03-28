using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Rules
{
  internal static partial class ValueComparison
  {
    private static readonly Dictionary<(Type, Type), IComparer> _comparers = BuildComparers();

    private static Dictionary<(Type, Type), IComparer> BuildComparers()
    {
      var comparers = new Dictionary<(Type, Type), IComparer>();

      comparers.Add((typeof(bool), typeof(bool)), new BoolBoolComparer());
      comparers.Add((typeof(string), typeof(string)), new StringStringComparer());
      comparers.Add((typeof(TimeSpan), typeof(TimeSpan)), new TimeSpanTimeSpanComparer());
      comparers.Add((typeof(DateTime), typeof(DateTime)), new DateTimeDateTimeComparer());
      comparers.Add((typeof(DateTimeOffset), typeof(DateTimeOffset)), new DateTimeOffsetDateTimeOffsetComparer());

      AddGeneratedComparers(comparers);

      return comparers;
    }

    public static IComparer GetComparer(Type left, Type right)
    {
      // Unwrap nullable types like int?. The comparers are written such that they support
      // both variants.

      if (left != null)
        left = Nullable.GetUnderlyingType(left) ?? left;
      if (right != null)
        right = Nullable.GetUnderlyingType(right) ?? right;

      // We promote either the left or right side to the other if one of them is null.

      if (left == null)
        left = right;
      if (right == null)
        right = left;

      if (_comparers.TryGetValue((left, right), out var comparer))
        return comparer;

      return null;
    }

    private class BoolBoolComparer : IComparer
    {
      public int Compare(object x, object y)
      {
        if (x == null)
          return y == null ? 0 : 1;
        if (y == null)
          return -1;

        bool left = (bool)x;
        bool right = (bool)y;

        return left.CompareTo(right);
      }
    }

    private class StringStringComparer : IComparer
    {
      public int Compare(object x, object y)
      {
        string left = (string)x;
        string right = (string)y;

        return String.Compare(left, right, StringComparison.CurrentCulture);
      }
    }

    private class TimeSpanTimeSpanComparer : IComparer
    {
      public int Compare(object x, object y)
      {
        if (x == null)
          return y == null ? 0 : 1;
        if (y == null)
          return -1;

        var left = (TimeSpan)x;
        var right = (TimeSpan)y;

        return left.CompareTo(right);
      }
    }

    private class DateTimeDateTimeComparer : IComparer
    {
      public int Compare(object x, object y)
      {
        if (x == null)
          return y == null ? 0 : 1;
        if (y == null)
          return -1;

        var left = (DateTime)x;
        var right = (DateTime)y;

        return left.CompareTo(right);
      }
    }

    private class DateTimeOffsetDateTimeOffsetComparer : IComparer
    {
      public int Compare(object x, object y)
      {
        if (x == null)
          return y == null ? 0 : 1;
        if (y == null)
          return -1;

        var left = (DateTimeOffset)x;
        var right = (DateTimeOffset)y;

        return left.CompareTo(right);
      }
    }
  }
}
