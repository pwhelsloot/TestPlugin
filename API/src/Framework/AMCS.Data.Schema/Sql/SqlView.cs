using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AMCS.Data.Schema.Sql
{
  [Serializable]
  [DataContract]
  public class SqlView : SqlQueryableObject, IView
  {
    public SqlView(string name, IList<IColumn> columns)
      : base(name, columns)
    {
    }
  }
}
