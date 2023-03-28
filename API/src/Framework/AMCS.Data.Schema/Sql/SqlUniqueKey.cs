using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AMCS.Data.Schema.Sql
{
  [Serializable]
  [DataContract]
  public class SqlUniqueKey: SqlKey, IUniqueKey
  {
    public SqlUniqueKey(string name, string tableSchema, string tableName, IList<string> columnNames) : base(name, tableSchema, tableName, columnNames) { }
  }
}
