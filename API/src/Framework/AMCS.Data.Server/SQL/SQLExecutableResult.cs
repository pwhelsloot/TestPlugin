using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  internal class SQLExecutableResult : ISQLExecutableResult
  {
    public static readonly SQLExecutableResult Empty = new SQLExecutableResult(null, 0, null);

    public static SQLExecutableResult FromCommand(SqlCommand command, int rowsAffected)
    {
      Dictionary<string, object> parameters = null;

      foreach (SqlParameter parameter in command.Parameters)
      {
        if (parameter.Direction != ParameterDirection.Input)
        {
          object value = parameter.Value;
          if (value is DBNull)
            value = null;
          if (parameters == null)
            parameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
          parameters.Add(parameter.ParameterName, value);
        }
      }

      if (parameters == null && rowsAffected == 0)
        return Empty;

      return new SQLExecutableResult(parameters, rowsAffected, command);
    }

    public static SQLExecutableResult GetCommand(SqlCommand command)
    {
      var parameters = new Dictionary<string, object>();
      foreach (SqlParameter parameter in command.Parameters)
      {
        parameters.Add(parameter.ParameterName, parameter.Value);
      }
      return new SQLExecutableResult(parameters, 0, command);
    }

    private readonly Dictionary<string, object> parameters;

    public int RowsAffected { get; }

    public SqlCommand Command { get; }

    private SQLExecutableResult(Dictionary<string, object> parameters, int rowsAffected, SqlCommand command)
    {
      this.parameters = parameters;
      RowsAffected = rowsAffected;
      Command = command;
    }

    public object Get(string parameterName)
    {
      if (parameters == null || !parameters.TryGetValue(parameterName, out var value))
        throw new ArgumentOutOfRangeException($"No output or return parameter by name '{parameterName}' found");

      return value;
    }

    public T Get<T>(string parameterName)
    {
      return (T)Get(parameterName);
    }
  }
}
