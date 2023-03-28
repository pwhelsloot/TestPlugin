using System.Collections.Generic;

namespace AMCS.Data.Schema
{
  /// <summary>
  /// Move this into a lower level library if we ever work with other DB type, this isn't SQL Server specific
  /// </summary>
  public interface IColumn: IDatabaseObject
  {
    string ParentTableSchema { get; }
    string ParentTableName { get; }

    string Type { get; }
    string TypeAdditional { get; }
    bool IsNullable { get; }

    IList<IKey> Keys { get; }

    void AddKey(IKey key);
    void AddKeys(IList<IKey> keys);
  }
}
