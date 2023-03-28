using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity.History;

namespace AMCS.Data.Server.SQL
{
  internal class SQLEntityHistoryChange
  {
    public string PropertyKey { get; set; }

    public object OldValue { get; set; }

    public object NewValue { get; set; }

    public static IList<EntityHistoryChange> Convert(IList<SQLEntityHistoryChange> changes)
    {
      IList<EntityHistoryChange> entityChanges = new List<EntityHistoryChange>();
      if (changes != null && changes.Count > 0)
      {
        foreach (SQLEntityHistoryChange change in changes)
        {
          entityChanges.Add(new EntityHistoryChange
          {
            PropertyKey = change.PropertyKey,
            OldValue = change.OldValue,
            NewValue = change.NewValue
          });
        }
      }
      return entityChanges;
    }
  }
}
