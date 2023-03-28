namespace AMCS.Data.Server.Glossary
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using AMCS.Data.Entity.Glossary;
  using AMCS.Data.Server.Broadcast;
  using AMCS.Data.Server.Services;
  using AMCS.Data.Server.SQL.Querying;
  using log4net;

  public class GlossaryCacheService : CacheCoherentEntityService<ApiGlossary>, IGlossaryCacheService
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(GlossaryCacheService));
   
    private readonly IUserService userService;
    private volatile IList<ApiGlossary> glossaries = new List<ApiGlossary>();

    public GlossaryCacheService(IUserService userService,IBroadcastService broadcastService)
      : base(broadcastService)
    {
      this.userService = userService;
    }

    protected override void RefreshData()
    {
      try
      {
        var systemToken = userService.CreateSystemSessionToken();
        using (var dataSession = BslDataSessionFactory.GetDataSession(systemToken))
        using (var transaction = dataSession.CreateTransaction())
        {
          glossaries = dataSession.GetAll<ApiGlossary>(systemToken, false);
          transaction.Commit();
          RaiseRefreshed();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Could not load glossaries from the database.", ex);
      }
    }

    protected override ICriteria GetFilterCriteria(string category)
    {
      return Criteria.For(typeof(ApiGlossary));
    }

    public IList<ApiGlossary> GetGlossaries()
    {
      return new ReadOnlyCollection<ApiGlossary>(glossaries);
    }
  }
}