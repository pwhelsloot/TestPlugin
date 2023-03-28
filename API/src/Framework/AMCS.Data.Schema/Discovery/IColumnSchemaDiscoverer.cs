﻿using System.Collections.Generic;

namespace AMCS.Data.Schema.Discovery
{
  /// <summary>
  /// Move this into a lower level library if we ever work with other DB type, this isn't SQL Server specific
  /// </summary>
  public interface IColumnSchemaDiscoverer
  {
    IColumn Discover(string schema, string queryableObject, string column);
    IList<IColumn> Discover(string schema, string queryableObject);
  }
}
