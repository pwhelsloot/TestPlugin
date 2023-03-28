using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AMCS.ApiService.Support.JsonDom;
using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;

namespace AMCS.ApiService.Elemos
{
  internal class EntityObjectInternalReader : EntityObjectReader<int>
  {
    public EntityObjectInternalReader(Type entityType, ISessionToken user)
      : base(entityType, user)
    {
    }

    public override EntityObject GetById(IDataSession session, ISessionToken user, Type entityType, int id)
    {
      return session.GetById(user, entityType, id);
    }

    protected override IJsonDomElement BuildIdResponse(IDataSession session, Type entityType, int? id, Guid? guid = null)
    {
      var response = new JsonDomObject();
      response.Add("resource", id.HasValue ? JsonDomValue.From(id.Value) : null);
      return response;
    }

    protected override IJsonDomElement BuildStatusResponse(IDataSession session, Type entityType, int? id, bool isSuccess)
    {
      var response = new JsonDomObject();

      var status = new JsonDomObject();
      response.Add("status", status);

      status.Add("id", id.HasValue ? JsonDomValue.From(id.Value) : null);
      status.Add("isSuccess", JsonDomValue.From(isSuccess));

      return response;
    }

    protected override IJsonDomElement BuildCollectionResource(IList<EntityObject> entities, string links, string udf, IApiContext context, IDataSession session)
    {
      var result = new JsonDomEntityArray(entities);
      if (!string.IsNullOrEmpty(udf))
        SetUdfDataForMultipleEntities(udf, entities, result, session);

      return result;
    }

    protected override int GetInternalId(IDataSession session, ISessionToken user, Type entityType, int id)
    {
      return id;
    }

    protected override EntityLinkBuilder GetLinkBuilder(IApiContext context, EntityObject entity)
    {
      return EntityLinkInternalBuilder.ForEntity(context, entity);
    }
  }
}
