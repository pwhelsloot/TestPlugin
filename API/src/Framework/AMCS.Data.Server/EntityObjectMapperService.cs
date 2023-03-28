using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using AMCS.Data.Entity.History;
using AMCS.Data.Server.SQL.Querying;

namespace AMCS.Data.Server
{
  public abstract class EntityObjectMapperService<TSource, TTarget> : TranslatableService, IEntityObjectService<TSource>
    where TSource : EntityObject
    where TTarget : EntityObject
  {
    protected IEntityObjectService<TTarget> Target { get; }
    protected IEntityObjectMapper Mapper { get; }

    protected EntityObjectMapperService(IEntityObjectService<TTarget> target, IEntityObjectMapper mapper)
    {
      Target = target;
      Mapper = mapper;
    }

    protected virtual TSource Map(TTarget target)
    {
      return Mapper.Map<TSource>(target);
    }

    protected virtual TTarget Map(TSource source)
    {
      return Mapper.Map<TTarget>(source);
    }

    protected virtual IList<TSource> MapList(IList<TTarget> target)
    {
      return Mapper.MapList<TSource>(target);
    }

    protected virtual IList<TTarget> MapList(IList<TSource> source)
    {
      return Mapper.MapList<TTarget>(source);
    }

    EntityObject IEntityObjectService.GetNew(ISessionToken userId, IDataSession existingDataSession)
    {
      return GetNew(userId, existingDataSession);
    }

    public virtual TSource GetNew(ISessionToken userId, IDataSession existingDataSession, params object[] args)
    {
      return Map(Target.GetNew(userId, existingDataSession, args));
    }

    public virtual TSource GetNewAsCopy(ISessionToken userId, TSource entity)
    {
      return Map(Target.GetNewAsCopy(userId, Map(entity)));
    }

    public virtual TSource GetById(ISessionToken userId, int id, IDataSession existingDataSession = null)
    {
      return Map(Target.GetById(userId, id, existingDataSession));
    }

    public virtual TSource GetByGuid(ISessionToken userId, Guid guid, IDataSession existingDataSession = null)
    {
      return Map(Target.GetByGuid(userId, guid, existingDataSession));
    }

    public virtual IList<TSource> GetAllById(ISessionToken userId, int id, bool includeDeleted, IDataSession existingDataSession)
    {
      return MapList(Target.GetAllById(userId, id, includeDeleted, existingDataSession));
    }

    public virtual IList<TSource> GetAllByParentId(ISessionToken userId, int parentId, bool includeDeleted, IDataSession existingDataSession)
    {
      return MapList(Target.GetAllByParentId(userId, parentId, includeDeleted, existingDataSession));
    }

    public virtual IList<TSource> GetAllByTemplate(ISessionToken userId, TSource template, bool andTemplateProperties, IDataSession existingDataSession = null)
    {
      return MapList(Target.GetAllByTemplate(userId, Map(template), andTemplateProperties, existingDataSession));
    }

    public virtual IList<TSource> GetAllByTemplate(ISessionToken userId, TSource template, bool andTemplateProperties, IList<string> ignorePropertyNames, IDataSession existingDataSession = null)
    {
      return MapList(Target.GetAllByTemplate(userId, Map(template), andTemplateProperties, ignorePropertyNames, existingDataSession));
    }

    public virtual IList<TSource> GetAllByTemplate(ISessionToken userId, TSource template, bool andTemplateProperties, IList<string> ignorePropertyNames, IList<string> includeOnlyPropertyNames, IDataSession existingDataSession = null)
    {
      return MapList(Target.GetAllByTemplate(userId, Map(template), andTemplateProperties, ignorePropertyNames, includeOnlyPropertyNames, existingDataSession));
    }

    public virtual int? Save(ISessionToken userId, TSource entity, IDataSession existingDataSession = null)
    {
      return Target.Save(userId, Map(entity), existingDataSession);
    }

    public virtual void Save(ISessionToken userId, IList<TSource> entities, IDataSession existingDataSession = null)
    {
      Target.Save(userId, MapList(entities), existingDataSession);
    }

    public virtual void Delete(ISessionToken userId, TSource entity, IDataSession existingDataSession = null)
    {
      Target.Delete(userId, Map(entity), existingDataSession);
    }

    public virtual void DeleteAllByTemplate(ISessionToken userId, TSource template, bool andTemplateProperties, IDataSession existingDataSession = null)
    {
      Target.DeleteAllByTemplate(userId, Map(template), andTemplateProperties, existingDataSession);
    }

    public virtual void UnDelete(ISessionToken userId, TSource entity, IDataSession existingDataSession = null)
    {
      Target.UnDelete(userId, Map(entity), existingDataSession);
    }

    public virtual IList<TSource> GetAllByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession)
    {
      return MapList(Target.GetAllByCriteria(userId, criteria, existingDataSession));
    }

