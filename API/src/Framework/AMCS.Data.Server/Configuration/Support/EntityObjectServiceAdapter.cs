using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Configuration.Mapping.Translate;
using AMCS.Data.Entity;
using AMCS.Data.Entity.History;
using AMCS.Data.Server.SQL.Querying;

namespace AMCS.Data.Server.Configuration.Support
{
  internal class EntityObjectServiceAdapter<TSource, TTarget> : IEntityObjectService<TSource>, IEntityObjectService
    where TSource : TTarget
    where TTarget : EntityObject
  {
    private readonly IEntityObjectService<TTarget> target;

    public bool UseReportingDatabase
    {
      get => target.UseReportingDatabase;
      set => target.UseReportingDatabase = value;
    }

    public EntityObjectServiceAdapter(IEntityObjectService<TTarget> target)
    {
      this.target = target;
    }

    public int? GetIntIdFromGUID(IDataSession dataSession, ISessionToken userId, Type entityType, Guid? guid)
    {
      return target.GetIntIdFromGUID(dataSession, userId, entityType, guid);
    }

    public EntityObjectTranslator GetEntityObjectTranslator(Type entityObjectType)
    {
      return target.GetEntityObjectTranslator(entityObjectType);
    }

    public BusinessObjectStringTranslator GetTranslator(Type businessStringsType)
    {
      return target.GetTranslator(businessStringsType);
    }

    public void ValidateEntity(EntityObject entity, Exception innerException = null)
    {
      target.ValidateEntity(entity, innerException);
    }

    EntityObject IEntityObjectService.GetNew(ISessionToken userId, IDataSession existingDataSession)
    {
      return ((IEntityObjectService)target).GetNew(userId, existingDataSession);
    }

    EntityObject IEntityObjectService.GetNew(ISessionToken userId, IDataSession existingDataSession, params object[] args)
    {
      return ((IEntityObjectService)target).GetNew(userId, existingDataSession, args);
    }

    EntityObject IEntityObjectService.GetNewAsCopy(ISessionToken userId, EntityObject entity)
    {
      return ((IEntityObjectService)target).GetNewAsCopy(userId, entity);
    }

    EntityObject IEntityObjectService.GetById(ISessionToken userId, int id, IDataSession existingDataSession)
    {
      return ((IEntityObjectService)target).GetById(userId, id, existingDataSession);
    }

    EntityObject IEntityObjectService.GetByGuid(ISessionToken userId, Guid guid, IDataSession existingDataSession)
    {
      return ((IEntityObjectService)target).GetByGuid(userId, guid, existingDataSession);
    }

    IList<EntityObject> IEntityObjectService.GetAllById(ISessionToken userId, int id, bool includeDeleted, IDataSession existingDataSession)
    {
      return ((IEntityObjectService)target).GetAllById(userId, id, includeDeleted, existingDataSession);
    }

    IList<EntityObject> IEntityObjectService.GetAllByParentId(ISessionToken userId, int parentId, bool includeDeleted, IDataSession existingDataSession)
    {
      return ((IEntityObjectService)target).GetAllByParentId(userId, parentId, includeDeleted, existingDataSession);
    }

    IList<EntityObject> IEntityObjectService.GetAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, IDataSession existingDataSession)
    {
      return ((IEntityObjectService)target).GetAllByTemplate(userId, template, andTemplateProperties, existingDataSession);
    }

