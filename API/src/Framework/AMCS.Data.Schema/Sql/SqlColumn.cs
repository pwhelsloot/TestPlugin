using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AMCS.Data.Schema.Sql
{
  [Serializable]
  [DataContract]
  public class SqlColumn : SqlDatabaseObject, IColumn
  {
    #region Properties/Variables

    public string Type { get { return SqlType.ToString(); } }

    [DataMember]
    public string ParentTableSchema { get; private set; }

    [DataMember]
    public string ParentTableName { get; private set; }

    [DataMember]
    public SqlDataType SqlType { get; private set; }

    [DataMember]
    public string TypeAdditional { get; private set; }

    [DataMember]
    public bool IsNullable { get; private set; }

    [DataMember]
    public IList<IKey> Keys { get; private set; }

    #endregion Properties/Variables

    #region ctors

    public SqlColumn(string parentTableSchema, string parentTableName, string name, SqlDataType type, bool isNullable, string typeAdditional = null, IList<IKey> keys = null)
      : base(name)
    {
      ParentTableSchema = parentTableSchema;
      ParentTableName = parentTableName;
      SqlType = type;
      TypeAdditional = typeAdditional;
      IsNullable = isNullable;
      Keys = keys;
    }

    public SqlColumn(string parentTableSchema, string parentTableName, string name, string type, bool isNullable, string typeAdditional = null, IList<IKey> keys = null)
      : this(parentTableSchema, parentTableName, name, ParseTypeString(type), isNullable, typeAdditional, keys)
    {
    }

    #endregion ctors

    #region Methods

    public void AddKey(IKey key)
    {
      if (key == null)
        throw new ArgumentNullException(nameof(key));
      if (Keys == null)
        Keys = new List<IKey>();
      Keys.Add(key);
    }

    public void AddKeys(IList<IKey> keys)
    {
      if (keys == null)
        throw new ArgumentNullException(nameof(keys));
      foreach (IKey key in keys)
        AddKey(key);
    }

    private static SqlDataType ParseTypeString(string type)
    {
      SqlDataType result = SqlDataType.Unknown;
      if (!Enum.TryParse<SqlDataType>(type, out result))
        throw new ArgumentException("Could not parse SQL data type name of '{0}'", type);
      return result;
    }

    #endregion Methods
  }
}
