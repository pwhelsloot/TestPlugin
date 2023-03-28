using AMCS.Data.Entity;
using AMCS.Data.Server.Broadcast;
using AMCS.Data.Server.Services;
using AMCS.Data.Server.SQL.Querying;

namespace AMCS.Data.Server.BslTriggers
{
  internal class BslTriggerCacheService : CacheCoherentEntityService<BslTriggerEntity>
  {
    public BslTriggerCacheService(IBroadcastService broadcastService) : base(broadcastService)
    {

    }

    protected override void RefreshData()
    {
      RaiseRefreshed();
    }

    protected override ICriteria GetFilterCriteria(string category)
    {
      var criteria = Criteria.For(typeof(BslTriggerEntity))
        .Add(Expression.Eq(nameof(BslTriggerEntity.SystemCategory), category));

      return criteria;
    }
  }
}