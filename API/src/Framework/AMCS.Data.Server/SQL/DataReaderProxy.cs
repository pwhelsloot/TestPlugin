using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  public class DataReaderProxy : IDataReader
  {
    private bool disposed;

    public IDataReader Inner { get; }

    public int Depth => Inner.Depth;

    public bool IsClosed => Inner.IsClosed;

    public int RecordsAffected => Inner.RecordsAffected;

    public event EventHandler NextRow;
    public event EventHandler Disposed;

    public DataReaderProxy(IDataReader inner)
    {
      this.Inner = inner;
    }

    public string GetName(int i)
    {
      return Inner.GetName(i);
    }

    public string GetDataTypeName(int i)
    {
      return Inner.GetDataTypeName(i);
    }

    public Type GetFieldType(int i)
    {
      return Inner.GetFieldType(i);
    }

    public object GetValue(int i)
    {
      return Inner.GetValue(i);
    }

    public int GetValues(object[] values)
    {
      return Inner.GetValues(values);
    }

    public int GetOrdinal(string name)
    {
      return Inner.GetOrdinal(name);
    }

    public bool GetBoolean(int i)
    {
      return Inner.GetBoolean(i);
    }

    public byte GetByte(int i)
    {
      return Inner.GetByte(i);
    }

    public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
    {
      return Inner.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
    }

    public char GetChar(int i)
    {
      return Inner.GetChar(i);
    }

    public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
    {
      return Inner.GetChars(i, fieldoffset, buffer, bufferoffset, length);
    }

    public Guid GetGuid(int i)
    {
      return Inner.GetGuid(i);
    }

    public short GetInt16(int i)
    {
      return Inner.GetInt16(i);
    }

    public int GetInt32(int i)
    {
      return Inner.GetInt32(i);
    }

    public long GetInt64(int i)
    {
      return Inner.GetInt64(i);
    }

    public float GetFloat(int i)
    {
      return Inner.GetFloat(i);
    }

    public double GetDouble(int i)
    {
      return Inner.GetDouble(i);
    }

    public string GetString(int i)
    {
      return Inner.GetString(i);
    }

    public decimal GetDecimal(int i)
    {
      return Inner.GetDecimal(i);
    }

    public DateTime GetDateTime(int i)
    {
      return Inner.GetDateTime(i);
    }

    public IDataReader GetData(int i)
    {
      return Inner.GetData(i);
    }

    public bool IsDBNull(int i)
    {
      return Inner.IsDBNull(i);
    }

    public int FieldCount => Inner.FieldCount;

    public object this[int i] => Inner[i];

    public object this[string name] => Inner[name];

    public void Close()
    {
      Inner.Close();
    }

    public DataTable GetSchemaTable()
    {
      return Inner.GetSchemaTable();
    }

    public bool NextResult()
    {
      return Inner.NextResult();
    }

    public bool Read()
    {
      if (Inner.Read())
      {
        OnNextRow();
        return true;
      }

      return false;
    }

    protected virtual void OnNextRow() => NextRow?.Invoke(this, EventArgs.Empty);
    protected virtual void OnDisposed() => Disposed?.Invoke(this, EventArgs.Empty);

    public void Dispose()
    {
      if (!disposed)
      {
        Inner.Dispose();

        OnDisposed();

        disposed = true;
      }
    }
  }
}
