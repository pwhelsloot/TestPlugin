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
  internal class GetNewRequest : IBatchRequest
  {
    private readonly string collectionName;

    public GetNewRequest(string collectionName)
    {
      this.collectionName = collectionName;
    }

    public void Perform(JsonWriter json, ISessionToken user, IApiContext context)
    {
      var metadata = DataServices.Resolve<IEntityObjectMetadataManager>().ForCollection(collectionName);

      new EntityObjectInternalReader(metadata.EntityType, user)
        .GetNew()
        .Write(json);
    }
  }
}
