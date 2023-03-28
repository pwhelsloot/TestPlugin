using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  public interface ISQLParameterizedExecutable<out T> : ISQLExecutable<T>
    where T : ISQLParameterizedExecutable<T>
  {
    T Arguments(object arguments);

    T Set(string parameterName, bool value);

    T Set(string parameterName, bool? value);

    T Set(string parameterName, int value);

    T Set(string parameterName, int? value);

    T Set(string parameterName, decimal value);

    T Set(string parameterName, decimal value, int precision, int scale);

    T Set(string parameterName, decimal? value);

    T Set(string parameterName, decimal? value, int precision, int scale);

    T Set(string parameterName, float value);

    T Set(string parameterName, float value, int precision, int scale);

    T Set(string parameterName, float? value);

    T Set(string parameterName, float? value, int precision, int scale);

    T Set(string parameterName, double value);

    T Set(string parameterName, double value, int precision, int scale);

    T Set(string parameterName, double? value);

    T Set(string parameterName, double? value, int precision, int scale);

    T Set(string parameterName, Guid value);

    T Set(string parameterName, Guid? value);

    T Set(string parameterName, DateTime value);

    T Set(string parameterName, DateTime? value);

    T SetDate(string parameterName, DateTime value);

    T SetDate(string parameterName, DateTime? value);

    T Set(string parameterName, DateTimeOffset value);

    T Set(string parameterName, DateTimeOffset? value);

    T Set(string parameterName, string value);

    T Set(string parameterName, byte[] value);

    T SetIdList(string parameterName, string listName, IEnumerable<int> values);

    T SetIdList(string parameterName, string listName, IEnumerable<int> values, string typeName);

    T SetIdList(string parameterName, string listName, IEnumerable<Guid> values);

    T SetIdList(string parameterName, string listName, IEnumerable<Guid> values, string typeName);

    T SetStringList(string parameterName, string listName, IEnumerable<string> values);

    T SetStringList(string parameterName, string listName, IEnumerable<string> values, string typeName);

    T SetIdDataTable(string parameterName, IEnumerable<int> values);

    T SetObject(string parameterName, object value);

    T SetObject(string parameterName, object value, SqlDbType type);

    T SetObject(string parameterName, object value, SqlDbType type, string typeName);

    T SetOutput(string parameterName, SqlDbType type);

    T SetOutput(string parameterName, SqlDbType type, int precision, int scale);

    T SetReturnValue(string parameterName, SqlDbType type);

    T SetReturnValue(string parameterName, SqlDbType type, int precision, int scale);
  }
}
