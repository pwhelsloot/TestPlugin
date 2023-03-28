using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import
{
  internal static class MessageCollectionExtensions
  {
    public static void AddInfo(this MessageCollection self, string message, ImportAction action, MessageException exception = null)
    {
      self.AddInfo(message, action.Table.DataSet, action.Record, exception);
    }

    public static void AddWarn(this MessageCollection self, string message, ImportAction action, MessageException exception = null)
    {
      self.AddWarn(message, action.Table.DataSet, action.Record, exception);
    }

    public static void AddError(this MessageCollection self, string message, ImportAction action, MessageException exception = null)
    {
      self.AddError(message, action.Table.DataSet, action.Record, exception);
    }
  }
}
