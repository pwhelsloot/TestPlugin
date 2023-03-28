using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AMCS.ApiService.Support;
using AMCS.Data;
using AMCS.Data.Configuration;
using AMCS.Data.Entity;

namespace AMCS.ApiService.Elemos
{
  internal class EntityLinkInternalBuilder : EntityLinkBuilder
  {
    public override string Self { get; }

    public static EntityLinkBuilder ForEntity(IApiContext context, EntityObject entity)
    {
      string serviceRoot = DataServices.Resolve<IServiceRootResolver>().ServiceRoot;
      var metadata = DataServices.Resolve<IEntityObjectMetadataManager>().ForType(entity.GetType());

      return new EntityLinkInternalBuilder(entity, metadata, serviceRoot);
    }

    protected EntityLinkInternalBuilder(EntityObject entity, IEntityObjectMetadata metadata, string serviceRoot)
      : base(entity, metadata, serviceRoot)
    {
      Self = $"{serviceRoot.TrimEnd('/')}/{metadata.CollectionName}/{entity.Id32}";
    }
  }
}
