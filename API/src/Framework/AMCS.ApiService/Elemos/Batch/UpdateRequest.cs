using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AMCS.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AMCS.ApiService.Elemos.Batch
{
  internal class UpdateRequest : IBatchRequest
  {
    private readonly string collectionName;
    private readonly int id;
    private readonly JObject data;

    public UpdateRequest(string collectionName, int id, JObject data)
    {
      this.collectionName = collectionName;
      this.id = id;
      this.data = data;
    }

    public void Perform(JsonWriter json, ISessionToken user, IApiContext context)
    {
      var metadata = DataServices.Resolve<IEntityObjectMetadataManager>().ForCollection(collectionName);

      new EntityObjectInternalReader(metadata.EntityType, user)
        .Update(id, data)
        .Write(json);
    }
  }
}
