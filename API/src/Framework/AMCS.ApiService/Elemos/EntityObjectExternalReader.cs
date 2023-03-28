namespace AMCS.ApiService.Elemos
{
  using System;
  using System.Collections.Generic;
  using AMCS.ApiService.Support.JsonDom;
  using AMCS.Data;
  using AMCS.Data.Entity;
  using AMCS.Data.Server;
  using AMCS.Data.Server.Services;
  using Newtonsoft.Json.Linq;

  internal class EntityObjectExternalReader : EntityObjectReader<Guid>
  {
    private readonly ISessionToken user;
    private readonly Type entityType;

    public EntityObjectExternalReader(Type entityType, ISessionToken user)
      : base(entityType, user)
    {
      this.entityType = entityType;
      this.user = user;
    }

    public override EntityObject GetById(IDataSession session, ISessionToken user, Type entityType, Guid id)
    {
      return session.GetByGuid(user, entityType, id);
    }

    public override IJsonDomElement Create(JObject data)
    {
      var existingGuid = data["GUID"]?.ToObject<Guid>();
      if (existingGuid.HasValue)
      {
        using (var session = BslDataSessionFactory.GetDataSession(user, false))
        using (var transaction = session.CreateTransaction())
        {
          try
          {
            if (session.FindByGuid(user, entityType, existingGuid.Value) != null)
              return Update(existingGuid.Value, data);
          }
          catch (Exception)
          {
            // ignored
          }
        }
      }

      return base.Create(data);
    }

    public override IJsonDomElement Update(Guid id, JObject data)
    {
      var existingGuid = data["GUID"]?.ToObject<Guid>();
      if (existingGuid.HasValue && existingGuid.Value != id)
        throw BslUserExceptionFactory<BslUserException>.CreateException($"Mismatch GUIDs found; expected {id}, actual {existingGuid}");

      return base.Update(id, data);
    }

    protected override IJsonDomElement BuildIdResponse(IDataSession session, Type entityType, int? id, Guid? guid = null)
    {
      if(id != null)
      { 
        guid = DataServices.Resolve<IDataAccessIdService>().GetGuidById(session, entityType, id.Value);
      }
      
      var response = new JsonDomObject();
      response.Add("resource", guid.HasValue ? JsonDomValue.From(guid.Value.ToString()) : null);
      return response;
    }

    protected override IJsonDomElement BuildStatusResponse(IDataSession session, Type entityType, Guid? id, bool isSuccess)
    {
      var response = new JsonDomObject();

      var status = new JsonDomObject();
      response.Add("status", status);

      status.Add("id", id.HasValue ? JsonDomValue.From(id.Value.ToString()) : null);
      status.Add("isSuccess", JsonDomValue.From(isSuccess));

      return response;
    }

    protected override IJsonDomElement BuildCollectionResource(IList<EntityObject> entities, string links, string udf, IApiContext context, IDataSession session)
    {
      var array = new JsonDomArray();

      foreach (var entity in entities)
      {
        array.Add(BuildEntityResponse(entity, links, null, null, udf, context, session));
      }

      return array;
    }

    protected override int GetInternalId(IDataSession session, ISessionToken user, Type entityType, Guid id)
    {
      return session.GetByGuid(user, entityType, id).Id32;
    }

    protected override EntityLinkBuilder GetLinkBuilder(IApiContext context, EntityObject entity)
    {
      return EntityLinkExternalBuilder.ForEntity(context, entity);
    }
  }
}
