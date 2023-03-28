using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AMCS.Data.Schema.Sql
{
  [Serializable]
  [DataContract]
  public abstract class SqlQueryableObject : SqlDatabaseObject, IQueryableObject
  {
    [DataMember]
    public IList<IColumn> Columns { get; protected set; }

    public SqlQueryableObject(string name, IList<IColumn> columns)
      : base(name)
    {
      if (columns == null)
        throw new ArgumentNullException(nameof(columns));
      Columns = columns;
    }

    //  private string TryGetSchemaName()
    //  {
    //    if (!string.IsNullOrWhiteSpace(Name) && Name.Contains("."))
    //    {
    //      string[] tokens = Name.Split('.');
    //      if (tokens.Length == 3)
    //        return tokens[1]; //DB will be index 0
    //      else if (tokens.Length == 2)
    //        return tokens[0];
    //      else if (tokens.Length == 1) //just have table name
    //        return null;
    //      else
    //        throw new InvalidOperationException(string.Format("Table name '{0}' is not in a recognised format", Name));
    //    }
    //    return null;
    //  }

    //  private string GetTableNameWithoutSchema()
    //  {
    //    if (string.IsNullOrWhiteSpace(Name))
    //      throw new InvalidOperationException("Table name has not been set");

    //    if (Name.Contains("."))
    //    {
    //      string[] tokens = Name.Split('.');
    //      if (tokens.Length == 3)
    //        return tokens[2]; //DB will be index 0, Schema 1 and Table 2
    //      else if (tokens.Length == 2)
    //        return tokens[1];
    //      throw new InvalidOperationException(string.Format("Table name '{0}' is not in a recognised format", Name));
    //    }
    //    return Name;
    //  }
    //}
  }
}
