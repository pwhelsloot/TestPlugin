using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  public static class SQLLiteralExpander
  {
    public static string Translate(string sql, IList<SqlParameter> parameters)
    {
      StringBuilder sb = null;
      List<int> seen = null;
      int offset = 0;

      while (offset < sql.Length)
      {
        int start = sql.IndexOf("{=", offset, StringComparison.Ordinal);
        if (start == -1)
          break;

        int end = sql.IndexOf('}', start + 2);
        if (end == -1)
          break;

        string parameterName = sql.Substring(start + 2, end - (start + 2)).Trim();

        // Parameter names in literal expansion should not start with an '@', but we do
        // allow for it. The parameter in the list however is required to start with an '@',
        // so we add it here if it's not in the parameter name to be able to find a match.

        if (!parameterName.StartsWith("@"))
          parameterName = "@" + parameterName;

        int index = FindParameter(parameters, parameterName);
        if (index == -1)
          throw new SQLLiteralExpanderException($"Cannot expand literal parameter '{parameterName}': parameter not found");

        var parameter = parameters[index];
        if (parameter.Direction != ParameterDirection.Input)
          throw new SQLLiteralExpanderException($"Cannot expand literal parameter '{parameterName}': parameter must have direction Input");

        if (sb == null)
          sb = new StringBuilder();
        if (seen == null)
          seen = new List<int>();

        if (!seen.Contains(index))
          seen.Add(index);

        sb.Append(sql, offset, start - offset);
        ExpandLiteral(sb, parameter.Value);

        offset = end + 1;
      }

      if (sb == null)
        return sql;

      sb.Append(sql, offset, sql.Length - offset);

      // We remove the processed parameters from the list of parameters to be
      // send to the server. A reason for this is that e.g. parameters of type
      // IList<int> will not be handled correctly.
      //
      // Note that this can cause issues when parameters are used both verbatim
      // and using literal expansion. Don't do that.

      seen.Sort();

      for (var i = seen.Count - 1; i >= 0; i--)
      {
        parameters.RemoveAt(seen[i]);
      }

      return sb.ToString();
    }

    private static void ExpandLiteral(StringBuilder sb, object value)
    {
      switch (value)
      {
        case null:
          // We expect the parameter list to be prepared already so that it does not
          // contain `null`'s anymore for literal values. It can still contain `null`'s
          // for structured parameters, but these aren't supported anyway.
          throw new SQLLiteralExpanderException("Value for literal expansion cannot be null");
        case DBNull _:
          sb.Append("null");
          return;
        case bool boolValue:
          if (boolValue)
            sb.Append('1');
          else
            sb.Append('0');
          return;
        case byte byteValue:
          sb.Append(byteValue.ToString(CultureInfo.InvariantCulture));
          return;
        case sbyte sbyteValue:
          sb.Append(sbyteValue.ToString(CultureInfo.InvariantCulture));
          return;
        case short shortValue:
          sb.Append(shortValue.ToString(CultureInfo.InvariantCulture));
          return;
        case ushort ushortValue:
          sb.Append(ushortValue.ToString(CultureInfo.InvariantCulture));
          return;
        case int intValue:
          sb.Append(intValue.ToString(CultureInfo.InvariantCulture));
          return;
        case uint uintValue:
          sb.Append(uintValue.ToString(CultureInfo.InvariantCulture));
          return;
        case long longValue:
          sb.Append(longValue.ToString(CultureInfo.InvariantCulture));
          return;
        case ulong ulongValue:
          sb.Append(ulongValue.ToString(CultureInfo.InvariantCulture));
          return;
        case float floatValue:
          sb.Append(floatValue.ToString(CultureInfo.InvariantCulture));
          return;
        case double doubleValue:
          sb.Append(doubleValue.ToString(CultureInfo.InvariantCulture));
          return;
        case decimal decimalValue:
          sb.Append(decimalValue.ToString(CultureInfo.InvariantCulture));
          return;
        case string stringValue:
          sb.Append('\'');
          foreach (char c in stringValue)
          {
            if (c == '\'')
              sb.Append("''");
            else
              sb.Append(c);
          }
          sb.Append('\'');
          return;
        case IEnumerable enumerable:
          if (value is string)
            break;

          sb.Append('(');
          bool hadOne = false;
          foreach (object entry in enumerable)
          {
            if (hadOne)
              sb.Append(", ");
            else
              hadOne = true;
            ExpandLiteral(sb, entry);
          }
          sb.Append(')');
          return;
      }

      throw new SQLLiteralExpanderException($"Cannot expand literal of type '{value.GetType().FullName}'");
    }

    private static int FindParameter(IList<SqlParameter> parameters, string parameterName)
    {
      for (int i = 0; i < parameters.Count; i++)
      {
        var parameter = parameters[i];
        if (string.Equals(parameter.ParameterName, parameterName, StringComparison.OrdinalIgnoreCase))
          return i;
      }

      return -1;
    }
  }
}
