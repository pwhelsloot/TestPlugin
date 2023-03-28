using System;
using System.Collections.Generic;
using System.Linq;
using AMCS.Data.Entity;
using AMCS.Data.Server.Broadcast;
using AMCS.Data.Server.SQL.Querying;

namespace AMCS.Data.Server.Services
{
  public abstract class CacheCoherentEntityService<T> : ICacheCoherentEntityService<T>
    where T : EntityObject, ICacheCoherentEntity, new()
  {
    private readonly IBroadcastService broadcastService;

    public event EventHandler DataRefreshed;

    public CacheCoherentEntityService(IBroadcastService broadcastService)
    {
      this.broadcastService = broadcastService;
    }

    public void Start()
    {
      broadcastService.On<BroadcastMessage<T>>(entity => RefreshData());
      RefreshData();
    }

    public virtual void Publish(IList<T> publishedEntities, string category,
      ISessionToken systemToken, IDataSession dataSession, BroadcastMode broadcastMode = BroadcastMode.Single)
    {
      if (publishedEntities == null)
        publishedEntities = new List<T>();

      var filterCriteria = GetFilterCriteria(category);
      var dbEntities = dataSession.GetAllByCriteria<T>(systemToken, filterCriteria);

      var updateList = new List<T>();
      foreach (var publishedEntity in publishedEntities)
      {
        var existingEntity = dbEntities.SingleOrDefault(entity => entity.Id32 == publishedEntity.Id32);

        if (existingEntity != null && publishedEntity.IsEqualTo(existingEntity))
          continue;

        updateList.Add(publishedEntity);
        dataSession.Save(systemToken, publishedEntity);
        Broadcast();
      }

      var hasDeleted = false;

      foreach (var dbEntity in dbEntities)
      {
        var deletedEntity = publishedEntities.SingleOrDefault(entity => entity.Id32 == dbEntity.Id32);

        if (deletedEntity != null)
          continue;

        hasDeleted = true;
        dataSession.Delete(systemToken, dbEntity, false);
        Broadcast();
      }

      if (broadcastMode == BroadcastMode.Batch && (updateList.Count > 0 || hasDeleted))
        dataSession.Broadcast(new BroadcastMessage<T>());

      void Broadcast()
      {
        if (broadcastMode == BroadcastMode.Single)
          dataSession.Broadcast(new BroadcastMessage<T>());
      }
    }

    protected void RaiseRefreshed()
    {
      DataRefreshed?.Invoke(this, EventArgs.Empty);
    }


    protected abstract void RefreshData();

    protected abstract ICriteria GetFilterCriteria(string category);

    private class BroadcastMessage<TEntity>
    {
    }
  }
}