namespace AMCS.Data.Server.WebHook
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using Entity.WebHook;
  using AMCS.Data.Server.Broadcast;
  using AMCS.Data.Server.Services;
  using AMCS.Data.Server.SQL.Querying;
  
  public class WebHookCacheService : CacheCoherentEntityService<WebHookEntity>, IWebHookCacheService
  {
    private readonly IUserService userService;
    private volatile IList<WebHookEntity> webHooks = new List<WebHookEntity>();

    public WebHookCacheService(IBroadcastService broadcastService, IUserService userService)
      : base(broadcastService)
    {
      this.userService = userService;
    }

    protected override void RefreshData()
    {
      var systemToken = userService.CreateSystemSessionToken();
      using (var dataSession = BslDataSessionFactory.GetDataSession(systemToken))
      using (var transaction = dataSession.CreateTransaction())
      {
        webHooks = dataSession.GetAll<WebHookEntity>(systemToken, false);
        transaction.Commit();
      }

      RaiseRefreshed();
    }

    protected override ICriteria GetFilterCriteria(string category)
    {
      var criteria = Criteria.For(typeof(WebHookEntity))
        .Add(Expression.Eq(nameof(WebHookEntity.SystemCategory), category));

      return criteria;
    }

    public IList<WebHookEntity> GetWebHooks()
    {
      return new ReadOnlyCollection<WebHookEntity>(webHooks);
    }
  }
}