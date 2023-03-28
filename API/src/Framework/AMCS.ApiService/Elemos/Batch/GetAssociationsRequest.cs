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
  internal class GetAssociationsRequest : IBatchRequest
  {
    private readonly string collectionName;
    private readonly int id;
    private readonly string associatedCollectionName;
    private readonly string filter;
    private readonly string search;
    private readonly string order;
    private readonly int? max;
    private readonly int? page;
    private readonly bool? includeDeleted;
    private readonly string udf;

    public GetAssociationsRequest(string collectionName, int id, string associatedCollectionName, string filter, string search, string order, int? max, int? page, bool? includeDeleted, string udf)
    {
      this.collectionName = collectionName;
      this.id = id;
      this.associatedCollectionName = associatedCollectionName;
      this.filter = filter;
      this.search = search;
      this.order = order;
      this.max = max;
      this.page = page;
      this.includeDeleted = includeDeleted;
      this.udf = udf;
    }

    public void Perform(JsonWriter json, ISessionToken user, IApiContext context)
    {
      var metadata = DataServices.Resolve<IEntityObjectMetadataManager>().ForCollection(collectionName);

      foreach (var relationship in metadata.Relationships)
      {
        if (
          relationship is IEntityObjectChildRelationship childRelationship &&
          childRelationship.Name == associatedCollectionName)
        {
          new EntityObjectInternalReader(metadata.EntityType, user)
            .GetAssociations(id, childRelationship, filter, search, order, max, page, includeDeleted, null, udf, context)
            .Write(json);

          return;
        }
      }

      throw new BatchRequestException($"Cannot find associated collection {associatedCollectionName} of collection {collectionName}");
    }
  }
}
