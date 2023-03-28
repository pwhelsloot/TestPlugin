using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Rules
{
  internal abstract class Invocable
  {
    private static readonly Invocable[] AllInvocables = {
      new StringIsNullOrEmptyInvocable(),
      new StringIsNullOrWhitespaceInvocable(),
      new RegexIsMatchInvocable()
    };

    public static Invocable GetInvocable(string name, IList<Type> argumentTypes)
    {
      if (name == null)
        throw new ArgumentNullException(nameof(name));
      if (argumentTypes == null)
        throw new ArgumentNullException(nameof(argumentTypes));

      foreach (var invocable in AllInvocables)
      {
        if (invocable.IsMatch(name, argumentTypes))
          return invocable;
      }

      return null;
    }

    public abstract Type ReturnType { get; }

    protected abstract bool IsMatch(string name, IList<Type> argumentTypes);

    public abstract object GetValue(object[] arguments);

    private static bool MaybeType(Type actual, Type expected)
    {
      return actual == null || actual == expected;
    }

    private class StringIsNullOrEmptyInvocable : Invocable
    {
      public override Type ReturnType => typeof(bool);

      protected override bool IsMatch(string name, IList<Type> argumentTypes)
      {
        if (
          argumentTypes.Count != 1 ||
          !MaybeType(argumentTypes[0], typeof(string))
        )
          return false;

        switch (name)
        {
          case "string.IsNullOrEmpty":
          case "String.IsNullOrEmpty":
            return true;
          default:
            return false;
        }
      }

      public override object GetValue(object[] arguments)
      {
        return String.IsNullOrEmpty((string)arguments[0]);
      }
    }

    private class StringIsNullOrWhitespaceInvocable : Invocable
    {
      public override Type ReturnType => typeof(bool);

      protected override bool IsMatch(string name, IList<Type> argumentTypes)
      {
        if (
          argumentTypes.Count != 1 ||
          !MaybeType(argumentTypes[0], typeof(string))
        )
          return false;

        switch (name)
        {
          case "string.IsNullOrWhiteSpace":
          case "String.IsNullOrWhiteSpace":
            return true;
          default:
            return false;
        }
      }

      public override object GetValue(object[] arguments)
      {
        return String.IsNullOrWhiteSpace((string)arguments[0]);
      }
    }

    private class RegexIsMatchInvocable : Invocable
    {
      public override Type ReturnType => typeof(bool);

      protected override bool IsMatch(string name, IList<Type> argumentTypes)
      {
        if (
          argumentTypes.Count != 2 ||
          !MaybeType(argumentTypes[0], typeof(string)) ||
          !MaybeType(argumentTypes[1], typeof(string))
        )
          return false;

        switch (name)
        {
          case "Regex.IsMatch":
          case "System.Text.RegularExpressions.Regex.IsMatch":
            return true;
          default:
            return false;
        }
      }

      public override object GetValue(object[] arguments)
      {
        return Regex.IsMatch((string)arguments[0], (string)arguments[1]);
      }
    }
  }
}
