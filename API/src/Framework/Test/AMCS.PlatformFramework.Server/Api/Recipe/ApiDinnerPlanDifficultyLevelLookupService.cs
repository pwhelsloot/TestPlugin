using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL.Querying;
using AMCS.PlatformFramework.Entity.Api.Recipe;
using System.Collections.Generic;
using System.Linq;

namespace AMCS.PlatformFramework.Server.Api.Recipe
{
  // RDM - You'd never normally need create a service/sql layer for a lookup, this is just to get test data.
  public class ApiDinnerPlanDifficultyLevelLookupService : EntityObjectService<ApiDinnerPlanDifficultyLevelLookup>
  {
    public ApiDinnerPlanDifficultyLevelLookupService(IEntityObjectAccess<ApiDinnerPlanDifficultyLevelLookup> dataAccess)
    : base(dataAccess)
        {
        }

    private int idCount = 1;

    public override IList<ApiDinnerPlanDifficultyLevelLookup> GetAllById(ISessionToken userId, int id, bool includeDeleted, IDataSession existingDataSession = null)
    {
      var items = StockData.GetDifficultyLevels();
      List<ApiDinnerPlanDifficultyLevelLookup> difficultyLevels = new List<ApiDinnerPlanDifficultyLevelLookup>();
      foreach (var item in items)
      {
        difficultyLevels.Add(new ApiDinnerPlanDifficultyLevelLookup()
        {
          DinnerPlanDifficultyLevelId = idCount++,
          Description = item.Key,
        });
      }
      return difficultyLevels;
    }
  }
}