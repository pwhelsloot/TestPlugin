using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Synchronisation
{
  /// <summary>
  /// This class is only here because a the ISynchronisationNotificationService is a duplex service
  /// and in order to use it the callbacks need to be implemented.  ELEMOS is not being called back
  /// to - the synchronisation windows service is.
  /// </summary>
  public class SynchroniseNotificationCallbackService : ISynchroniseNotificationCallbackService
  {
    public IList<string> OnGetTablesSynchronisedInRealtimeRequested()
    {
      return null;
    }

    public void OnSynchroniseRecordDeletionRequested(string tableName, string idFieldName, object idFieldValue)
    {
    }

    public void OnSynchroniseRecordDeletionRequested(string tableName, Guid recordGuid)
    {
    }

    public void OnSynchroniseRecordInsertionRequested(string tableName, string idFieldName, object idFieldValue)
    {
    }

    public void OnSynchroniseRecordInsertionRequested(string tableName, Guid recordGuid)
    {
    }

    public void OnSynchroniseRecordUpdateRequested(string tableName, string idFieldName, object idFieldValue)
    {
    }

    public void OnSynchroniseRecordUpdateRequested(string tableName, Guid recordGuid)
    {
    }
  }
}