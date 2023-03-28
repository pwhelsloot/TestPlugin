using System.Collections.Generic;
using System.Linq;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.PlatformFramework.Entity.Api.Settings;
using AMCS.PlatformFramework.Server.Api.Recipe;

namespace AMCS.PlatformFramework.Server.Api.Settings
{
  public class ApiExampleSettingService : EntityObjectService<ApiExampleSetting>
  {
    public ApiExampleSettingService(IEntityObjectAccess<ApiExampleSetting> dataAccess)
    : base(dataAccess)
    {
    }

    public override int? Save(ISessionToken userId, ApiExampleSetting entity, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);

      try
      {
        if (StockData.ExampleSettingsBrowsers == null || StockData.ExampleSettings == null)
        {
          StockData.ExampleSettingsBrowsers = new List<ApiExampleSettingBrowser>();
          StockData.ExampleSettings = new List<ApiExampleSetting>();
        }

        IList<ApiExampleSettingLookup> lookupItems = ds.GetAll<ApiExampleSettingLookup>(userId, false);

        if (entity.ExampleSettingId == null)
        {
          entity.ExampleSettingId = StockData.ExampleSettings.Count + 1;
          StockData.ExampleSettings.Add(entity);
          StockData.ExampleSettingsBrowsers.Add(ConvertToBrowserItem(entity, lookupItems));
        }
        else
        {
          var data = StockData.ExampleSettings.FirstOrDefault(x => x.ExampleSettingId == entity.ExampleSettingId );
          var browserData = StockData.ExampleSettingsBrowsers.FirstOrDefault(x => x.ExampleSettingId == entity.ExampleSettingId);
          data.Description = entity.Description;
          data.DropdownId = entity.DropdownId;
          browserData.Description = entity.Description;
          browserData.Dropdown = lookupItems.Single(x => x.ExampleSettingLookupId == entity.DropdownId).Description;
        }

        return entity.ExampleSettingId;
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    private static ApiExampleSettingBrowser ConvertToBrowserItem(ApiExampleSetting entity, IList<ApiExampleSettingLookup> lookups)
    {
      return new ApiExampleSettingBrowser
      {
        ExampleSettingId = entity.ExampleSettingId,
        Description = entity.Description,
        Dropdown = lookups.Single(x => x.ExampleSettingLookupId == entity.DropdownId).Description
      };
      }

    public override void Delete(ISessionToken userId, ApiExampleSetting entity, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);

      try
      {
        if (StockData.ExampleSettingsBrowsers == null || StockData.ExampleSettings == null)
            return;

        if (entity.ExampleSettingId == null)
          return;

        StockData.ExampleSettingsBrowsers = StockData.ExampleSettingsBrowsers.Where(x => x.ExampleSettingId != entity.ExampleSettingId).ToList();
        StockData.ExampleSettings = StockData.ExampleSettings.Where(x => x.ExampleSettingId != entity.ExampleSettingId).ToList();
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    public override ApiExampleSetting GetById(ISessionToken userId, int id, IDataSession existingDataSession = null)
    {
      return StockData.ExampleSettings.FirstOrDefault(x => x.ExampleSettingId == id);
    }
  }
}