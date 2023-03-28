using System;
using System.Runtime.Serialization;

namespace AMCS.Data.Schema.Sql
{
  [Serializable]
  [DataContract]
  public class SqlConstraint : SqlDatabaseObject, IConstraint
  {
    public SqlConstraint(string name) : base(name) { }
  }
}
