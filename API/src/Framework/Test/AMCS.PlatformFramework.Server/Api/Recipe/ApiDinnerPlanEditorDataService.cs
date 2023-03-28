using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL.Querying;
using AMCS.PlatformFramework.Entity.Api.Recipe;
using System.Collections.Generic;

namespace AMCS.PlatformFramework.Server.Api.Recipe
{
  public class ApiDinnerPlanEditorDataService : EntityObjectService<ApiDinnerPlanEditorData>
  {
    public ApiDinnerPlanEditorDataService(IEntityObjectAccess<ApiDinnerPlanEditorData> dataAccess)
   : base(dataAccess)
    {
    }

    public override ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);

      try
      {
        var editorData = new ApiDinnerPlanEditorData();

        var dinnerPlanIdCriteria = FieldUtils.ExtractFromCriteria(criteria, nameof(ApiDinnerPlanEditorData.DinnerPlanId), FieldComparison.Eq);
        if (dinnerPlanIdCriteria != null)
        {
          editorData.DataModel = ds.GetById<ApiDinnerPlan>(userId, (int)dinnerPlanIdCriteria.Value);
        }
        else
        {
          // This would usually use ds.GetNew<ApiDinnerPlan>(userId) but that fires db calls which we want to avoid for example app
          editorData.DataModel = new ApiDinnerPlan();
        }

        editorData.Courses = ds.GetAll<ApiDinnerCourseLookup>(userId, false);
        editorData.DifficultyLevels = ds.GetAll<ApiDinnerPlanDifficultyLevelLookup>(userId, false);

        return new ApiQuery(new List<EntityObject> { editorData }, null);
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }
  }
}
