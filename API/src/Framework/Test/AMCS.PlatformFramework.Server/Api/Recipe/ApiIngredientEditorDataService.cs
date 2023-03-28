using System.Linq;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL.Querying;
using AMCS.PlatformFramework.Entity.Api.Recipe;

namespace AMCS.PlatformFramework.Server.Api.Recipe
{
  public class ApiIngredientEditorDataService : EntityObjectService<ApiIngredientEditorData>
  {
    public ApiIngredientEditorDataService(IEntityObjectAccess<ApiIngredientEditorData> dataAccess)
     : base(dataAccess)
    {
    }

    public override ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        IFieldExpression idField = FieldUtils.ExtractFromCriteria(criteria, nameof(ApiIngredientEditorData.IngredientId), FieldComparison.Eq);
        int? id = null;
        if (idField != null)
        {
          FieldUtils.Validate(idField, typeof(int), FieldComparison.Eq, nameof(ApiIngredientEditorData.IngredientId));
          id = (int)idField.Value;
        }

        ApiIngredientEditorData editorData = new ApiIngredientEditorData();
        editorData.DataModel = GetIngredient(userId, id, ds);
        editorData.SelectedMeasurement = editorData.DataModel.MeasurementId.HasValue ? StockData.GetMeasurements().Single(x => x.ApiMeasurementId == editorData.DataModel.MeasurementId.Value) : null;
        editorData.IngredientTypes = StockData.GetIngredientTypes();
        return ApiQuery.Single(editorData);
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    private static ApiIngredient GetIngredient(ISessionToken userId, int? id, IDataSession datasession)
    {
      if (id.HasValue)
      {
        return datasession.GetById<ApiIngredient>(userId, id.Value);
      }
      else
      {
        return new ApiIngredient();
      }
    }
  }
}
