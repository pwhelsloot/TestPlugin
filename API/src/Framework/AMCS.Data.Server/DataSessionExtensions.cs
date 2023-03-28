// -----------------------------------------------------------------------------
// <copyright file="BusinessServiceManager.cs" company="AMCS Group">
//   Copyright © 2010-12 AMCS Group. All rights reserved.
// </copyright>
//
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
// -----------------------------------------------------------------------------

using AMCS.Data.Server.BslTriggers;

#pragma warning disable CS0618

namespace AMCS.Data.Server
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.IO;
  using AMCS.Data;
  using AMCS.Data.Entity;
  using AMCS.Data.Entity.History;
  using AMCS.Data.Server.Broadcast;
  using AMCS.Data.Server.SQL;
  using AMCS.Data.Server.SQL.Parallel;
  using AMCS.Data.Server.SQL.Querying;
  using static Dapper.SqlMapper;

  /// <summary>
  /// The Business Service Manager.
  /// </summary>
  public static class DataSessionExtensions
  {
    private static readonly object[] EmptyArguments = new object[0];

    /// <summary>
    ///   Gets a new entity.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static T GetNew<T>(this IDataSession existingDataSession, ISessionToken userId)
      where T : EntityObject
    {
      return GetNew<T>(existingDataSession, userId, EmptyArguments);
    }

    /// <summary>
    ///   Gets a new entity.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="args">The args.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static T GetNew<T>(this IDataSession existingDataSession, ISessionToken userId, params object[] args)
      where T : EntityObject
    {
      return BusinessServiceManager.GetNew<T>(userId, existingDataSession, args);
    }

    /// <summary>
    ///   Gets a new entity.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static EntityObject GetNew(this IDataSession existingDataSession, ISessionToken userId, Type entityType)
    {
      return GetNew(existingDataSession, userId, entityType, EmptyArguments);
    }

    /// <summary>
    ///   Gets a new entity.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="args">The args.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static EntityObject GetNew(this IDataSession existingDataSession, ISessionToken userId, Type entityType, params object[] args)
    {
      return BusinessServiceManager.GetNew(userId, entityType, existingDataSession, args);
    }

    /// <summary>
    /// Gets the new as copy.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static EntityObject GetNewAsCopy(this IDataSession existingDataSession, ISessionToken userId, EntityObject entity)
    {
      return BusinessServiceManager.GetNewAsCopy(userId, entity, existingDataSession);
    }

    /// <summary>
    ///   Deletes the specified entity by id.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="id">The id.</param>
    /// <param name="isUndelete">if set to <c>true</c> [is undelete].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    public static void Delete<T>(this IDataSession existingDataSession, ISessionToken userId, int id, bool isUndelete)
        where T : EntityObject
    {
      BusinessServiceManager.Delete<T>(userId, id, isUndelete, existingDataSession);
    }

    /// <summary>
    ///   Deletes the specified entity.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="id">The id.</param>
    /// <param name="isUndelete">if set to <c>true</c> [is undelete].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    public static void Delete(this IDataSession existingDataSession, ISessionToken userId, Type entityType, int id, bool isUndelete)
    {
      BusinessServiceManager.Delete(userId, entityType, id, isUndelete, existingDataSession);
    }

    /// <summary>
    ///   Deletes the specified entity.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="isUndelete">if set to <c>true</c> [is undelete].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    public static void Delete(this IDataSession existingDataSession, ISessionToken userId, EntityObject entity, bool isUndelete)
    {
      BusinessServiceManager.Delete(userId, entity, isUndelete, existingDataSession);
    }

    /// <summary>
    /// Deletes all by template.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    public static void DeleteAllByTemplate<T>(this IDataSession existingDataSession, ISessionToken userId, T template, bool andTemplateProperties)
        where T : EntityObject
    {
      BusinessServiceManager.DeleteAllByTemplate<T>(userId, template, andTemplateProperties, existingDataSession);
    }

    /// <summary>
    /// Deletes all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    public static void DeleteAllByTemplate(this IDataSession existingDataSession, ISessionToken userId, EntityObject template, bool andTemplateProperties)
    {
      BusinessServiceManager.DeleteAllByTemplate(userId, template, andTemplateProperties, existingDataSession);
    }

    /// <summary>
    ///   Gets all entities.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static IList<T> GetAll<T>(this IDataSession existingDataSession, ISessionToken userId, bool includeDeleted)
        where T : EntityObject
    {
      return BusinessServiceManager.GetAll<T>(userId, includeDeleted, existingDataSession);
    }

    /// <summary>
    ///   Gets all entities.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static IList<EntityObject> GetAll(this IDataSession existingDataSession, ISessionToken userId, Type entityType, bool includeDeleted)
    {
      return BusinessServiceManager.GetAll(userId, entityType, includeDeleted, existingDataSession);
    }

    /// <summary>
    ///   Gets all entities, always including the one specified by id (even if it's deleted).
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="id">The id.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static IList<T> GetAllById<T>(this IDataSession existingDataSession, ISessionToken userId, int id, bool includeDeleted)
        where T : EntityObject
    {
      return BusinessServiceManager.GetAllById<T>(userId, id, includeDeleted, existingDataSession);
    }

    /// <summary>
    ///   Gets all entities, always including the one specified by id (even if it's deleted).
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="id">The id.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static IList<EntityObject> GetAllById(this IDataSession existingDataSession, ISessionToken userId, Type entityType, int id, bool includeDeleted)
    {
      return BusinessServiceManager.GetAllById(userId, entityType, id, includeDeleted, existingDataSession);
    }

    /// <summary>
    ///   Gets all by parent entity id.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="parentId">The parent id.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static IList<T> GetAllByParentId<T>(this IDataSession existingDataSession, ISessionToken userId, int parentId, bool includeDeleted)
        where T : EntityObject
    {
      return BusinessServiceManager.GetAllByParentId<T>(userId, parentId, includeDeleted, existingDataSession);
    }

    /// <summary>
    ///   Gets all by parent entity id.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="parentId">The parent id.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static IList<EntityObject> GetAllByParentId(this IDataSession existingDataSession, ISessionToken userId, Type entityType, int parentId, bool includeDeleted)
    {
      return BusinessServiceManager.GetAllByParentId(userId, entityType, parentId, includeDeleted, existingDataSession);
    }

    /// <summary>
    /// Gets a single entity by template.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="template"></param>
    /// <param name="andTemplateProperties"></param>
    /// <param name="throwExceptionIfNotFound"></param>
    /// <param name="existingDataSession"></param>
    /// <returns></returns>
    public static EntityObject GetByTemplate(this IDataSession existingDataSession, ISessionToken userId, EntityObject template, bool andTemplateProperties, bool throwExceptionIfNotFound)
    {
      return BusinessServiceManager.GetByTemplate(userId, template, andTemplateProperties, throwExceptionIfNotFound, existingDataSession);
    }

    /// <summary>
    /// Gets a single entity by template.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static T GetByTemplate<T>(this IDataSession existingDataSession, ISessionToken userId, T template, bool andTemplateProperties, bool throwExceptionIfNotFound)
        where T : EntityObject
    {
      return BusinessServiceManager.GetByTemplate<T>(userId, template, andTemplateProperties, throwExceptionIfNotFound, existingDataSession);
    }

    /// <summary>
    /// Gets all by template.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="ignorePropertyNames">The ignore property names.</param>
    /// <param name="includeOnlyPropertyNames">The include only property names.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static IList<T> GetAllByTemplate<T>(this IDataSession existingDataSession, ISessionToken userId, T template, bool andTemplateProperties, IList<string> ignorePropertyNames, IList<string> includeOnlyPropertyNames)
        where T : EntityObject
    {
      return BusinessServiceManager.GetAllByTemplate(userId, template, andTemplateProperties, ignorePropertyNames, includeOnlyPropertyNames, existingDataSession);
    }

    /// <summary>
    /// Gets all by template.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static IList<T> GetAllByTemplate<T>(this IDataSession existingDataSession, ISessionToken userId, T template, bool andTemplateProperties)
      where T : EntityObject
    {
      return BusinessServiceManager.GetAllByTemplate<T>(userId, template, andTemplateProperties, existingDataSession);
    }

    /// <summary>
    /// Gets all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static IList<EntityObject> GetAllByTemplate(this IDataSession existingDataSession, ISessionToken userId, EntityObject template, bool andTemplateProperties)
    {
      return BusinessServiceManager.GetAllByTemplate(userId, template, andTemplateProperties, existingDataSession);
    }

    /// <summary>
    ///   Gets the entity by Guid.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="guid">The guid.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static T GetByGuid<T>(this IDataSession existingDataSession, ISessionToken userId, Guid guid)
        where T : EntityObject
    {
      return BusinessServiceManager.GetByGuid<T>(userId, guid, existingDataSession);
    }

    /// <summary>
    /// Gets the entity by Guid
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="guid">The unique identifier.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static EntityObject GetByGuid(this IDataSession existingDataSession, ISessionToken userId, Type entityType, Guid guid)
    {
      return BusinessServiceManager.GetByGuid(userId, entityType, guid, existingDataSession);
    }

    /// <summary>
    ///   Gets the entity by Guid.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="guid">The guid.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static T FindByGuid<T>(this IDataSession existingDataSession, ISessionToken userId, Guid guid)
      where T : EntityObject
    {
      try
      {
        return existingDataSession.GetByGuid<T>(userId, guid);
      }
      catch (EntityRecordNotFoundException)
      {
        return null;
      }
    }

    /// <summary>
    /// Gets the entity by Guid
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="guid">The unique identifier.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static EntityObject FindByGuid(this IDataSession existingDataSession, ISessionToken userId, Type entityType, Guid guid)
    {
      try
      {
        return existingDataSession.GetByGuid(userId, entityType, guid);
      }
      catch (EntityRecordNotFoundException)
      {
        return null;
      }
    }

    /// <summary>
    ///   Gets the entity by Id.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static T FindById<T>(this IDataSession existingDataSession, ISessionToken userId, int id)
      where T : EntityObject
    {
      try
      {
        return existingDataSession.GetById<T>(userId, id);
      }
      catch (EntityRecordNotFoundException)
      {
        return null;
      }
    }

    /// <summary>
    /// Gets the entity by Id
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static EntityObject FindById(this IDataSession existingDataSession, ISessionToken userId, Type entityType, int id)
    {
      try
      {
        return existingDataSession.GetById(userId, entityType, id);
      }
      catch (EntityRecordNotFoundException)
      {
        return null;
      }
    }
    
    /// <summary>
    ///   Gets the entity by id.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="id">The id.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static T GetById<T>(this IDataSession existingDataSession, ISessionToken userId, int id)
        where T : EntityObject
    {
      return BusinessServiceManager.GetById<T>(userId, id, existingDataSession);
    }

    /// <summary>
    ///   Gets the entity by id.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="id">The id.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static EntityObject GetById(this IDataSession existingDataSession, ISessionToken userId, Type entityType, int id)
    {
      return BusinessServiceManager.GetById(userId, entityType, id, existingDataSession);
    }

    /// <summary>
    ///   Saves the specified entity.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static int? Save(this IDataSession existingDataSession, ISessionToken userId, EntityObject entity)
    {
      return BusinessServiceManager.Save(userId, entity, existingDataSession);
    }

    /// <summary>
    ///   Saves the specified entities
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entities">The entities.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static bool Save(this IDataSession existingDataSession, ISessionToken userId, IList<EntityObject> entities)
    {
      return BusinessServiceManager.Save(userId, entities, existingDataSession);
    }

    /// <summary>
    ///   Saves the specified user id.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="entities">The entities.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static bool Save<T>(this IDataSession existingDataSession, ISessionToken userId, IList<T> entities)
        where T : EntityObject
    {
      return BusinessServiceManager.Save<T>(userId, entities, existingDataSession);
    }

    /// <summary>
    ///   Bulk Saves the list of new entities
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="entities">The entities.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    public static bool BulkSave<T>(this IDataSession existingDataSession, ISessionToken userId, IList<T> entities)
      where T : EntityObject
    {
      return BusinessServiceManager.BulkSave<T>(userId, entities, existingDataSession);
    }

    /// <summary>
    ///   Bulk Saves the list of new entities with audit
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="entities">The entities.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    public static bool BulkSaveWithAudit<T>(this IDataSession existingDataSession, ISessionToken userId, IList<T> entities)
      where T : EntityObject
    {
      return BusinessServiceManager.BulkSaveWithAudit<T>(userId, entities, existingDataSession);
    }

    public static ApiQuery<T> GetApiCollection<T>(this IDataSession existingDataSession, ISessionToken userId, ICriteria criteria, bool includeCount) where T : EntityObject
    {
      var query = BusinessServiceManager.GetService<T>().GetApiCollection(userId, criteria, includeCount, existingDataSession);

      return new ApiQuery<T>(query.Entities, query.Count);
    }

    public static IList<T> GetAllByCriteria<T>(this IDataSession existingDataSession, ISessionToken userId, ICriteria criteria) where T : EntityObject
    {
      return BusinessServiceManager.GetAllByCriteria<T>(userId, criteria, existingDataSession);
    }

    public static int GetCountByCriteria<T>(this IDataSession existingDataSession, ISessionToken userId, ICriteria criteria) where T : EntityObject
    {
      return BusinessServiceManager.GetCountByCriteria<T>(userId, criteria, existingDataSession);
    }
    
    public static bool GetExistsByCriteria<T>(this IDataSession existingDataSession, ISessionToken userId, ICriteria criteria) where T : EntityObject
    {
      if (existingDataSession == null)
        throw new ArgumentNullException(nameof(existingDataSession));
      if (userId == null)
        throw new ArgumentNullException(nameof(userId));
      if (criteria == null)
        throw new ArgumentNullException(nameof(criteria));

      return BusinessServiceManager.GetExistsByCriteria<T>(userId, criteria, existingDataSession);
    }

    public static IList<EntityHistory> GetAllHistoryByCriteria<T>(this IDataSession existingDataSession, ISessionToken userId, ICriteria criteria) where T : EntityObject
    {
      return BusinessServiceManager.GetAllHistoryByCriteria<T>(userId, criteria, existingDataSession);
    }

    public static EntityHistoryBuilder GetHistoryBuilder(this IDataSession existingDataSession, EntityObject entity)
    {
      return BusinessServiceManager.GetHistoryBuilder(entity);
    }

    public static void CreateEntityHistory(this IDataSession existingDataSession, ISessionToken userId, EntityHistoryBuilder builder)
    {
      BusinessServiceManager.CreateEntityHistory(existingDataSession, userId, builder);
    }

    public static void CreateEntityHistory(this IDataSession existingDataSession, ISessionToken userId, EntityObject entity, bool forceWhenNoChanges = false)
    {
      BusinessServiceManager.CreateEntityHistory(existingDataSession, userId, entity, forceWhenNoChanges);
    }

    /// <summary>
    /// Get the integer identifier from guid.
    /// </summary>
    /// <param name="dataSession">The dataSession.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="guid">The unique identifier.</param>
    /// <returns></returns>
    public static int? GetIntIdFromGUID(this IDataSession dataSession, ISessionToken userId, Type entityType, Guid? guid)
    {
      return BusinessServiceManager.GetIntIdFromGUID(dataSession, userId, entityType, guid);
    }

    public static Stream GetBlob(this IDataSession dataSession, EntityBlob blob)
    {
      return BusinessServiceManager.GetBlob(blob, dataSession);
    }

    public static byte[] GetBlobAsArray(this IDataSession dataSession, EntityBlob blob)
    {
      return BusinessServiceManager.GetBlobAsArray(blob, dataSession);
    }

    public static IDataSessionTransaction CreateTransaction(this IDataSession self)
    {
      bool owner = self.GetTransaction() == null;
      if (owner)
        self.StartTransaction();
      return new DataSessionTransaction(self, owner);
    }

    public static IDataSessionTransaction CreateTransaction(this IDataSession self, IsolationLevel level)
    {
      bool owner = self.GetTransaction() == null;
      if (owner)
        self.StartTransaction(level);
      return new DataSessionTransaction(self, owner);
    }

    public static ParallelPipelineBuilder Parallel(this IDataSession self)
    {
      return new ParallelPipelineBuilder(self, ((SQLDataSession)self).Configuration.ParallelDataSessionThreadCount ?? 1);
    }
    
    public static void Broadcast(this IDataSession self, object obj)
    {
      // self can be null to simplify the existingDataSession pattern
      if (self != null && self.IsTransaction())
      {
        self.AfterCommit(dataSession => DataServices.Resolve<IBroadcastService>().Broadcast(obj));
      }
      else
      {
        DataServices.Resolve<IBroadcastService>().Broadcast(obj);
      }
    }

    private class DataSessionTransaction : IDataSessionTransaction
    {
      private IDataSession dataSession;
      private readonly bool owner;
      private bool commit;
      private bool disposed;

      public DataSessionTransaction(IDataSession dataSession, bool owner)
      {
        this.dataSession = dataSession;
        this.owner = owner;
      }

      public void Commit()
      {
        commit = true;
      }

      public void Dispose()
      {
        if (!disposed)
        {
          if (dataSession != null)
          {
            if (owner)
            {
              if (commit)
                dataSession.CommitTransaction();
              else
                dataSession.RollbackTransaction();
            }
            dataSession = null;
          }

          disposed = true;
        }
      }
    }
  }
}