    IList<EntityObject> IEntityObjectService.GetAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, IList<string> ignorePropertyNames, IDataSession existingDataSession)
    {
      return ((IEntityObjectService)target).GetAllByTemplate(userId, template, andTemplateProperties, ignorePropertyNames, existingDataSession);
    }

    IList<EntityObject> IEntityObjectService.GetAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, IList<string> ignorePropertyNames, IList<string> includeOnlyPropertyNames, IDataSession existingDataSession)
    {
      return ((IEntityObjectService)target).GetAllByTemplate(userId, template, andTemplateProperties, ignorePropertyNames, includeOnlyPropertyNames, existingDataSession);
    }

    int? IEntityObjectService.Save(ISessionToken userId, EntityObject entity, IDataSession existingDataSession)
    {
      return ((IEntityObjectService)target).Save(userId, entity, existingDataSession);
    }

    void IEntityObjectService.Save(ISessionToken userId, IList<EntityObject> entities, IDataSession existingDataSession)
    {
      ((IEntityObjectService)target).Save(userId, entities, existingDataSession);
    }

    void IEntityObjectService.BulkSave(ISessionToken userId, IList<EntityObject> entities, IDataSession existingDataSession)
    {
      ((IEntityObjectService)target).BulkSave(userId, entities, existingDataSession);
    }

    void IEntityObjectService.BulkSaveWithAudit(ISessionToken userId, IList<EntityObject> entities, IDataSession existingDataSession)
    {
      ((IEntityObjectService)target).BulkSaveWithAudit(userId, entities, existingDataSession);
    }

    void IEntityObjectService.Delete(ISessionToken userId, EntityObject entity, IDataSession existingDataSession)
    {
      ((IEntityObjectService)target).Delete(userId, entity, existingDataSession);
    }

    void IEntityObjectService.DeleteAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, IDataSession existingDataSession)
    {
      ((IEntityObjectService)target).DeleteAllByTemplate(userId, template, andTemplateProperties, existingDataSession);
    }

    void IEntityObjectService.UnDelete(ISessionToken userId, EntityObject entity, IDataSession existingDataSession)
    {
      ((IEntityObjectService)target).UnDelete(userId, entity, existingDataSession);
    }

    IList<EntityObject> IEntityObjectService.GetAllByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession)
    {
      return ((IEntityObjectService)target).GetAllByCriteria(userId, criteria, existingDataSession);
    }

    public ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null)
    {
      return target.GetApiCollection(userId, criteria, includeCount, existingDataSession);
    }

    public int GetCountByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession = null)
    {
      return target.GetCountByCriteria(userId, criteria, existingDataSession);
    }

    public bool GetExistsByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession = null)
    {
      if (userId == null)
        throw new ArgumentNullException(nameof(userId));
      if (criteria == null)
        throw new ArgumentNullException(nameof(criteria));

      return target.GetExistsByCriteria(userId, criteria, existingDataSession);
    }

    public void Delete(ISessionToken userId, TSource entity, IDataSession existingDataSession = null)
    {
      target.Delete(userId, entity, existingDataSession);
    }

    public void DeleteAllByTemplate(ISessionToken userId, TSource template, bool andTemplateProperties, IDataSession existingDataSession = null)
    {
      target.DeleteAllByTemplate(userId, template, andTemplateProperties, existingDataSession);
    }

    public IList<TSource> GetAllById(ISessionToken userId, int id, bool includeDeleted, IDataSession existingDataSession = null)
    {
      return target.GetAllById(userId, id, includeDeleted, existingDataSession).Cast<TSource>().ToList();
    }

    public IList<TSource> GetAllByParentId(ISessionToken userId, int parentId, bool includeDeleted, IDataSession existingDataSession = null)
    {
      return target.GetAllByParentId(userId, parentId, includeDeleted, existingDataSession).Cast<TSource>().ToList();
    }

    public IList<TSource> GetAllByTemplate(ISessionToken userId, TSource template, bool andTemplateProperties, IDataSession existingDataSession = null)
    {
      return target.GetAllByTemplate(userId, template, andTemplateProperties, existingDataSession).Cast<TSource>().ToList();
    }

    public IList<TSource> GetAllByTemplate(ISessionToken userId, TSource template, bool andTemplateProperties, IList<string> ignorePropertyNames, IDataSession existingDataSession)
    {
      return target.GetAllByTemplate(userId, template, andTemplateProperties, ignorePropertyNames, existingDataSession).Cast<TSource>().ToList();
    }

    public IList<TSource> GetAllByTemplate(ISessionToken userId, TSource template, bool andTemplateProperties, IList<string> ignorePropertyNames, IList<string> includeOnlyPropertyNames, IDataSession existingDataSession)
    {
      return target.GetAllByTemplate(userId, template, andTemplateProperties, ignorePropertyNames, includeOnlyPropertyNames, existingDataSession).Cast<TSource>().ToList();
    }

    public TSource GetByGuid(ISessionToken userId, Guid guid, IDataSession existingDataSession = null)
    {
      return (TSource)target.GetByGuid(userId, guid, existingDataSession);
    }

    public TSource GetById(ISessionToken userId, int id, IDataSession existingDataSession = null)
    {
      return (TSource)target.GetById(userId, id, existingDataSession);
    }

    public TSource GetNew(ISessionToken userId, IDataSession existingDataSession)
    {
      return (TSource)target.GetNew(userId, existingDataSession);
    }

    public TSource GetNew(ISessionToken userId, IDataSession existingDataSession, params object[] args)
    {
      return (TSource)target.GetNew(userId, existingDataSession, args);
    }

    public TSource GetNewAsCopy(ISessionToken userId, TSource entity)
    {
      return (TSource)target.GetNewAsCopy(userId, entity);
    }

    public int? Save(ISessionToken userId, TSource entity, IDataSession existingDataSession = null)
    {
      return target.Save(userId, entity, existingDataSession);
    }

    public void Save(ISessionToken userId, IList<TSource> entities, IDataSession existingDataSession = null)
    {
      target.Save(userId, entities.Cast<TTarget>().ToList(), existingDataSession);
    }

    public void UnDelete(ISessionToken userId, TSource entity, IDataSession existingDataSession = null)
    {
      target.UnDelete(userId, entity, existingDataSession);
    }

    public IList<TSource> GetAllByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession = null)
    {
      return target.GetAllByCriteria(userId, criteria, existingDataSession).Cast<TSource>().ToList();
    }

    public IList<EntityHistory> GetAllHistoryByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession = null)
    {
      return target.GetAllHistoryByCriteria(userId, criteria, existingDataSession);
    }
  }
}
