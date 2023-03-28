using System;
using FastMember;

namespace AMCS.Data.EntityValidation.Rules
{
  internal class IdentifierExpression : IExpression
  {
    private readonly TypeAccessor typeAccessor;
    private readonly string memberName;

    public Type Type { get; }

    public IdentifierExpression(TypeAccessor typeAccessor, string memberName, Type type)
    {
      if (typeAccessor == null)
        throw new ArgumentNullException(nameof(typeAccessor));
      if (memberName == null)
        throw new ArgumentNullException(nameof(memberName));
      if (type == null)
        throw new ArgumentNullException(nameof(type));

      this.typeAccessor = typeAccessor;
      this.memberName = memberName;
      Type = type;
    }

    public object GetValue(object target)
    {
      return typeAccessor[target, memberName];
    }
  }
}
