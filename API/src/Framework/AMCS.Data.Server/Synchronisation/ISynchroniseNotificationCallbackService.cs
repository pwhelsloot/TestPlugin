using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Synchronisation
{
  public interface ISynchroniseNotificationCallbackService
  {
    [OperationContract]
    void OnSynchroniseRecordInsertionRequested(string tableName, Guid recordGuid);

    [OperationContract]
    void OnSynchroniseRecordUpdateRequested(string tableName, Guid recordGuid);

    [OperationContract]
    void OnSynchroniseRecordDeletionRequested(string tableName, Guid recordGuid);

    [OperationContract]
    IList<string> OnGetTablesSynchronisedInRealtimeRequested();
  }
}
