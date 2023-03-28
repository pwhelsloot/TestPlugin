using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Support;

namespace AMCS.Data.Server.SQL
{
  internal class SQLCommandParameterBuilder
  {
    private static readonly ConcurrentDictionary<Type, SQLCommandParameterBuilder> Builders = new ConcurrentDictionary<Type, SQLCommandParameterBuilder>();
    private static readonly Func<Type, SQLCommandParameterBuilder> BuildDelegate = Build;

    public static SQLCommandParameterBuilder ForType(Type argumentsType)
    {
      return Builders.GetOrAdd(argumentsType, BuildDelegate);
    }

    private static SQLCommandParameterBuilder Build(Type argumentsType)
    {
      var parameters = new List<Parameter>();

      foreach (var property in argumentsType.GetProperties())
      {
        parameters.Add(new Parameter(
          ReflectionHelper.GetPropertyGetter(property),
          "@" + property.Name,
          GetDbType(property.PropertyType)));
      }

      return new SQLCommandParameterBuilder(parameters.ToArray());
    }

    private static SqlDbType? GetDbType(Type type)
    {
      // Add type overrides here. By default parameters are added
      // using AddWithValue which doesn't always get it right. If this
      // method returns a se
      return null;
    }

    private readonly Parameter[] parameters;

    private SQLCommandParameterBuilder(Parameter[] parameters)
    {
      this.parameters = parameters;
    }

    public void Build(IList<SqlParameter> sqlParameters, object arguments)
    {
      foreach (var parameter in parameters)
      {
        sqlParameters.Add(parameter.CreateParameter(arguments));
      }
    }

    private class Parameter
    {
      private readonly Func<object, object> getter;
      private readonly string name;
      private readonly SqlDbType? type;

      public Parameter(Func<object, object> getter, string name, SqlDbType? type)
      {
        this.getter = getter;
        this.name = name;
        this.type = type;
      }

      public SqlParameter CreateParameter(object arguments)
      {
        var value = getter(arguments) ?? DBNull.Value;

        if (type.HasValue)
          return new SqlParameter(name, type.Value) { Value = value };

        return new SqlParameter(name, value);
      }
    }
  }
}
