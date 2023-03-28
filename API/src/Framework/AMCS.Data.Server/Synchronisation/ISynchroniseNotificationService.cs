using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Synchronisation
{
  [ServiceKnownType(typeof(Guid))]
  [ServiceContract(Namespace = "http://www.amcsgroup.com/Sync", SessionMode = SessionMode.Required, CallbackContract = typeof(ISynchroniseNotificationCallbackService))]
  public interface ISynchroniseNotificationService
  {
    [OperationContract]
    void Register(string authKey);

    /// <summary>
    /// Just used to check if the service can be connected to.
    /// </summary>
    [OperationContract]
    void Ping();

    [OperationContract]
    void SynchroniseRecordInsertion(string authKey, string tableName, Guid recordGuid);

    [OperationContract]
    void SynchroniseRecordUpdate(string authKey, string tableName, Guid recordGuid);

    [OperationContract]
    void SynchroniseRecordDeletion(string authKey, string tableName, Guid recordGuid);

    [OperationContract]
    IList<string> GetTablesSynchronisedInRealtime(string authKey);
  }
}
