using System.Collections.Generic;
using AMCS.Data.Schema.Access;
using AMCS.Data.Schema.Sql;

namespace AMCS.Data.Schema.Discovery.Sql
{
  public class SqlStoredProcedureDiscoverer : IStoredProcedureDiscoverer
  {
    private IRoutineDiscoverer _routineDiscoverer;

    public SqlStoredProcedureDiscoverer(IDatabaseInterface dbInterface)
    {
      _routineDiscoverer = new SqlRoutineDiscoverer(dbInterface);
    }

    public IList<IStoredProcedure> Discover()
    {
      return ConvertRoutinesToStoredProcedures(_routineDiscoverer.Discover(RoutineType.StoredProcedure));
    }

    public IList<IStoredProcedure> Discover(string schema)
    {
      return ConvertRoutinesToStoredProcedures(_routineDiscoverer.Discover(schema, RoutineType.StoredProcedure));
    }

    public IStoredProcedure Discover(string schema, string name)
    {
      IRoutine routine = _routineDiscoverer.Discover(schema, RoutineType.StoredProcedure, name);
      if (routine == null)
        return null;
      return new SqlStoredProcedure(routine);
    }

    private IList<IStoredProcedure> ConvertRoutinesToStoredProcedures(IList<IRoutine> routines)
    {
      List<IStoredProcedure> result = new List<IStoredProcedure>();
      if (routines != null && routines.Count > 0)
      {
        foreach (IRoutine routine in routines)
          result.Add(new SqlStoredProcedure(routine));
      }
      return result;
    }
  }
}
