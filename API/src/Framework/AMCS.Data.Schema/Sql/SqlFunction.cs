using System;
using System.Runtime.Serialization;

namespace AMCS.Data.Schema.Sql
{
  [Serializable]
  [DataContract]
  public class SqlFunction: SqlRoutine, IFunction
  {
    public SqlFunction(string name, string text) : base(RoutineType.Function, name, text) { }

    public SqlFunction(IRoutine initiliseWith) : this(initiliseWith.Name, initiliseWith.Text) { }
  }
}
