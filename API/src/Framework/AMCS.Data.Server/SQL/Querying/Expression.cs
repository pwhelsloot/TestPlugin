using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  public static class Expression
  {
    public static IExpression Eq(string field, object value)
    {
      return new FieldExpression(field, FieldComparison.Eq, value);
    }

    public static IExpression Ne(string field, object value)
    {
      return new FieldExpression(field, FieldComparison.Ne, value);
    }

    public static IExpression Gt(string field, object value)
    {
      return new FieldExpression(field, FieldComparison.Gt, value);
    }

    public static IExpression Ge(string field, object value)
    {
      return new FieldExpression(field, FieldComparison.Ge, value);
    }

    public static IExpression Lt(string field, object value)
    {
      return new FieldExpression(field, FieldComparison.Lt, value);
    }

    public static IExpression Le(string field, object value)
    {
      return new FieldExpression(field, FieldComparison.Le, value);
    }

    public static IExpression Null(string field)
    {
      return new FieldExpression(field, FieldComparison.Null, null);
    }

    public static IExpression NotNull(string field)
    {
      return new FieldExpression(field, FieldComparison.NotNull, null);
    }

    public static IExpression Like(string field, string pattern)
    {
      return new FieldExpression(field, FieldComparison.Like, pattern);
    }

    public static IExpression Like(string field, Like pattern)
    {
      return new FieldExpression(field, FieldComparison.Like, pattern);
    }

    public static IExpression In(string field, IEnumerable values)
    {
      return new FieldExpression(field, FieldComparison.In, values);
    }

    public static IExpression InSQL(string field, string sql, params object[] values)
    {
      return new FieldExpression(field, FieldComparison.InSQL, SQL(sql, values));
    }

    public static IExpression SQL(string sql, params object[] values)
    {
      return new SQLExpression(SQLFragment.Parse(sql), values);
    }

    public static IExpression Search(string search)
    {
      return new SearchExpression(search);
    }
  }
}
