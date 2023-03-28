using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AMCS.Data.Schema.Sql
{
  [Serializable]
  [DataContract]
  public class SqlKey: SqlDatabaseObject, IKey
  {
    [DataMember]
    public string TableSchema { get; private set; }

    [DataMember]
    public string TableName { get; private set; }

    [DataMember]
    public IList<string> ColumnNames { get; private set; }

    public SqlKey(string name, string tableSchema, string tableName, IList<string> columnNames)
      : base(name)
    {
      if (tableSchema == null)
        throw new ArgumentNullException(nameof(tableSchema));
      if (tableName == null)
        throw new ArgumentNullException(nameof(tableName));
      if (columnNames == null)
        throw new ArgumentNullException(nameof(columnNames));
      if (columnNames.Count == 0)
        throw new ArgumentException("Columns count must be positive");

      TableSchema = tableSchema;
      TableName = tableName;
      ColumnNames = columnNames;
    }
  }
}
