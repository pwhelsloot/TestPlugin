using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.EntityValidation.Parser;
using AMCS.Data.EntityValidation.Syntax;

namespace AMCS.Data.EntityValidation.Rules
{
  public class Validation
  {
    public static Validation Parse(Type targetType, string expression)
    {
      if (targetType == null)
        throw new ArgumentNullException(nameof(targetType));
      if (expression == null)
        throw new ArgumentNullException(nameof(expression));

      var syntax = ValidationParser.Parse(expression);
      syntax = syntax.Accept(new SimplificationRewriter());

      var validationExpression = syntax.Accept(new ValidationVisitor(targetType));

      if (validationExpression.Type != typeof(bool))
        throw new ParseException("Expected expression to resolve to a boolean");

      return new Validation(validationExpression);
    }

    private readonly IExpression expression;

    private Validation(IExpression expression)
    {
      this.expression = expression;
    }

    public bool IsValid(object target)
    {
      return !(bool)expression.GetValue(target);
    }
  }
}
