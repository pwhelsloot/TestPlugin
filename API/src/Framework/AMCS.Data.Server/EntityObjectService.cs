// //-----------------------------------------------------------------------------
// // <copyright file="EntityObjectService.cs" company="AMCS Group">
// //   Copyright © 2010-12 AMCS Group. All rights reserved.
// // </copyright>
// // 
// // PROJECT: P142 - Elemos
// //
// // AMCS Elemos Project
// //
// //----------------------------------------------------------------------------- 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using AMCS.Data.Entity;
using AMCS.Data.Entity.History;
using AMCS.Data.Entity.Interfaces;
using AMCS.Data.Server.Services;
using AMCS.Data.Server.SQL.Querying;
using AMCS.Data.Support.Security;

namespace AMCS.Data.Server
{
  /// <summary>
  /// Entity Object Service.
  /// </summary>
  /// <typeparam name="T">The type of Entity.</typeparam>
  public class EntityObjectService<T> : TranslatableService, IEntityObjectService<T>
    where T : EntityObject
  {
    private static readonly object[] EmptyArguments = new object[0];

    #region Properties

    private const string cacheString = "EntityObjectServiceCache";
    private static readonly object bulkSaveLock = new object();

    private IEntityObjectAccess<T> dataAccess;

    public EntityObjectService()
    {
      if (StrictMode.IsRequireTransaction)
        throw new StrictModeException("Parameter less constructor on entity object service cannot be used");
    }

    public EntityObjectService(IEntityObjectAccess<T> dataAccess)
    {
      this.dataAccess = dataAccess;
    }

    /// <summary>
    /// Gets or sets the data access.
    /// </summary>
    /// <value>
    /// The data access.
    /// </value>
    protected IEntityObjectAccess<T> DataAccess
    {
      get
      {
        // We delay resolve the data access because there are unit tests that don't have
        // a database, that are using these object services.
        if (dataAccess == null)
          dataAccess = DataServices.Resolve<IEntityObjectAccess<T>>();
        return dataAccess;
      }
    }

    #endregion Properties

    #region Methods

    #region IEntityObjectService Members

    /// <summary>
    /// Deletes the specified user identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    void IEntityObjectService.Delete(ISessionToken userId, EntityObject entity, IDataSession existingDataSession)
    {
      this.Delete(userId, (T)entity, existingDataSession);
    }

    /// <summary>
    /// Deletes all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    void IEntityObjectService.DeleteAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, IDataSession existingDataSession)
    {
      this.DeleteAllByTemplate(userId, (T)template, andTemplateProperties, existingDataSession);
    }

    /// <summary>
    /// Gets all by identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    IList<EntityObject> IEntityObjectService.GetAllById(ISessionToken userId, int id, bool includeDeleted, IDataSession existingDataSession)
    {
      return this.GetAllById(userId, id, includeDeleted, existingDataSession).ToList<EntityObject>();
    }

    /// <summary>
    /// Gets all by parent identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="parentId">The parent identifier.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    IList<EntityObject> IEntityObjectService.GetAllByParentId(ISessionToken userId, int parentId, bool includeDeleted, IDataSession existingDataSession)
    {
      return this.GetAllByParentId(userId, parentId, includeDeleted, existingDataSession).ToList<EntityObject>();
    }

    /// <summary>
    /// Gets all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    IList<EntityObject> IEntityObjectService.GetAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, IDataSession existingDataSession)
    {
      return this.GetAllByTemplate(userId, (T)template, andTemplateProperties, existingDataSession).ToList<EntityObject>();
    }

    /// <summary>
    /// Gets all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="ignorePropertyNames">The ignore property names.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    IList<EntityObject> IEntityObjectService.GetAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, IList<string> ignorePropertyNames, IDataSession existingDataSession)
    {
      return this.GetAllByTemplate(userId, (T)template, andTemplateProperties, ignorePropertyNames, null, existingDataSession).ToList<EntityObject>();
    }

    /// <summary>
    /// Gets all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="ignorePropertyNames">The ignore property names.</param>
    /// <param name="includeOnlyPropertyNames">The include only property names.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    IList<EntityObject> IEntityObjectService.GetAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, IList<string> ignorePropertyNames, IList<string> includeOnlyPropertyNames, IDataSession existingDataSession)
    {
      return this.GetAllByTemplate(userId, (T)template, andTemplateProperties, ignorePropertyNames, includeOnlyPropertyNames, existingDataSession).ToList<EntityObject>();
    }

    /// <summary>
    /// Gets the by unique identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="guid">The unique identifier.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    EntityObject IEntityObjectService.GetByGuid(ISessionToken userId, Guid guid, IDataSession existingDataSession)
    {
      EntityObject entity = this.GetByGuid(userId, guid, existingDataSession);
      return entity;
    }

    /// <summary>
    /// Gets the by identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    EntityObject IEntityObjectService.GetById(ISessionToken userId, int id, IDataSession existingDataSession)
    {
      EntityObject entity = this.GetById(userId, id, existingDataSession);
      return entity;
    }

    /// <summary>
    /// Gets the new.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns></returns>
    EntityObject IEntityObjectService.GetNew(ISessionToken userId, IDataSession existingDataSession)
    {
      return this.GetNew(userId, existingDataSession, EmptyArguments);
    }

    /// <summary>
    /// Gets the new.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="args">The arguments.</param>
    /// <returns></returns>
    EntityObject IEntityObjectService.GetNew(ISessionToken userId, IDataSession existingDataSession, params object[] args)
    {
      return this.GetNew(userId, existingDataSession, args);
    }

    /// <summary>
    /// Gets the new as copy.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entity">The entity.</param>
    /// <returns></returns>
    EntityObject IEntityObjectService.GetNewAsCopy(ISessionToken userId, EntityObject entity)
    {
      return this.GetNewAsCopy(userId, (T)entity);
    }

    /// <summary>
    /// Saves the specified user identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    int? IEntityObjectService.Save(ISessionToken userId, EntityObject entity, IDataSession existingDataSession)
    {
      return this.Save(userId, (T)entity, existingDataSession);
    }

    /// <summary>
    /// Saves the specified user identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entities">The entities.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    void IEntityObjectService.Save(ISessionToken userId, IList<EntityObject> entities, IDataSession existingDataSession)
    {
      List<T> typedEntities = new List<T>();
      foreach (T entity in entities)
      {
        typedEntities.Add(entity);
      }
      this.Save(userId, typedEntities, existingDataSession);
    }

    /// <summary>
    /// Bulk saves the list of new entities
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entities">The entities.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    void IEntityObjectService.BulkSave(ISessionToken userId, IList<EntityObject> entities, IDataSession existingDataSession)
    {
      List<T> typedEntities = new List<T>();
      foreach (T entity in entities)
      {
        typedEntities.Add(entity);
      }
      this.BulkSave(userId, typedEntities, existingDataSession);
    }

    /// <summary>
    /// Bulk saves the list of new entities with audit
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entities">The entities.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    void IEntityObjectService.BulkSaveWithAudit(ISessionToken userId, IList<EntityObject> entities, IDataSession existingDataSession)
    {
      List<T> typedEntities = new List<T>();
      foreach (T entity in entities)
      {
        typedEntities.Add(entity);
      }
      this.BulkSaveWithAudit(userId, typedEntities, existingDataSession);
    }

    /// <summary>
    /// Undo the delete.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    void IEntityObjectService.UnDelete(ISessionToken userId, EntityObject entity, IDataSession existingDataSession)
    {
      this.UnDelete(userId, (T)entity, existingDataSession);
    }

    /// <summary>
    /// Executes a filtered collection query for the API.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="criteria">The criteria to filter by.</param>
    /// <param name="includeCount">Whether to include a count based on the criteria in the response.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns>The results of the executed query.</returns>
    public virtual ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        int? count = null;
        if (includeCount)
          count = GetCountByCriteria(userId, criteria.Clone(), ds);

        var entities = ((IEntityObjectService)this).GetAllByCriteria(userId, criteria, ds);

        return new ApiQuery(entities, count);
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    IList<EntityObject> IEntityObjectService.GetAllByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        return DataAccess.GetByCriteria(ds, userId, criteria, CriteriaQueryType.Select)
          .List<EntityObject>(typeof(T));
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    public int GetCountByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        return DataAccess.GetByCriteria(ds, userId, criteria, CriteriaQueryType.Count)
          .SingleScalar<int>();
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    public virtual bool GetExistsByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession = null)
    {
      if (userId == null)
        throw new ArgumentNullException(nameof(existingDataSession));
      if (criteria == null)
        throw new ArgumentNullException(nameof(userId));
      if (existingDataSession == null)
        throw new ArgumentNullException(nameof(criteria));

      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
            return DataAccess.GetByCriteria(ds, userId, criteria, CriteriaQueryType.Exists).SingleOrDefaultScalar<object>() != null;
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    IList<EntityHistory> IEntityObjectService.GetAllHistoryByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        return DataAccess.GetAllHistoryByCriteria(ds, userId, criteria);
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    #endregion IEntityObjectService Members

    #region IEntityObjectService<T> Members

    /// <summary>
    ///   Deletes the specified user id.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    public virtual void Delete(ISessionToken userId, T entity, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        DataAccess.Delete(ds, userId, entity, false);
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    /// <summary>
    ///   Deletes all by template.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    public virtual void DeleteAllByTemplate(ISessionToken userId, T template, bool andTemplateProperties, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        // perform a get and then delete so that auditing can be performed.
        IList<T> entities = DataAccess.GetAllByTemplate(ds, userId, template, andTemplateProperties);
        foreach (T entity in entities)
        {
          Delete(userId, entity, ds);
        }
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    /// <summary>
    /// Gets all by identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public virtual IList<T> GetAllById(ISessionToken userId, int id, bool includeDeleted, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        IList<T> entities = DataAccess.GetAllById(ds, userId, id, includeDeleted);
        return entities;
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    /// <summary>
    /// Gets all by parent identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="parentId">The parent identifier.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public virtual IList<T> GetAllByParentId(ISessionToken userId, int parentId, bool includeDeleted, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        IList<T> entities = DataAccess.GetAllByParentId(ds, userId, parentId, includeDeleted);
        return entities;
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    /// <summary>
    /// Gets all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public virtual IList<T> GetAllByTemplate(ISessionToken userId, T template, bool andTemplateProperties, IDataSession existingDataSession = null)
    {
      return GetAllByTemplate(userId, template, andTemplateProperties, null, existingDataSession);
    }

    /// <summary>
    /// Gets all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="ignorePropertyNames">The ignore property names.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public IList<T> GetAllByTemplate(ISessionToken userId, T template, bool andTemplateProperties, IList<string> ignorePropertyNames, IDataSession existingDataSession)
    {
      return GetAllByTemplate(userId, template, andTemplateProperties, ignorePropertyNames, null, existingDataSession);
    }

    /// <summary>
    /// Gets all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="ignorePropertyNames">The ignore property names.</param>
    /// <param name="includeOnlyPropertyNames">The include only property names.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public IList<T> GetAllByTemplate(ISessionToken userId, T template, bool andTemplateProperties, IList<string> ignorePropertyNames, IList<string> includeOnlyPropertyNames, IDataSession existingDataSession)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        IList<T> entities = DataAccess.GetAllByTemplate(ds, userId, template, andTemplateProperties, ignorePropertyNames, includeOnlyPropertyNames);
        return entities;
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    /// <summary>
    /// Gets the by unique identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="guid">The unique identifier.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public virtual T GetByGuid(ISessionToken userId, Guid guid, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        T entity = DataAccess.GetByGuid(ds, userId, guid);
        return entity;
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    /// <summary>
    /// Gets the by identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public virtual T GetById(ISessionToken userId, int id, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        T entity = DataAccess.GetById(ds, userId, id);
        return entity;
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    /// <summary>
    /// Gets the data access.
    /// </summary>
    /// <returns></returns>
    public IEntityObjectAccess<T> GetDataAccess()
    {
      return DataAccessManager.GetAccessForEntity<T>();
    }

    /// <summary>
    /// Gets the new. Should never be virtual/abstract.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns></returns>
    public T GetNew(ISessionToken userId, IDataSession existingDataSession)
    {
      return GetNew(userId, existingDataSession, EmptyArguments);
    }

    /// <summary>
    /// Gets the new. Should never be virtual/abstract.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="args">The arguments.</param>
    /// <returns></returns>
    public T GetNew(ISessionToken userId, IDataSession existingDataSession, params object[] args)
    {
      T entity = this.EnhancedGetNew(userId, existingDataSession, args);
      string entityName = entity.GetType().FullName;
      SetEntityDefaults(userId, entity, entityName, existingDataSession);
      return entity;
    }

    /// <summary>
    /// Sets any entity defaults from the entityproperty table.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="entityName"></param>
    private void SetEntityDefaults(ISessionToken userId, T entity, string entityName, IDataSession existingDataSession)
    {
      // Get entityEntity
      EntityEntity entityEntity = existingDataSession.GetAllByTemplate<EntityEntity>(userId, new EntityEntity() { ClassName = entityName, EntityProperties = new List<EntityPropertyEntity>() }, true).FirstOrDefault();
      
      if (entityEntity?.EntityId != null)
      {
        // Get Properties
        PropertyInfo[] props = entity.GetType().GetProperties();
        IList<EntityPropertyEntity> properties = existingDataSession.GetAllByTemplate<EntityPropertyEntity>(userId, new EntityPropertyEntity() { EntityId = entityEntity.EntityId }, true);
        
        // Do work
        if (properties != null)
        {
          foreach (EntityPropertyEntity property in properties.Where(x => !string.IsNullOrWhiteSpace(x.DefaultValue)))
          {
            PropertyInfo prop = props.FirstOrDefault(x => x.Name.Equals(property.PropertyName));
            if (prop != null && prop.GetSetMethod() != null && !prop.PropertyType.FullName.Contains("ObservableCollection"))
            {
              object propValue = null;
              try
              {
                TypeConverter typeConverter = TypeDescriptor.GetConverter(prop.PropertyType);
                propValue = typeConverter.ConvertFromString(property.DefaultValue);
                prop.SetValue(entity, propValue, null);
              }
              catch (Exception)
              {
                continue;
              }
            }
          }
        }
      }
    }

    private string GetEntityEntityCacheString(string entityName)
    {
      return cacheString + ':' + entityName;
    }

    private string GetEntityPropertyCacheString(string entityName, int entityId)
    {
      return cacheString + ":[]" + entityName + entityId.ToString();
    }

    /// <summary>
    /// The updated overridable version of NewGet. This replaced 'GetNew(string userId, params object[] args)' as we needed a way to always run 'SetEntityDefaults' whether
    /// the override method existed or called its base method.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    protected virtual T EnhancedGetNew(ISessionToken userId, IDataSession existingDataSession, params object[] args)
    {
      return (T)Activator.CreateInstance(typeof(T));
    }

    /// <summary>
    /// Gets the new as copy.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entity">The entity.</param>
    /// <returns></returns>
    public virtual T GetNewAsCopy(ISessionToken userId, T entity)
    {
      return (T)entity.CloneWithoutKeys();
    }

    /// <summary>
    /// Saves the specified user identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public virtual int? Save(ISessionToken userId, T entity, IDataSession existingDataSession = null)
    {
      ValidateEntity(entity);
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        if (existingDataSession == null)
          ds.StartTransaction();
        try
        {
          int? id = DataAccess.Save(ds, userId, entity);

          if (existingDataSession == null)
            ds.CommitTransaction();

          return id;
        }
        catch
        {
          if (existingDataSession == null)
            ds.RollbackTransaction();
          throw;
        }
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    /// <summary>
    /// Saves the specified user identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entities">The entities.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <exception cref="System.Exception">System Error: Request to save entities of type  + typeof(T).FullName +  but 'null' entities supplied.</exception>
    public virtual void Save(ISessionToken userId, IList<T> entities, IDataSession existingDataSession = null)
    {
      if (entities == null)
        throw new Exception("System Error: Request to save entities of type " + typeof(T).FullName + " but 'null' entities supplied.");

      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        if (existingDataSession == null)
          ds.StartTransaction();
        try
        {
          foreach (T entity in entities)
          {
            Save(userId, entity, ds);
          }

          if (existingDataSession == null)
            ds.CommitTransaction();
        }
        catch
        {
          if (existingDataSession == null)
            ds.RollbackTransaction();
          throw;
        }
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    public virtual void BulkSave(ISessionToken userId, IList<T> entities, IDataSession existingDataSession = null)
    {
      foreach (var entity in entities)
      {
        ValidateEntity(entity);
      }
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        if (existingDataSession == null)
          ds.StartTransaction();
        try
        {
          int minQuantityForBulkSave = DataServices.Resolve<ISettingsService>().GetInteger("MinQuantityForBulkSave", 10);

          if (minQuantityForBulkSave != 0 && entities.Count >= minQuantityForBulkSave)
          {
            lock (bulkSaveLock)
            {
              DataAccess.BulkSave(ds, userId, entities);
            }
          }
          else
          {
            this.Save(userId, entities, ds);
          }

          if (existingDataSession == null)
            ds.CommitTransaction();
        }
        catch
        {
          if (existingDataSession == null)
            ds.RollbackTransaction();
          throw;
        }
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    public virtual void BulkSaveWithAudit(ISessionToken userId, IList<T> entities, IDataSession existingDataSession = null)
    {
      foreach (var entity in entities)
      {
        ValidateEntity(entity);
      }
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        if (existingDataSession == null)
          ds.StartTransaction();
        try
        {
          int minQuantityForBulkSave = DataServices.Resolve<ISettingsService>().GetInteger("MinQuantityForBulkSave", 10);

          if (minQuantityForBulkSave != 0 && entities.Count >= minQuantityForBulkSave)
          {
            lock (bulkSaveLock)
            {
              DataAccess.BulkSaveWithAudit(ds, userId, entities);
            }
          }
          else
          {
            this.Save(userId, entities, ds);
          }

          if (existingDataSession == null)
            ds.CommitTransaction();
        }
        catch
        {
          if (existingDataSession == null)
            ds.RollbackTransaction();
          throw;
        }
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    /// <summary>
    /// Undo the delete.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    public virtual void UnDelete(ISessionToken userId, T entity, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        DataAccess.Delete(ds, userId, entity, true);
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    public virtual IList<T> GetAllByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        return DataAccess.GetByCriteria(ds, userId, criteria, CriteriaQueryType.Select)
          .List<T>();
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    public virtual IList<EntityHistory> GetAllHistoryByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);
      try
      {
        return DataAccess.GetAllHistoryByCriteria(ds, userId, criteria);
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    #endregion IEntityObjectService<T> Members

    public static void EncryptProperties<TEncryptable>(TEncryptable entity) where TEncryptable : IEncryptableEntity
    {
      entity.IsEncrypted = true;
      if (entity.EncryptedProperties != null)
      {
        foreach (string propertyName in entity.EncryptedProperties.Keys)
        {
          PropertyInfo property = typeof(TEncryptable).GetProperty(propertyName);
          if (property.PropertyType == typeof(string) && entity.EncryptedProperties[propertyName])
          {
            string value = (string)property.GetValue(entity, null);
            property.SetValue(entity, StringEncryptor.DefaultEncryptor.Encrypt(value), null);
          }
        }
      }
    }

    #endregion Methods
  }
}