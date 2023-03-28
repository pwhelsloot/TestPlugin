using System.Collections.Generic;

namespace AMCS.Data.Schema
{
  /// <summary>
  /// Move this into a lower level library if we ever work with other DB type, this isn't SQL Server specific
  /// </summary>
  public interface ITable: IQueryableObject
  {
  }
}
