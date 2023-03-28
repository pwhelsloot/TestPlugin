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
  internal class GetRequest : IBatchRequest
  {
    private readonly string collectionName;
    private readonly int id;
    private readonly string links;
    private readonly string include;
    private readonly string expand;
    private readonly string udf;

    public GetRequest(string collectionName, int id, string links, string include, string expand, string udf)
    {
      this.collectionName = collectionName;
      this.id = id;
      this.links = links;
      this.include = include;
      this.expand = expand;
      this.udf = udf;
    }

    public void Perform(JsonWriter json, ISessionToken user, IApiContext context)
    {
      var metadata = DataServices.Resolve<IEntityObjectMetadataManager>().ForCollection(collectionName);

      new EntityObjectInternalReader(metadata.EntityType, user)
        .GetById(id, links, include, expand, udf, context)
        .Write(json);
    }
  }
}
