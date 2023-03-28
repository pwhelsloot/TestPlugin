using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Syntax
{
  internal abstract class SyntaxNode
  {
    public abstract void Accept(ISyntaxVisitor visitor);
    public abstract T Accept<T>(ISyntaxVisitor<T> visitor);

    public override string ToString()
    {
      return ToString(false);
    }

    public string ToString(bool indent)
    {
      var visitor = new SyntaxPrinter(indent);
      Accept(visitor);
      return visitor.ToString();
    }
  }
}
