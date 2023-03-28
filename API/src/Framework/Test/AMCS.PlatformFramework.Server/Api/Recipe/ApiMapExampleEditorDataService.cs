using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL.Querying;
using AMCS.PlatformFramework.Entity.Api.Recipe;
using System.Collections.Generic;

namespace AMCS.PlatformFramework.Server.Api.Recipe
{
  public class ApiMapExampleEditorDataService : EntityObjectService<ApiMapExampleEditorData>
  {
    public ApiMapExampleEditorDataService(IEntityObjectAccess<ApiMapExampleEditorData> dataAccess)
    : base(dataAccess)
    {
    }

    public override ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);

      try
      {
        var editorData = new ApiMapExampleEditorData();

        var mapExampleIdCriteria = FieldUtils.ExtractFromCriteria(criteria, nameof(ApiMapExampleEditorData.MapExampleId), FieldComparison.Eq);
        if (mapExampleIdCriteria != null)
        {
          FieldUtils.Validate(mapExampleIdCriteria, typeof(int), FieldComparison.Eq, nameof(ApiMapExampleEditorData.MapExampleId));
          editorData.MapExample = ds.GetById<ApiMapExample>(userId, (int)mapExampleIdCriteria.Value);
        }
        else
        {
          // This would usually use ds.GetNew<ApiMapExample>(userId) but that fires db calls which we want to avoid for example app
          editorData.MapExample = new ApiMapExample();
        }

        return new ApiQuery(new List<EntityObject> { editorData }, null);
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }
  }
}
