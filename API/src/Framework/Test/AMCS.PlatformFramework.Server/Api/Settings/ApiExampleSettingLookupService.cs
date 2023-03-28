using System.Collections.Generic;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.PlatformFramework.Entity.Api.Settings;
using AMCS.PlatformFramework.Server.Api.Recipe;

namespace AMCS.PlatformFramework.Server.Api.Settings
{
  public class ApiExampleSettingLookupService : EntityObjectService<ApiExampleSettingLookup>
  {
    public ApiExampleSettingLookupService(IEntityObjectAccess<ApiExampleSettingLookup> dataAccess)
    : base(dataAccess)
    {
    }
    public override IList<ApiExampleSettingLookup> GetAllById(ISessionToken userId, int id, bool includeDeleted, IDataSession existingDataSession = null)
    {
      if (StockData.ExampleSettingLookups == null)
      {
        StockData.ExampleSettingLookups = new List<ApiExampleSettingLookup>
        {
          new ApiExampleSettingLookup()
            {
            ExampleSettingLookupId = 1,
            Description = "Lookup One"
            },
          new ApiExampleSettingLookup()
            {
            ExampleSettingLookupId = 2,
            Description = "Lookup Two"
            }
        };
      }
      return StockData.ExampleSettingLookups;
    }
  }
}