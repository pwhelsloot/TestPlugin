using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Rules
{
  internal abstract class MemberAccess
  {
    private static readonly MemberAccess[] AllAccessors = {
      new StringLengthAccess()
    };

    public static MemberAccess GetMemberAccess(Type targetType, string member)
    {
      if (targetType == null)
        throw new ArgumentNullException(nameof(targetType));
      if (member == null)
        throw new ArgumentNullException(nameof(member));

      foreach (var accessor in AllAccessors)
      {
        if (accessor.IsMatch(targetType, member))
          return accessor;
      }

      return null;
    }

    public abstract Type ReturnType { get; }

    protected abstract bool IsMatch(Type targetType, string member);

    public abstract object GetValue(object target);

    private class StringLengthAccess : MemberAccess
    {
      public override Type ReturnType => typeof(int);

      protected override bool IsMatch(Type targetType, string member)
      {
        return targetType == typeof(string) && member == "Length";
      }

      public override object GetValue(object target)
      {
        return ((string)target).Length;
      }
    }
  }
}
