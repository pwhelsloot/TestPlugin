namespace AMCS.Data.Server.Synchronisation
{
  using System;
  using System.Collections.Generic;
  using System.ServiceModel;
  using AMCS.Data.Server.Services;
  using AMCS.Data.Server.SQL;
  using Data.Util.Extension;

  public class SynchronisationRequestManager : TranslatableService
  {
    #region Strings

    public enum SyncStrings
    {
      [StringValue("Failed to discover tables that need synchronised. Please ensure the offline mode synchronisation service is running.")]
      FailedToDiscoverTables,

      [StringValue("Requested operation failed because insertion could not be synchronised to offline store. Please ensure the offline mode synchronisation service is running.")]
      FailedToSyncInsertion,

      [StringValue("Requested operation failed because update could not be synchronised to offline store. Please ensure the offline mode synchronisation service is running.")]
      FailedToSyncUpdate,

      [StringValue("Requested operation failed because deletion could not be synchronised to offline store. Please ensure the offline mode synchronisation service is running.")]
      FailedToSyncDeletion
    }

    #endregion Strings

    #region Properties/Variables

    private const string NotificationServiceName = "ISynchroniseNotificationService";
    private const string RequiredGuidFieldName = "GUID";

    private readonly IUserService userService;
    private readonly Dictionary<ISessionToken, IList<string>> authKeySyncTables = new Dictionary<ISessionToken, IList<string>>();

    #endregion Properties/Variables

    #region Singleton

    internal SynchronisationRequestManager(IUserService userService)
    {
      this.userService = userService;
    }

    #endregion Singleton

    #region Sync Methods

    public bool IsSynchronisationRequired(ISessionToken authKey, string tableName)
    {
#if NETFRAMEWORK
      if (!userService.IsSystemSessionToken(authKey))
      {
        if (authKey.IsOfflineModeEnabled)
        {
          if (!authKeySyncTables.ContainsKey(authKey))
            authKeySyncTables.Add(authKey, GetTablesSynchronisedInRealtime(authKey));

          IList<string> tables = authKeySyncTables[authKey];
          if (tables != null)
            return tables.Contains(tableName);
        }
      }
      return false;
#else
      return false;
#endif
    }

    private Guid GetRecordGuidValue(IDataSession dataSession, string tableName, string idFieldName, object idFieldValue)
    {
      string sql = string.Format("SELECT {0} FROM {1} WHERE {2} = @IdFieldValue", RequiredGuidFieldName, tableName, idFieldName);
      var ids = dataSession.Query(sql)
        .SetObject("@IdFieldValue", idFieldValue)
        .Execute()
        .List<Guid>();

      if (ids.Count == 0)
        throw new SynchronisationException(
          string.Format(
            "Could not determine {0} value for record in table '{1}' with '{2}' value of '{3}' and so could not synchronise.",
            RequiredGuidFieldName, 
            tableName, 
            idFieldName, 
            idFieldValue));

      // do a quick sense check before returning
      if (ids.Count > 1)
      {
        throw new SynchronisationException(
          string.Format(
            "Expected to find exactly 1 record in table '{1}' with '{2}' value of '{3}' but found more so could no synchornise.",
            RequiredGuidFieldName, 
            tableName, 
            idFieldName, 
            idFieldValue));
      }

      return ids[0];
    }

    public void SynchroniseRecordInsertion(IDataSession dataSession, ISessionToken authKey, string tableName, string idFieldName, object idFieldValue)
    {
#if NETFRAMEWORK
      if (!IsSynchronisationRequired(authKey, tableName))
        return;
      Guid recordGuid = GetRecordGuidValue(dataSession, tableName, idFieldName, idFieldValue);
      SynchroniseRecordInsertion(authKey, tableName, recordGuid);
#endif
    }

    public void SynchroniseRecordInsertion(ISessionToken authKey, string tableName, Guid recordGuid)
    {
#if NETFRAMEWORK
      if (!IsSynchronisationRequired(authKey, tableName))
        return;

      ISynchroniseNotificationService client = null;
      try
      {
        DuplexChannelFactory<ISynchroniseNotificationService> factory = new DuplexChannelFactory<ISynchroniseNotificationService>(new SynchroniseNotificationCallbackService(), NotificationServiceName);
        client = factory.CreateChannel();
        ((ICommunicationObject)client).Open();

        client.SynchroniseRecordInsertion(DataServices.Resolve<IUserService>().SerializeSessionToken(authKey), tableName, recordGuid);
      }
      catch (Exception ex)
      {
        if (client != null)
        {
          ((ICommunicationObject)client).Abort();
          client = null;
        }
        throw BslUserExceptionFactory<BslUserException>.CreateException(this.GetType(), typeof(SyncStrings), (int)SyncStrings.FailedToSyncInsertion, ex.Message);
      }
      finally
      {
        if (client != null)
          ((ICommunicationObject)client).Close();
      }
#endif
    }

    public void SynchroniseRecordUpdate(IDataSession dataSession, ISessionToken authKey, string tableName, string idFieldName, object idFieldValue)
    {
#if NETFRAMEWORK
      if (!IsSynchronisationRequired(authKey, tableName))
        return;
      Guid recordGuid = GetRecordGuidValue(dataSession, tableName, idFieldName, idFieldValue);
      SynchroniseRecordUpdate(authKey, tableName, recordGuid);
#endif
    }

    public void SynchroniseRecordUpdate(ISessionToken authKey, string tableName, Guid recordGuid)
    {
#if NETFRAMEWORK
      if (!IsSynchronisationRequired(authKey, tableName))
        return;

      ISynchroniseNotificationService client = null;
      try
      {
        DuplexChannelFactory<ISynchroniseNotificationService> factory = new DuplexChannelFactory<ISynchroniseNotificationService>(new SynchroniseNotificationCallbackService(), NotificationServiceName);
        client = factory.CreateChannel();
        ((ICommunicationObject)client).Open();

        client.SynchroniseRecordUpdate(DataServices.Resolve<IUserService>().SerializeSessionToken(authKey), tableName, recordGuid);
      }
      catch (Exception ex)
      {
        if (client != null)
        {
          ((ICommunicationObject)client).Abort();
          client = null;
        }
        throw BslUserExceptionFactory<BslUserException>.CreateException(this.GetType(), typeof(SyncStrings), (int)SyncStrings.FailedToSyncUpdate, ex.Message);
      }
      finally
      {
        if (client != null)
          ((ICommunicationObject)client).Close();
      }
#endif
    }

    public void SynchroniseRecordDeletion(IDataSession dataSession, ISessionToken authKey, string tableName, string idFieldName, object idFieldValue)
    {
#if NETFRAMEWORK
      if (!IsSynchronisationRequired(authKey, tableName))
        return;
      Guid recordGuid = GetRecordGuidValue(dataSession, tableName, idFieldName, idFieldValue);
      SynchroniseRecordDeletion(authKey, tableName, recordGuid);
#endif
    }

    public void SynchroniseRecordDeletion(ISessionToken authKey, string tableName, Guid recordGuid)
    {
#if NETFRAMEWORK
      if (!IsSynchronisationRequired(authKey, tableName))
        return;

      ISynchroniseNotificationService client = null;
      try
      {
        DuplexChannelFactory<ISynchroniseNotificationService> factory = new DuplexChannelFactory<ISynchroniseNotificationService>(new SynchroniseNotificationCallbackService(), NotificationServiceName);
        client = factory.CreateChannel();
        ((ICommunicationObject)client).Open();

        client.SynchroniseRecordDeletion(DataServices.Resolve<IUserService>().SerializeSessionToken(authKey), tableName, recordGuid);
      }
      catch (Exception ex)
      {
        if (client != null)
        {
          ((ICommunicationObject)client).Abort();
          client = null;
        }
        throw BslUserExceptionFactory<BslUserException>.CreateException(this.GetType(), typeof(SyncStrings), (int)SyncStrings.FailedToSyncDeletion, ex.Message);
      }
      finally
      {
        if (client != null)
          ((ICommunicationObject)client).Close();
      }
#endif
    }

    public IList<string> GetTablesSynchronisedInRealtime(ISessionToken authKey)
    {
#if NETFRAMEWORK
      ISynchroniseNotificationService client = null;
      try
      {
        DuplexChannelFactory<ISynchroniseNotificationService> factory = new DuplexChannelFactory<ISynchroniseNotificationService>(new SynchroniseNotificationCallbackService(), NotificationServiceName);
        client = factory.CreateChannel();
        ((ICommunicationObject)client).Open();

        return client.GetTablesSynchronisedInRealtime(DataServices.Resolve<IUserService>().SerializeSessionToken(authKey));
      }
      catch (Exception ex)
      {
        if (client != null)
        {
          ((ICommunicationObject)client).Abort();
          client = null;
        }
        throw BslUserExceptionFactory<BslUserException>.CreateException(this.GetType(), typeof(SyncStrings), (int)SyncStrings.FailedToDiscoverTables, ex.Message);
      }
      finally
      {
        if (client != null)
          ((ICommunicationObject)client).Close();
      }
#else
      return new List<string>();
#endif
    }

#endregion Sync Methods
  }
}