    public virtual TSource GetNew(ISessionToken userId, IDataSession existingDataSession)
    {
      return Map(Target.GetNew(userId, existingDataSession));
    }

    EntityObject IEntityObjectService.GetNew(ISessionToken userId, IDataSession existingDataSession, params object[] args)
    {
      return GetNew(userId, existingDataSession, args);
    }

    EntityObject IEntityObjectService.GetNewAsCopy(ISessionToken userId, EntityObject entity)
    {
      return GetNewAsCopy(userId, (TSource)entity);
    }

    EntityObject IEntityObjectService.GetById(ISessionToken userId, int id, IDataSession existingDataSession)
    {
      return GetById(userId, id, existingDataSession);
    }

    EntityObject IEntityObjectService.GetByGuid(ISessionToken userId, Guid guid, IDataSession existingDataSession)
    {
      return GetByGuid(userId, guid, existingDataSession);
    }

    IList<EntityObject> IEntityObjectService.GetAllById(ISessionToken userId, int id, bool includeDeleted, IDataSession existingDataSession)
    {
      return GetAllById(userId, id, includeDeleted, existingDataSession).ToList<EntityObject>();
    }

    IList<EntityObject> IEntityObjectService.GetAllByParentId(ISessionToken userId, int parentId, bool includeDeleted, IDataSession existingDataSession)
    {
      return GetAllByParentId(userId, parentId, includeDeleted, existingDataSession).ToList<EntityObject>();
    }

    IList<EntityObject> IEntityObjectService.GetAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, IDataSession existingDataSession)
    {
      return GetAllByTemplate(userId, (TSource)template, andTemplateProperties, existingDataSession).ToList<EntityObject>();
    }

    IList<EntityObject> IEntityObjectService.GetAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, IList<string> ignorePropertyNames, IDataSession existingDataSession)
    {
      return GetAllByTemplate(userId, (TSource)template, andTemplateProperties, ignorePropertyNames, existingDataSession).ToList<EntityObject>();
    }

    IList<EntityObject> IEntityObjectService.GetAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, IList<string> ignorePropertyNames, IList<string> includeOnlyPropertyNames, IDataSession existingDataSession)
    {
      return GetAllByTemplate(userId, (TSource)template, andTemplateProperties, ignorePropertyNames, includeOnlyPropertyNames, existingDataSession).ToList<EntityObject>();
    }

    int? IEntityObjectService.Save(ISessionToken userId, EntityObject entity, IDataSession existingDataSession)
    {
      return Save(userId, (TSource)entity, existingDataSession);
    }

    void IEntityObjectService.Save(ISessionToken userId, IList<EntityObject> entities, IDataSession existingDataSession)
    {
      Save(userId, entities.Cast<TSource>().ToList(), existingDataSession);
    }

    public virtual void BulkSave(ISessionToken userId, IList<EntityObject> entities, IDataSession existingDataSession = null)
    {
      Target.BulkSave(userId, entities, existingDataSession);
    }

    public virtual void BulkSaveWithAudit(ISessionToken userId, IList<EntityObject> entities, IDataSession existingDataSession = null)
    {
      Target.BulkSaveWithAudit(userId, entities, existingDataSession);
    }

    void IEntityObjectService.Delete(ISessionToken userId, EntityObject entity, IDataSession existingDataSession)
    {
      Delete(userId, (TSource)entity, existingDataSession);
    }

    void IEntityObjectService.DeleteAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, IDataSession existingDataSession)
    {
      DeleteAllByTemplate(userId, (TSource)template, andTemplateProperties, existingDataSession);
    }

    void IEntityObjectService.UnDelete(ISessionToken userId, EntityObject entity, IDataSession existingDataSession)
    {
      UnDelete(userId, (TSource)entity, existingDataSession);
    }

    public virtual ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null)
    {
      var query = Target.GetApiCollection(userId, criteria, includeCount, existingDataSession);

      return new ApiQuery(
        query.Entities
          .Cast<TTarget>()
          .Select(Map)
          .ToList<EntityObject>(),
        query.Count
      );
    }

    IList<EntityObject> IEntityObjectService.GetAllByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession)
    {
      return GetAllByCriteria(userId, criteria, existingDataSession).ToList<EntityObject>();
    }

    public virtual IList<EntityHistory> GetAllHistoryByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession = null)
    {
      return Target.GetAllHistoryByCriteria(userId, criteria, existingDataSession);
    }

    public virtual int GetCountByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession = null)
    {
      return Target.GetCountByCriteria(userId, criteria, existingDataSession);
    }

    public bool GetExistsByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession = null)
    {
      return Target.GetExistsByCriteria(userId, criteria, existingDataSession);
    }
  }
}
