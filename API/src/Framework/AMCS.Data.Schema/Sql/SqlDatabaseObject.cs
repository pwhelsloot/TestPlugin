using System;
using System.Runtime.Serialization;

namespace AMCS.Data.Schema.Sql
{
  [Serializable]
  [DataContract]
  public class SqlDatabaseObject: IDatabaseObject
  {
    [DataMember]
    public string Name { get; private set; }

    public SqlDatabaseObject(string name)
    {
      if (name == null)
        throw new ArgumentNullException(nameof(name));
      Name = name;
    }
  }
}
