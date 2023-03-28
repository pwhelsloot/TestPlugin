using AMCS.Data;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL.Querying;
using AMCS.PlatformFramework.Entity.Api.Settings;

namespace AMCS.PlatformFramework.Server.Api.Settings
{
  public class ApiExampleSettingEditorDataService : EntityObjectService<ApiExampleSettingEditorData>
  {
    public ApiExampleSettingEditorDataService(IEntityObjectAccess<ApiExampleSettingEditorData> dataAccess)
    : base(dataAccess)
    {
    }

    public override ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        var editorData = new ApiExampleSettingEditorData();
        var mapExampleIdCriteria = FieldUtils.ExtractFromCriteria(criteria, nameof(ApiExampleSettingEditorData.ExampleSettingId), FieldComparison.Eq);
        if (mapExampleIdCriteria != null)
        {
          FieldUtils.Validate(mapExampleIdCriteria, typeof(int), FieldComparison.Eq, nameof(ApiExampleSettingEditorData.ExampleSettingId));
          editorData.DataModel = ds.GetById<ApiExampleSetting>(userId, (int)mapExampleIdCriteria.Value);
        }
        else
        {
          // This would usually use ds.GetNew<ApiExampleSetting>(userId) but that fires db calls which we want to avoid for example app
          editorData.DataModel = new ApiExampleSetting();
        }
        editorData.LookupItems = ds.GetAll<ApiExampleSettingLookup>(userId, false);
        return ApiQuery.Single(editorData);
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }
  }
}
