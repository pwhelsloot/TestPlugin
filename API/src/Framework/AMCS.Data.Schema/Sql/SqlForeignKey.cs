using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AMCS.Data.Schema.Sql
{
  [Serializable]
  [DataContract]
  public class SqlForeignKey: SqlKey, IForeignKey
  {
    [DataMember]
    public IKey ReferencedKey { get; private set; }

    public SqlForeignKey(string name, string tableSchema, string tableName, IList<string> columnNames, IKey referencedKey)
      : base(name, tableSchema, tableName, columnNames)
    {
      if (referencedKey == null)
        throw new ArgumentNullException(nameof(referencedKey));
      ReferencedKey = referencedKey;
    }
  }
}
