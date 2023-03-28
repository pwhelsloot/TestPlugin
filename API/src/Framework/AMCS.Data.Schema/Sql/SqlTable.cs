using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AMCS.Data.Schema.Sql
{
  [Serializable]
  [DataContract]
  public class SqlTable : SqlQueryableObject, ITable
  {
    public SqlTable(string name, IList<IColumn> columns)
      : base(name, columns)
    {
    }
  }
}
