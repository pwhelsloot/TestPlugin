using System.Collections.Generic;
using System.Linq;
using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL.Querying;
using AMCS.PlatformFramework.Entity.Api.Settings;
using AMCS.PlatformFramework.Server.Api.Recipe;

namespace AMCS.PlatformFramework.Server.Api.Settings
{
  public class ApiExampleSettingBrowserService : EntityObjectService<ApiExampleSettingBrowser>
  {
    public ApiExampleSettingBrowserService(IEntityObjectAccess<ApiExampleSettingBrowser> dataAccess)
    : base(dataAccess)
    {
    }

    public override ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);
      List<ApiExampleSettingBrowser> results = StockData.ExampleSettingsBrowsers;
      try
      {
        return new ApiQuery(results?.Cast<EntityObject>()?.ToList(), results?.Count);
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }
  }
}

