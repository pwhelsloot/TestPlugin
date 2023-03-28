using System.Collections.Generic;

namespace AMCS.Data.Schema.Discovery
{
  /// <summary>
  /// Move this into a lower level library if we ever work with other DB type, this isn't SQL Server specific
  /// </summary>
  public interface IStoredProcedureDiscoverer
  {
    IList<IStoredProcedure> Discover();
    IList<IStoredProcedure> Discover(string schema);
    IStoredProcedure Discover(string schema, string name);
  }
}
