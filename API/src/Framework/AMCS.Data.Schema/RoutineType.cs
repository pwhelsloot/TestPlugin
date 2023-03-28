using System;

namespace AMCS.Data.Schema
{
  [Serializable]
  public enum RoutineType { StoredProcedure = 0, Function = 1, ScalarFunction = 2, TableValuedFunction = 3 }

  [Serializable]
  public enum FunctionType { ScalarFunction = 2, TableValuedFunction = 3 }
}
