using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AMCS.AzureServiceBusSupport;
using AMCS.Data.Configuration;
using AMCS.Data.Entity;
using AMCS.Data.Server.Broadcast;
using AMCS.Data.Server.BslTriggers.Data;
using AMCS.Data.Server.Services;
using AMCS.JobSystem;
using AMCS.JobSystem.Scheduler.Api;
using AMCS.AzureServiceBusSupport.RetryUtils;
using log4net;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Newtonsoft.Json;

namespace AMCS.Data.Server.BslTriggers
{
  internal class BslTriggerManager : IBslTriggerManager
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(BslTriggerManager));

    private static readonly RetryPolicy TriggerSaveRetryPolicy = RetryPolicy.Handle<Exception>()
      .MaxRetries(5)
      .Backoff(new BackoffProfile(TimeSpan.FromSeconds(0.1), 0, TimeSpan.FromSeconds(1)));

    private readonly ThreadLocal<int> recursion = new ThreadLocal<int>();

    private readonly ThreadLocal<bool> executingInJobSystem = new ThreadLocal<bool>();

    private volatile BslTriggerState bslTriggerState;

    private readonly object syncRoot = new object();
    private readonly SchedulerClient client;
    private readonly string defaultQueue;
    private readonly BslTriggerActionResolver actionResolver;
    private readonly BslTriggerEnabledEntityResolver enabledEntityResolver;

    private readonly TelemetryClient telemetryClient;

    public BslTriggerManager(SchedulerClient client, IBroadcastService broadcastService, string defaultQueue, TypeManager entityTypes, TypeManager actionTypes, ISetupService setupService, IDataEventsBuilderService dataEventsBuilderService)
    {
      this.client = client;

      this.defaultQueue = defaultQueue;
      actionResolver = new BslTriggerActionResolver(actionTypes);
      enabledEntityResolver = new BslTriggerEnabledEntityResolver(entityTypes);
      dataEventsBuilderService.Add(new BslTriggerEvents(this));

      broadcastService.On<BslTriggerChanged>(p =>
      {
        var userId = DataServices.Resolve<IUserService>().SystemUserSessionKey;

        using (var dataSession = BslDataSessionFactory.GetDataSession(userId))
        using (var transaction = dataSession.CreateTransaction())
        {
          lock (syncRoot)
          {
            var bslTriggerState = this.bslTriggerState.Copy();
            bslTriggerState.UpdateTriggers(p.TriggerId, dataSession.FindById<BslTriggerEntity>(userId, p.TriggerId));

            this.bslTriggerState = bslTriggerState;

            broadcastService.Broadcast(new AvailableBslTriggersUpdated(p.TriggerId));
          }

          transaction.Commit();
        }
      });

      setupService.RegisterSetup(LoadInitialBslTriggers, -1000);

#if NETFRAMEWORK
      telemetryClient = new TelemetryClient();
#else
      var config = TelemetryConfiguration.CreateDefault();
      telemetryClient = new TelemetryClient(config);
#endif

    }

    private void LoadInitialBslTriggers()
    {
      var userId = DataServices.Resolve<IUserService>().SystemUserSessionKey;

      using (var dataSession = BslDataSessionFactory.GetDataSession(userId))
      using (var transaction = dataSession.CreateTransaction())
      {
        bslTriggerState = new BslTriggerState();

        foreach (var trigger in dataSession.GetAll<BslTriggerEntity>(userId, false))
        {
          bslTriggerState.UpdateTriggers(trigger.Id32, trigger);
        }

        transaction.Commit();
      }
    }

    public void ConfigureBslTriggerSet(IDataSession dataSession, ISessionToken userId, string category, 
      IList<BslTriggerEntity> triggerSet)
    {
      foreach (var trigger in triggerSet)
      {
        trigger.SystemCategory = category;
      }

      var existingItems = DataServices.Resolve<IBslTriggerService>()
        .GetAllBySystemCategory(dataSession, userId, category);

      foreach (var trigger in triggerSet)
      {
        trigger.BslTriggerId = existingItems.FirstOrDefault(existingItem =>
          existingItem.TriggerEntity == trigger.TriggerEntity && existingItem.Action == trigger.Action)?.BslTriggerId;
      }

      var bslTriggerCacheService = DataServices.Resolve<ICacheCoherentEntityService<BslTriggerEntity>>();

      TriggerSaveRetryPolicy.Retry(() =>
      {
        bslTriggerCacheService.Publish(triggerSet, category, userId, dataSession);
      });
    }

    public void RaiseEntity(IDataSession session, ISessionToken userId, Type entityType, BslAction bslAction, int id, Guid? guid, EntityObject entityObject)
    {
      // creating local copy of field because this.bslTriggerState can be updated in a different thread
      // we want to work with the current instance for the entirety of this method
      var bslTriggerState = this.bslTriggerState;

      for (; entityType != null; entityType = entityType.BaseType)
      {
        if (!bslTriggerState.TriggersByTypeAction.TryGetValue((entityType.FullName, bslAction), out List<BslTriggerEntity> bslTriggers))
            continue;

        foreach (var bslTrigger in bslTriggers)
        {
          var bslTriggerRequest = new BslTriggerRequest
          {
            Action = bslAction,
            EntityType = entityType.AssemblyQualifiedName,
            EntityObject = entityObject,
            Id = id,
            GUID = guid,
            TransactionId = session.IsTransaction()
              ? session.GetTransactionId()
              : (Guid?)null
          };

          // Justification for doing this is that before "Before*" events, we didn't care about the current session because the
          // transaction was already committed by the time this was executed. With "Before*" events, we could potentially need
          // to preform actions within the current transaction. There might be better spots for passing the session, but this is
          // the most convenient as it won't break existing infrastructure
          if (bslAction == BslAction.BeforeCreate || bslAction == BslAction.BeforeUpdate || bslAction == BslAction.BeforeDelete)
            bslTriggerRequest.DataSession = session;

          var pendingTrigger = new BslPendingTrigger(bslTrigger.Id32, bslTriggerRequest);
          
          if (!session.Context.TryGetValue(typeof(BslActionCollection).FullName, out var bslActionCollection))
          {
            SetupBslActionCollection(session, userId, bslAction, bslTrigger, pendingTrigger);
          }
          else
          {
            SetPendingTrigger((BslActionCollection)bslActionCollection, bslTrigger, pendingTrigger, executingInJobSystem.Value);
          }
        }
      }
    }

    private void SetupBslActionCollection(IDataSession session, ISessionToken userId, BslAction bslAction, BslTriggerEntity bslTriggerEntity, BslPendingTrigger pendingTrigger)
    {
      var bslActionCollection = new BslActionCollection();
      session.Context.Add(typeof(BslActionCollection).FullName, bslActionCollection);

      var isPreCommitAction = bslAction == BslAction.BeforeCreate || bslAction == BslAction.BeforeDelete ||
                              bslAction == BslAction.BeforeUpdate;

      if (session.IsTransaction() && isPreCommitAction)
      {
        // Because we want "before commit" actions to execute immediately on setting the event, we need to set the pending trigger
        // before we set the callback
        bslActionCollection.PendingTriggers.Add(pendingTrigger);
        session.BeforeCommit(dataSession => bslActionCollection.Raise(dataSession, userId));

        return;
      }

      if (session.IsTransaction())
      {
        session.AfterCommit(dataSession => bslActionCollection.Raise(dataSession, userId));
      }
      else
      {
        session.AfterDispose(dataSession => bslActionCollection.Raise(dataSession, userId));
      }

      SetPendingTrigger(bslActionCollection, bslTriggerEntity, pendingTrigger, executingInJobSystem.Value);
    }

    private void SetPendingTrigger(BslActionCollection collection, BslTriggerEntity bslTriggerEntity, BslPendingTrigger pendingTrigger, bool executingInJobSystem)
    {
      if (bslTriggerEntity.UseJobSystem && !executingInJobSystem)
      {
        collection.PendingJobSystemTriggers.Add(pendingTrigger);
      }
      else
      {
        collection.PendingTriggers.Add(pendingTrigger);
      }
    }

    public void Raise(ISessionToken userId, IList<BslPendingTrigger> bslPendingTriggers)
    {
      RaiseBslTriggers(userId, bslPendingTriggers);
    }

    public void RaiseJobSystemJob(ISessionToken userId, IList<BslPendingTrigger> bslPendingTriggers)
    {
      // Before sending to the job system, we calculate the job size. If it's too big, we send the request to blob storage
      // and send the blob storage key to the job system.
      // It's done using a stream as below because we want to break immediately once the size of the job goes over the limits.

      if (client == null)
        throw new InvalidOperationException("JobSystem not configured");

      var builder = new JobBuilder()
        .Queue(defaultQueue)
        .PersistenceMode(PersistenceMode.Transient)
        .Handler(typeof(BslTriggerJobHandler));

      int bytes = 0;

      try
      {
        using (var counter = new CountingWriteStream(p =>
        {
          bytes += p;
          if (bytes + Constants.MessageSizeOverhead > Constants.MaxBatchSize)
          {
            throw new JobTooBigException();
          }
        }))
        {
          using (var writer = new StreamWriter(counter))
          using (var json = new JsonTextWriter(writer))
          {
            JsonSerializer.Create().Serialize(json, bslPendingTriggers);
          }

          builder
            .Parameters(new BslTriggerJobRequest(bslPendingTriggers, null));
        }
      }
      catch (JobTooBigException)
      {
        var key = DataServices.Resolve<ITempFileService>().WriteJson(bslPendingTriggers);
        builder
          .Parameters(new BslTriggerJobRequest(null, key));
      }

      client.Post(JobHandler.GetJobUserId(userId), builder.Build());
    }

    public void ExecuteBslTriggersFromJobSystem(ISessionToken userId, BslTriggerJobRequest bslTriggerJobRequest)
    {

      try
      {
        Debug.Assert(!executingInJobSystem.Value);

        executingInJobSystem.Value = true;

        var bslPendingTriggers = bslTriggerJobRequest.BslPendingTriggers
          ?? DataServices.Resolve<ITempFileService>().ReadJson<IList<BslPendingTrigger>>(bslTriggerJobRequest.BslTriggerJobKey);

        RaiseBslTriggers(userId, bslPendingTriggers);
      }
      catch (Exception ex)
      {
        Logger.Error("Error executing BslTrigger from JobSystem", ex);
      }
      finally
      {
        executingInJobSystem.Value = false;
      }
    }

    private void RaiseBslTriggers(ISessionToken userId, IList<BslPendingTrigger> bslPendingTriggers)
    {
      // Populate any BSL trigger ID's that are missing, that do have a GUID.
      // We delay create the session and transaction to limit the performance
      // impact.

      IDataSession session = null;
      IDataSessionTransaction transaction = null;
      IDataAccessIdService idService = null;

      try
      {
        foreach (var bslPendingTrigger in bslPendingTriggers)
        {
          if (bslPendingTrigger.Request.Id != 0)
            continue;

          if (bslPendingTrigger.Request.GUID.HasValue)
          {
            if (session == null)
            {
              session = BslDataSessionFactory.GetDataSession();
              transaction = session.CreateTransaction();
              idService = DataServices.Resolve<IDataAccessIdService>();
            }

            var entityType = Type.GetType(bslPendingTrigger.Request.EntityType, false);

            if (entityType != null)
            {
              int? id = null;

              try
              {
                id = idService.GetIdByGuid(session, entityType, bslPendingTrigger.Request.GUID.Value);
              }
              catch
              {
                // Ignore
              }

              if (id.HasValue)
                bslPendingTrigger.Request.Id = id.Value;
              else
                Logger.WarnFormat("Could not resolve ID of GUID '{0}' for entity object '{1}'", bslPendingTrigger.Request.GUID, bslPendingTrigger.Request.EntityType);
            }
            else
            {
              Logger.WarnFormat("Could not resolve type for entity '{0}'", bslPendingTrigger.Request.EntityType);
            }
          }
          else
          {
            Logger.WarnFormat("Pending BSL trigger for entity '{0}' has neither an ID or GUID", bslPendingTrigger.Request.EntityType);
          }
        }

        transaction?.Commit();
      }
      finally
      {
        if (session != null)
        {
          using (session)
          {
            transaction?.Dispose();
          }
        }
      }

      IBslActionContext bslActionContext = new BslActionContext();

      foreach (var bslPendingTrigger in bslPendingTriggers)
      {
        ExecuteBslTrigger(userId, bslPendingTrigger, bslActionContext);
      }
    }

    private void ExecuteBslTrigger(ISessionToken userId, BslPendingTrigger bslPendingTrigger, IBslActionContext bslActionContext)
    {
      using (var operation = TrackRequest(bslPendingTrigger.Request))
      {
        try
        {
          if (recursion.Value > 10)
            throw new MaxRecursionLevelReachedException($"Maximum recursion level ({ recursion }) reached while executing BslTriggers");

          recursion.Value++;

          var bslTriggerState = this.bslTriggerState;

          if (!bslTriggerState.TriggersById.TryGetValue(bslPendingTrigger.TriggerId, out BslTriggerEntity bslTrigger))
            return;

          var bslActionCaller = actionResolver.GetType(bslTrigger.ActionGuid);

          if (operation != null)
            operation.Telemetry.Name = "BSL " + bslActionCaller.ActionType;

          bslActionCaller.Execute(userId, bslPendingTrigger.Request, bslTrigger.ActionConfiguration, bslActionContext);
        }
        catch (Exception ex)
        {
          if (operation != null)
          {
            operation.Telemetry.Success = false;
            telemetryClient.TrackException(ex);
          }

          Logger.Error($"An error occurred executing Bsl Trigger with Id {bslPendingTrigger.TriggerId}", ex);

          var isPreCommitAction = bslPendingTrigger.Request.Action == BslAction.BeforeCreate
                                  || bslPendingTrigger.Request.Action == BslAction.BeforeUpdate
                                  || bslPendingTrigger.Request.Action == BslAction.BeforeDelete;

          if (ex is MaxRecursionLevelReachedException || isPreCommitAction)
            throw;
        }
        finally
        {
          recursion.Value--;
        }
      }
    }
        
    private IOperationHolder<RequestTelemetry> TrackRequest(BslTriggerRequest request)
    {
#if NETFRAMEWORK
      if (TelemetryConfiguration.Active.DisableTelemetry)
#else
      if (telemetryClient.TelemetryConfiguration.DisableTelemetry)
#endif
      return null;

      var requestTelemetry = new RequestTelemetry
      {
        Name = "BSL",
        Properties =
          {
            { "BSL entity type", request.EntityType.Substring(0, request.EntityType.IndexOf(',')) },
            { "BSL trigger request ID", request.Id.ToString() },
            { "BSL trigger request action", request.Action.ToString() }
          }
      };

      return telemetryClient.StartOperation(requestTelemetry);
    }

    public IList<BslTriggerActionEntity> GetBslTriggerActions()
    {
      return actionResolver.GetBslTriggerActions();
    }

    public IList<BslTriggerEnabledEntity> GetBslTriggerEnabledEntities()
    {
      return enabledEntityResolver.GetBslTriggerEnabledEntities();
    }

    private class BslTriggerState
    {
      public Dictionary<int, BslTriggerEntity> TriggersById { get; }
      public Dictionary<(string, BslAction), List<BslTriggerEntity>> TriggersByTypeAction { get; }

      public BslTriggerState()
      {
        TriggersById = new Dictionary<int, BslTriggerEntity>();
        TriggersByTypeAction = new Dictionary<(string, BslAction), List<BslTriggerEntity>>();
      }

      private BslTriggerState(Dictionary<int, BslTriggerEntity> triggersById, Dictionary<(string, BslAction), List<BslTriggerEntity>> triggersByTypeAction)
      {
        TriggersById = triggersById;
        TriggersByTypeAction = triggersByTypeAction;
      }

      public void UpdateTriggers(int triggerId, BslTriggerEntity trigger)
      {
        if (trigger == null)
        {
          if (TriggersById.TryGetValue(triggerId, out trigger))
          {
            RemoveExistingTriggers(trigger);
            TriggersById.Remove(triggerId);
          }
        }
        else
        {
          if (TriggersById.TryGetValue(triggerId, out var existingTrigger))
          {
            RemoveExistingTriggers(existingTrigger);
          }

          TriggersById[triggerId] = trigger;

          if (trigger.TriggerOnCreate)
          {
            AddTrigger(trigger, BslAction.Create);
          }

          if (trigger.TriggerOnUpdate)
          {
            AddTrigger(trigger, BslAction.Update);
          }

          if (trigger.TriggerOnDelete)
          {
            AddTrigger(trigger, BslAction.Delete);
          }

          if (trigger.TriggerBeforeCreate)
          {
            AddTrigger(trigger, BslAction.BeforeCreate);
          }

          if (trigger.TriggerBeforeUpdate)
          {
            AddTrigger(trigger, BslAction.BeforeUpdate);
          }

          if (trigger.TriggerBeforeDelete)
          {
            AddTrigger(trigger, BslAction.BeforeDelete);
          }
        }
      }

      private void AddTrigger(BslTriggerEntity trigger, BslAction bslActionFlag)
      {
        if (!TriggersByTypeAction.TryGetValue((trigger.TriggerEntity, bslActionFlag), out var triggers))
        {
          triggers = new List<BslTriggerEntity>();
          TriggersByTypeAction.Add((trigger.TriggerEntity, bslActionFlag), triggers);
        }

        triggers.Add(trigger);
      }

      private void RemoveExistingTriggers(BslTriggerEntity trigger)
      {
        RemoveTrigger(trigger, BslAction.Create);
        RemoveTrigger(trigger, BslAction.Update);
        RemoveTrigger(trigger, BslAction.Delete);
        RemoveTrigger(trigger, BslAction.BeforeCreate);
        RemoveTrigger(trigger, BslAction.BeforeUpdate);
        RemoveTrigger(trigger, BslAction.BeforeDelete);
      }

      private void RemoveTrigger(BslTriggerEntity trigger, BslAction bslActionFlag)
      {
        if (TriggersByTypeAction.ContainsKey((trigger.TriggerEntity, bslActionFlag)))
        {
          TriggersByTypeAction[(trigger.TriggerEntity, bslActionFlag)].RemoveAll(t => t.BslTriggerId == trigger.BslTriggerId);
        }
      }

      internal BslTriggerState Copy()
      {
        return new BslTriggerState(TriggersById.ToDictionary(k => k.Key, k => k.Value), TriggersByTypeAction.ToDictionary(k => k.Key, k => k.Value));
      }
    }

    private interface IBslActionCaller
    {
      string ActionType { get; }

      void Execute(ISessionToken userId, BslTriggerRequest request, string config, IBslActionContext bslActionContext);
    }

    private class BslActionCaller<TAction, TConfig> : IBslActionCaller
      where TAction : IBslAction<TConfig>, new()
    {
      public string ActionType => typeof(TAction).ToString();

      public void Execute(ISessionToken userId, BslTriggerRequest request, string config, IBslActionContext bslActionContext)
      {
        new TAction().Execute(userId, request, JsonConvert.DeserializeObject<TConfig>(config), bslActionContext);
      }
    }

    private class BslActionCaller<TAction> : IBslActionCaller
      where TAction : IBslAction, new()
    {
      public string ActionType => typeof(TAction).ToString();

      public void Execute(ISessionToken userId, BslTriggerRequest request, string config, IBslActionContext bslActionContext)
      {
        new TAction().Execute(userId, request, bslActionContext);
      }
    }

    private class BslTriggerActionResolver
    {
      private readonly Dictionary<Guid, IBslActionCaller> map = new Dictionary<Guid, IBslActionCaller>();

      private readonly ReadOnlyCollection<BslTriggerActionEntity> bslTriggerActions;

      public BslTriggerActionResolver(TypeManager typeFinder)
      {
        var actions = new List<BslTriggerActionEntity>();

        foreach (var type in typeFinder.GetTypes())
        {
          if (!type.CanConstruct())
            continue;

          if (!typeof(IBslAction).IsAssignableFrom(type) && !typeof(IBslAction<>).IsAssignableFromGeneric(type))
            continue;

          if (type.GetCustomAttribute<GuidAttribute>() == null)
            throw new InvalidOperationException("Missing Guid attribute on Bsl Action");

          if (map.ContainsKey(type.GUID))
            throw new InvalidOperationException("Duplicate Guid found on Bsl Action");

          if (typeof(IBslAction).IsAssignableFrom(type))
          {
            map[type.GUID] = (IBslActionCaller)Activator.CreateInstance(typeof(BslActionCaller<>).MakeGenericType(type));
          }
          else
          {
            map[type.GUID] = (IBslActionCaller)Activator.CreateInstance(typeof(BslActionCaller<,>).MakeGenericType(type, type.GetInterfaces().First().GetGenericArguments()[0]));
          }

          actions.Add(new BslTriggerActionEntity
          {
            ActionName = type.FullName,
            ActionGuid = type.GUID
          });
        }

        bslTriggerActions = new ReadOnlyCollection<BslTriggerActionEntity>(actions);
      }

      public IBslActionCaller GetType(Guid guid)
      {
        if (map.TryGetValue(guid, out var bslActionCaller))
          return bslActionCaller;

        throw new InvalidOperationException($"Could not find bslActionCaller for Bsl Action with Guid { guid }. Check if removed or changed");
      }

      public IList<BslTriggerActionEntity> GetBslTriggerActions()
      {
        return bslTriggerActions;
      }
    }

    private class BslTriggerEnabledEntityResolver
    {
      private readonly ReadOnlyCollection<BslTriggerEnabledEntity> bslTriggerEnabledEntities;

      public BslTriggerEnabledEntityResolver(TypeManager typeFinder)
      {
        var entities = new List<BslTriggerEnabledEntity>();

        foreach (var type in typeFinder.GetTypes())
        {
          if (!typeof(EntityObject).IsAssignableFrom(type))
            continue;

          if (type.GetCustomAttribute<BslTriggerEntityAttribute>() == null || !type.GetCustomAttribute<BslTriggerEntityAttribute>().AllowUISelection)
            continue;

          entities.Add(new BslTriggerEnabledEntity
          {
            EntityName = type.FullName
          });
        }

        bslTriggerEnabledEntities = new ReadOnlyCollection<BslTriggerEnabledEntity>(entities);
      }

      public IList<BslTriggerEnabledEntity> GetBslTriggerEnabledEntities()
      {
        return bslTriggerEnabledEntities;
      }
    }

    private class CountingWriteStream : Stream
    {
      private readonly Action<int> written;

      public override bool CanRead => true;

      public override bool CanSeek => true;

      public override bool CanTimeout => false;

      public override bool CanWrite => true;

      public override long Position { get; set; }

      public override int ReadTimeout { get; set; }

      public override int WriteTimeout { get; set; }

      public override long Length { get; }

      public CountingWriteStream(Action<int> written)
      {
        this.written = written;
      }

      public override void Write(byte[] buffer, int offset, int count)
      {
        written(count);
      }

      public override void WriteByte(byte value)
      {
        written(1);
      }

      public override void Flush()
      {
      }

      public override long Seek(long offset, SeekOrigin origin)
      {
        return 0;
      }

      public override void SetLength(long value)
      {
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        return 0;
      }
    }

    private class JobTooBigException : Exception
    {
    }

    private class MaxRecursionLevelReachedException : InvalidOperationException
    {
      public MaxRecursionLevelReachedException()
      {
      }

      public MaxRecursionLevelReachedException(string message)
        : base(message)
      {
      }
    }
  }
}
