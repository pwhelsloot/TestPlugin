using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL.Querying;
using AMCS.PlatformFramework.Entity.Api.Recipe;
using System.Collections.Generic;
using System.Linq;

namespace AMCS.PlatformFramework.Server.Api.Recipe
{
  public class ApiIngredientTypeService : EntityObjectService<ApiIngredientType>
  {
    public static IList<ApiIngredientType> allData = StockData.GetIngredientTypes();
    
    // Next available number from the enum IngredientType
    private static int nextId = 7;

    public ApiIngredientTypeService(IEntityObjectAccess<ApiIngredientType> dataAccess)
    : base(dataAccess)
    {
    }

    public override int? Save(ISessionToken userId, ApiIngredientType entity, IDataSession existingDataSession = null)
    {
      if (entity.ApiIngredientTypeId == (int)IngredientType.PANTRY)
      {
        throw BslUserExceptionFactory<BslUserException>.CreateException("Pantry Cannot be edited");
      }
      
      if (entity.ApiIngredientTypeId.HasValue)
      {
        allData.Single(x => x.ApiIngredientTypeId == entity.ApiIngredientTypeId.Value).Description = entity.Description;
      }
      else
      {
        entity.ApiIngredientTypeId = nextId;
        allData.Add(entity);
        nextId++;
      }
      
      return entity.ApiIngredientTypeId;
    }

    public override void Delete(ISessionToken userId, ApiIngredientType entity, IDataSession existingDataSession = null)
    {
      if (entity.ApiIngredientTypeId == (int)IngredientType.PANTRY)
      {
        throw BslUserExceptionFactory<BslUserException>.CreateException("Pantry Cannot be deleted");
      }
      
      allData = allData.Where(x => x.ApiIngredientTypeId != entity.ApiIngredientTypeId).ToList();
    }

    public override ApiIngredientType GetById(ISessionToken userId, int id, IDataSession existingDataSession = null)
    {
      return new ApiIngredientType { ApiIngredientTypeId = id };
    }

    public override ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null)
    {
      return new ApiQuery(allData.Cast<EntityObject>().ToList(), allData.Count);
    }
  }
}
