using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AMCS.Data;
using Newtonsoft.Json;

namespace AMCS.ApiService.Elemos.Batch
{
  internal class GetCollectionRequest : IBatchRequest
  {
    private readonly string collectionName;
    private readonly string filter;
    private readonly string search;
    private readonly string order;
    private readonly int? max;
    private readonly int? page;
    private readonly bool? includeCount;
    private readonly bool? includeDeleted;
    private readonly string udf;

    public GetCollectionRequest(string collectionName, string filter, string search, string order, int? max, int? page, bool? includeCount, bool? includeDeleted, string udf)
    {
      this.collectionName = collectionName;
      this.filter = filter;
      this.search = search;
      this.order = order;
      this.max = max;
      this.page = page;
      this.includeCount = includeCount;
      this.includeDeleted = includeDeleted;
      this.udf = udf;
    }

    public void Perform(JsonWriter json, ISessionToken user, IApiContext context)
    {
      var metadata = DataServices.Resolve<IEntityObjectMetadataManager>().ForCollection(collectionName);

      new EntityObjectInternalReader(metadata.EntityType, user)
        .GetCollection(filter, search, order, max, page, includeCount, includeDeleted, null, udf, context)
        .Write(json);
    }
  }
}
