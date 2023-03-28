using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AMCS.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AMCS.ApiService.Elemos.Batch
{
  internal class CreateRequest : IBatchRequest
  {
    private readonly string collectionName;
    private readonly JObject data;

    public CreateRequest(string collectionName, JObject data)
    {
      this.collectionName = collectionName;
      this.data = data;
    }

    public void Perform(JsonWriter json, ISessionToken user, IApiContext context)
    {
      var metadata = DataServices.Resolve<IEntityObjectMetadataManager>().ForCollection(collectionName);

      new EntityObjectInternalReader(metadata.EntityType, user)
        .Create(data)
        .Write(json);
    }
  }
}
