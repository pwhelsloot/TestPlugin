using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EntityValidation.Syntax
{
  internal class SyntaxPrinter : ISyntaxVisitor
  {
    private readonly StringBuilder sb = new StringBuilder();
    private readonly bool indent;
    private int indentation;
    private bool hadIndent;

    public SyntaxPrinter(bool indent)
    {
      this.indent = indent;
    }

    private void Indent()
    {
      indentation++;
    }

    private void Unindent()
    {
      indentation--;
    }

    private void EnsureIndent()
    {
      if (!hadIndent)
      {
        hadIndent = true;

        if (indent)
        {
          for (int i = 0; i < indentation; i++)
          {
            sb.Append("  ");
          }
        }
      }
    }

    private void Write(string text)
    {
      EnsureIndent();
      sb.Append(text);
    }

    private void Write(string format, params object[] args)
    {
      EnsureIndent();
      sb.Append(String.Format(format, args));
    }

    private void WriteLine()
    {
      EnsureIndent();
      if (indent)
        sb.AppendLine();
      else
        sb.Append(' ');
      hadIndent = false;
    }

    private void WriteLine(string text)
    {
      Write(text);
      WriteLine();
    }

    private void WriteLine(string format, params object[] args)
    {
      Write(format, args);
      WriteLine();
    }

    public override string ToString()
    {
      return sb.ToString();
    }

    public void VisitArgumentList(ArgumentListSyntax node)
    {
      WriteLine("ArgumentList {");

      Indent();

      for (var i = 0; i < node.Arguments.Count; i++)
      {
        Write("[{0}] = ", i);
        node.Arguments[i].Accept(this);
        if (i == node.Arguments.Count - 1)
          WriteLine();
        else
          WriteLine(",");
      }

      Unindent();

      WriteLine("}");
    }

    public void VisitBinaryExpression(BinaryExpressionSyntax node)
    {
      WriteLine("BinaryExpression {");

      Indent();

      WriteLine("Type = {0},", node.Type);

      Write("Left = ");
      node.Left.Accept(this);
      WriteLine(",");

      Write("Right = ");
      node.Right.Accept(this);
      WriteLine();

      Unindent();

      Write("}");
    }

    public void VisitLiteralExpression(LiteralExpressionSyntax node)
    {
      WriteLine("LiteralExpression {");

      Indent();

      switch (node.Value)
      {
        case null:
          WriteLine("Value = null");
          break;
        case bool boolValue:
          WriteLine("Value = {0}", boolValue);
          break;
        case string stringValue:
          WriteLine("Value = \"{0}\"", stringValue);
          break;
        case UnparsedNumber number:
          WriteLine("Value = UnparsedNumber {");

          Indent();

          WriteLine("Text = \"{0}\",", number.Text);
          WriteLine("Type = {0},", number.Type);
          WriteLine("NumberStyles = {0}", number.NumberStyles);

          Unindent();

          WriteLine("}");
          break;
        default:
          throw new InvalidOperationException("Unknown constant value");
      }

      Unindent();

      Write("}");
    }

    public void VisitIdentifierName(IdentifierNameSyntax node)
    {
      WriteLine("IdentifierName {");

      Indent();

      WriteLine("Name = \"{0}\"", node.Name);

      Unindent();

      Write("}");
    }

    public void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
      WriteLine("MemberAccessExpression {");

      Indent();

      Write("Expression = ");
      node.Expression.Accept(this);
      WriteLine(",");

      Write("Name = ");
      node.Name.Accept(this);
      WriteLine();

      Unindent();

      Write("}");
    }

    public void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
      WriteLine("InvocationExpression {");

      Indent();

      Write("Expression = ");
      node.Expression.Accept(this);
      WriteLine(",");

      WriteLine("Arguments = {");

      Indent();

      node.Arguments.Accept(this);

      Unindent();

      WriteLine("}");

      Unindent();

      Write("}");
    }

    public void VisitUnaryExpression(UnaryExpressionSyntax node)
    {
      WriteLine("UnaryExpression {");

      Indent();

      Write("Expression = ");
      node.Expression.Accept(this);
      WriteLine(",");

      WriteLine("Type = {0}", node.Type);

      Unindent();

      Write("}");
    }
  }
}
