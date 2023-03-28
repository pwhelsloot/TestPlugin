using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AMCS.ApiService.Support;
using AMCS.Data;
using AMCS.Data.Entity;

namespace AMCS.ApiService.Elemos
{
  internal abstract class EntityLinkBuilder
  {
    private readonly EntityObject entity;
    private readonly IEntityObjectMetadata metadata;
    private readonly string baseUrl;

    public abstract string Self { get; }

    protected EntityLinkBuilder(EntityObject entity, IEntityObjectMetadata metadata, string baseUrl)
    {
      this.entity = entity;
      this.metadata = metadata;
      this.baseUrl = baseUrl;
    }

    public IList<string> GetOperations()
    {
      var result = new List<string>();

      foreach (var operation in metadata.Operations)
      {
        result.Add(Self + "/op/" + operation.Name);
      }

      return result;
    }

    public IList<string> GetAssociations()
    {
      var result = new List<string>();

      foreach (var relationship in metadata.Relationships)
      {
        if (relationship is IEntityObjectChildRelationship childRelationship)
          result.Add(Self + "/" + Inflector.Pluralize(childRelationship.Name));
      }

      return result;
    }

    public IList<string> GetExpands()
    {
      var result = new List<string>();

      foreach (var relationship in metadata.Relationships)
      {
        if (relationship is IEntityObjectParentRelationship parentRelationship)
        {
          var parentMetadata = DataServices.Resolve<IEntityObjectMetadataManager>().ForType(parentRelationship.Target);
          int? parentId = (int?)parentRelationship.Property.GetValue(entity);

          if (parentId.HasValue)
            result.Add(baseUrl + parentMetadata.CollectionName + "/" + parentId.Value);
        }
      }

      return result;
    }
  }
}
