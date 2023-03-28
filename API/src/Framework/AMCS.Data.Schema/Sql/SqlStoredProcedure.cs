using System;
using System.Runtime.Serialization;

namespace AMCS.Data.Schema.Sql
{
  [Serializable]
  [DataContract]
  public class SqlStoredProcedure : SqlRoutine, IStoredProcedure
  {
    public SqlStoredProcedure(string name, string text) : base(RoutineType.StoredProcedure, name, text) { }

    public SqlStoredProcedure(IRoutine initiliseWith) : this(initiliseWith.Name, initiliseWith.Text) { }
  }
}
