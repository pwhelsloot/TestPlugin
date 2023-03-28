using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL.Querying;
using AMCS.PlatformFramework.Entity.Api.Recipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.PlatformFramework.Server.Api.Recipe
{
  public class ApiRecipeService : EntityObjectService<ApiRecipe>
  {
    public ApiRecipeService(IEntityObjectAccess<ApiRecipe> dataAccess)
    : base(dataAccess)
    {
    }

    public override ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null)
    {
      var dummyData = new List<ApiRecipe>();
      
      dummyData.Add(StockData.GetOvenBakedChickenBites());
      dummyData.Add(StockData.GetCarrotCorianderSoup());
      dummyData.Add(StockData.GetPorridge());
      dummyData.Add(StockData.GetCheeseSandwich());
      dummyData.Add(StockData.GetBoiledEgg());
      dummyData.Add(StockData.GetBeansOnToast());

      // if our data is coming from the database and the criteria includes values for MaxResults / FirstResult 
      // the framework will automatically add SQL to limit the results returned. in some cases as here, we need 
      // to do this paging at the service level and we generally do this with the LINQ operators Skip and Take.
      int skip = criteria.FirstResult ?? 0;
      int take = criteria.MaxResults ?? int.MaxValue;
      IList<ApiRecipe> pagedData = dummyData.Skip(skip).Take(take).ToList();
      
      return new ApiQuery(pagedData.Cast<EntityObject>().ToList(), dummyData.Count);
    }
  }
}
