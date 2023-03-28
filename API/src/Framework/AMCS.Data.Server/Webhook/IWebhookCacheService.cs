namespace AMCS.Data.Server.WebHook
{
  using System.Collections.Generic;
  using Entity.WebHook;
  using AMCS.Data.Server.Services;
  
  public interface IWebHookCacheService : ICacheCoherentEntityService<WebHookEntity>
  {
    IList<WebHookEntity> GetWebHooks();
  }
}