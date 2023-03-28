using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AMCS.Data;
using Newtonsoft.Json;

namespace AMCS.ApiService.Elemos.Batch
{
  internal class DeleteRequest : IBatchRequest
  {
    private readonly string collectionName;
    private readonly int id;

    public DeleteRequest(string collectionName, int id)
    {
      this.collectionName = collectionName;
      this.id = id;
    }

    public void Perform(JsonWriter json, ISessionToken user, IApiContext context)
    {
      var metadata = DataServices.Resolve<IEntityObjectMetadataManager>().ForCollection(collectionName);

      new EntityObjectInternalReader(metadata.EntityType, user)
        .Delete(id);

      json.WriteNull();
    }
  }
}
