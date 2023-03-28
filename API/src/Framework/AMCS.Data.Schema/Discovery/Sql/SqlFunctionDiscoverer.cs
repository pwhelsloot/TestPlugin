using System.Collections.Generic;
using AMCS.Data.Schema.Access;
using AMCS.Data.Schema.Sql;

namespace AMCS.Data.Schema.Discovery.Sql
{
  public class SqlFunctionDiscoverer : IFunctionDiscoverer
  {
    private IRoutineDiscoverer _routineDiscoverer;

    public SqlFunctionDiscoverer(IDatabaseInterface dbInterface)
    {
      _routineDiscoverer = new SqlRoutineDiscoverer(dbInterface);
    }

    public IList<IFunction> Discover()
    {
      return ConvertRoutinesToFunctions(_routineDiscoverer.Discover(RoutineType.Function));
    }

    public IList<IFunction> Discover(FunctionType functionType)
    {
      RoutineType rt = (RoutineType)functionType;
      return ConvertRoutinesToFunctions(_routineDiscoverer.Discover(rt));
    }

    public IList<IFunction> Discover(string schema)
    {
      return ConvertRoutinesToFunctions(_routineDiscoverer.Discover(schema, RoutineType.Function));
    }

    public IList<IFunction> Discover(string schema, FunctionType functionType)
    {
      List<IFunction> result = new List<IFunction>();

      RoutineType rt = (RoutineType)functionType;
      IList<IRoutine> routines = _routineDiscoverer.Discover(schema, rt);
      if (routines != null)
      {
        foreach (IRoutine routine in routines)
          result.Add(new SqlFunction(routine));
      }
      return result;
    }

    public IFunction Discover(string schema, string name)
    {
      IRoutine routine = _routineDiscoverer.Discover(schema, RoutineType.Function, name);
      if (routine == null)
        return null;
      return new SqlFunction(routine);
    }

    private IList<IFunction> ConvertRoutinesToFunctions(IList<IRoutine> routines)
    {
      List<IFunction> result = new List<IFunction>();
      if (routines != null && routines.Count > 0)
      {
        foreach (IRoutine routine in routines)
          result.Add(new SqlFunction(routine));
      }
      return result;
    }
  }
}
