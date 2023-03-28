using AMCS.Data;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL.Querying;
using AMCS.PlatformFramework.Entity.Api.Snippets;

namespace AMCS.PlatformFramework.Server.Api.Snippets
{
  public class ApiSnippetExampleEditorDataService : EntityObjectService<ApiSnippetExampleEditorData>
  {
    public ApiSnippetExampleEditorDataService(IEntityObjectAccess<ApiSnippetExampleEditorData> dataAccess)
  : base(dataAccess)
    {
    }


    public override ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null)
    {
      ApiSnippetExampleEditorData emptyExample = new ApiSnippetExampleEditorData();
      emptyExample.DataModel = new ApiSnippetExample { SnippetExampleId = 1 };
      return new ApiQuery(new[] { emptyExample }, null);
    }
  }
}