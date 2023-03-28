using System;
using System.Runtime.Serialization;

namespace AMCS.Data.Schema.Sql
{
  [Serializable]
  [DataContract]
  public class SqlRoutine : SqlDatabaseObject, IRoutine
  {
    [DataMember]
    public RoutineType Type { get; private set; }

    [DataMember]
    public string Text { get; private set; }

    public SqlRoutine(RoutineType type, string name, string text)
      : base(name)
    {
      if (text == null)
        throw new ArgumentNullException(nameof(text));
      Type = type;
      Text = text;
    }
  }
}
