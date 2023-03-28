using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  public class Like
  {
    public static Like Contains(string value)
    {
      return new Like(LikeKind.Contains, value);
    }

    public static Like StartsWith(string value)
    {
      return new Like(LikeKind.StartsWith, value);
    }

    public static Like EndsWith(string value)
    {
      return new Like(LikeKind.EndsWith, value);
    }

    public LikeKind Kind { get; }

    public string Value { get; }

    private Like(LikeKind kind, string value)
    {
      Kind = kind;
      Value = value;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      return
        obj is Like other &&
        Kind == other.Kind &&
        Value == other.Value;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((int)Kind * 397) ^ Value.GetHashCode();
      }
    }
  }
}
