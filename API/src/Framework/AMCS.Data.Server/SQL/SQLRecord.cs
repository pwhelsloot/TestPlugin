#pragma warning disable 0618

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Support;

namespace AMCS.Data.Server.SQL
{
  internal class SQLRecord : ISQLRecord
  {
    private readonly IDataReader reader;

    public int FieldCount => reader.FieldCount;

    public SQLRecord(IDataReader reader)
    {
      this.reader = reader;
    }

    public string GetName(int index)
    {
      return reader.GetName(index);
    }

    public object Get(string field)
    {
      object value = reader[field];
      if (value is DBNull)
        return null;
      return value;
    }

    public T Get<T>(string field)
    {
      var value = Get(field);
      if (value is T result)
        return result;
      return (T)ValueCoercion.Coerce(value, typeof(T));
    }

    public object Get(int index)
    {
      object value = reader[index];
      if (value is DBNull)
        return null;
      return value;
    }

    public T Get<T>(int index)
    {
      var value = Get(index);
      if (value is T result)
        return result;
      return (T)ValueCoercion.Coerce(value, typeof(T));
    }

    public T GetEntity<T>()
    {
      return GetEntity<T>(false);
    }

    public T GetEntity<T>(bool loadPropertyValues)
    {
      return (T)DataRowToObjectConvertor.ConvertDataRowToDataContractObject(reader, typeof(T), loadPropertyValues);
    }
  }
}
