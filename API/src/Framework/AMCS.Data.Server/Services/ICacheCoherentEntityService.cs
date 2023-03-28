using System;
using System.Collections.Generic;
using AMCS.Data.Configuration;
using AMCS.Data.Entity;

namespace AMCS.Data.Server.Services
{
  public interface ICacheCoherentEntityService<T> : IDelayedStartup
    where T : EntityObject, ICacheCoherentEntity, new()
  {
    event EventHandler DataRefreshed;

    void Publish(IList<T> publishedEntities, string category,
      ISessionToken systemToken, IDataSession dataSession, BroadcastMode broadcastMode = BroadcastMode.Single);
  }
}