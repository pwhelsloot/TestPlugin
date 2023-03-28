
using System.Collections.Generic;
namespace AMCS.Data.Schema.Discovery
{
  /// <summary>
  /// Move this into a lower level library if we ever work with other DB type, this isn't SQL Server specific
  /// </summary>
  public interface IRoutineDiscoverer
  {
    IList<IRoutine> Discover();
    IList<IRoutine> Discover(string schema);
    IList<IRoutine> Discover(string schema, RoutineType routineType);
    IList<IRoutine> Discover(RoutineType routineType);
    IRoutine Discover(string schema, RoutineType routineType, string name);
  }
}
