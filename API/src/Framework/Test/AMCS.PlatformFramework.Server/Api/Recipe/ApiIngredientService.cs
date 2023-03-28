using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL.Querying;
using AMCS.PlatformFramework.Entity.Api.Recipe;
using System.Collections.Generic;
using System.Linq;

namespace AMCS.PlatformFramework.Server.Api.Recipe
{
  public class ApiIngredientService : EntityObjectService<ApiIngredient>
  {    
    public ApiIngredientService(IEntityObjectAccess<ApiIngredient> dataAccess)
     : base(dataAccess)
    {
    }

    private int IdCount = 0;

    public override int? Save(ISessionToken userId, ApiIngredient entity, IDataSession existingDataSession = null)
    {
      return 1;
    }

    public override ApiIngredient GetById(ISessionToken userId, int id, IDataSession existingDataSession = null)
    {
      return new ApiIngredient { IngredientId = id, MeasurementId = 1, TypeId = 1, Name = "Test", Amount = 10.01 };
    }

    public override ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null)
    {
      List<ApiIngredient> dummyData = new List<ApiIngredient>();
      AddSpiceType(dummyData);
      AddPantryType(dummyData);
      AddRefrigeratorType(dummyData);
      AddCondimentsType(dummyData);
      AddVegetablesType(dummyData);
      AddCannedGoodsType(dummyData);

      if (ApiIngredientTypeService.allData?.Count > 0)
      {
        dummyData = dummyData.Where(ingredient => ApiIngredientTypeService.allData.Any(type => type.ApiIngredientTypeId == ingredient.TypeId)).ToList();
        foreach (ApiIngredient ingredient in dummyData)
        {
          ingredient.Type = ApiIngredientTypeService.allData.Single(type => type.ApiIngredientTypeId == ingredient.TypeId).Description;
        }
      }

      IFieldExpression typeField = FieldUtils.ExtractFromCriteria(criteria, nameof(ApiIngredient.TypeId), FieldComparison.Eq);
      if (typeField != null)
      {
        FieldUtils.Validate(typeField, typeof(int), FieldComparison.Eq, nameof(ApiIngredient.TypeId));
        dummyData = dummyData.Where(x => x.TypeId == (int)typeField.Value).ToList();
      }

      int skip = criteria.FirstResult ?? 0;
      int take = criteria.MaxResults ?? int.MaxValue;
      IList<ApiIngredient> pagedData = dummyData.Skip(skip).Take(take).ToList();

      return new ApiQuery(pagedData.Cast<EntityObject>().ToList(), dummyData.Count);
    }

    private void AddSpiceType(List<ApiIngredient> allIngredients)
    {
      var items = StockData.GetSpiceGoods();
      List<ApiIngredient> ingredients = new List<ApiIngredient>();
      
      foreach (var item in items)
      {
        ingredients.Add(new ApiIngredient()
        {
          IngredientId = IdCount++,
          Name = item.Key,
          Type = IngredientType.SPICES.ToString(),
          TypeId = (int)IngredientType.SPICES,
          Optional = false,
          MeasurementId = item.Value,
          Measurement = ((Measurement)item.Value).ToString()
        });
      }

      allIngredients.AddRange(ingredients);
    }

    private void AddPantryType(List<ApiIngredient> allIngredients)
    {
      var items = StockData.GetPantryGoods();
      List<ApiIngredient> ingredients = new List<ApiIngredient>();

      foreach (var item in items)
      {
        ingredients.Add(new ApiIngredient()
        {
          IngredientId = IdCount++,
          Name = item.Key,
          Type = IngredientType.PANTRY.ToString(),
          TypeId = (int)IngredientType.PANTRY,
          Optional = false,
          MeasurementId = item.Value,
          Measurement = ((Measurement)item.Value).ToString()
        });
      }

      allIngredients.AddRange(ingredients);
    }

    private void AddRefrigeratorType(List<ApiIngredient> allIngredients)
    {
      var items = StockData.GetRefrigeratorGoods();
      List<ApiIngredient> ingredients = new List<ApiIngredient>();

      foreach (var item in items)
      {
        ingredients.Add(new ApiIngredient()
        {
          IngredientId = IdCount++,
          Name = item.Key,
          Type = IngredientType.REFRIGERATOR.ToString(),
          TypeId = (int)IngredientType.REFRIGERATOR,
          Optional = false,
          MeasurementId = item.Value,
          Measurement = ((Measurement)item.Value).ToString()
        });
      }

      allIngredients.AddRange(ingredients);
    }

    private void AddCondimentsType(List<ApiIngredient> allIngredients)
    {
      var items = StockData.GetCondimentGoods();
      List<ApiIngredient> ingredients = new List<ApiIngredient>();

      foreach (var item in items)
      {
        ingredients.Add(new ApiIngredient()
        {
          IngredientId = IdCount++,
          Name = item.Key,
          Type = IngredientType.CONDIMENTS.ToString(),
          TypeId = (int)IngredientType.CONDIMENTS,
          Optional = true,
          MeasurementId = item.Value,
          Measurement = ((Measurement)item.Value).ToString()
        });
      }

      allIngredients.AddRange(ingredients);
    }

    private void AddVegetablesType(List<ApiIngredient> allIngredients)
    {
      var items = StockData.GetVegitableGoods();
      List<ApiIngredient> ingredients = new List<ApiIngredient>();

      foreach (var item in items)
      {
        ingredients.Add(new ApiIngredient()
        {
          IngredientId = IdCount++,
          Name = item.Key,
          Type = IngredientType.VEGETABLES.ToString(),
          TypeId = (int)IngredientType.VEGETABLES,
          Optional = false,
          MeasurementId = item.Value,
          Measurement = ((Measurement)item.Value).ToString()
        });
      }

      allIngredients.AddRange(ingredients);
    }

    private void AddCannedGoodsType(List<ApiIngredient> allIngredients)
    {
      var items = StockData.GetCannedGoods();
      List<ApiIngredient> ingredients = new List<ApiIngredient>();
      foreach (var item in items)
      {
        ingredients.Add(new ApiIngredient()
        {
          IngredientId = IdCount++,
          Name = item.Key,
          Type = IngredientType.CANNEDGOODS.ToString(),
          TypeId = (int)IngredientType.CANNEDGOODS,
          Optional = false,
          MeasurementId = item.Value,
          Measurement = ((Measurement)item.Value).ToString()
        });
      }

      allIngredients.AddRange(ingredients);
    }
  }
}
