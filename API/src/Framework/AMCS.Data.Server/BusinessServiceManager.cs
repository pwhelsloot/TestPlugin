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

#pragma warning disable CS0618

namespace AMCS.Data.Server
{
  using System;
  using System.Collections.Generic;
  using System.Configuration;
  using System.IO;
  using AMCS.Data.Entity;
  using AMCS.Data.Entity.History;
  using AMCS.Data.Server.Services;
  using AMCS.Data.Server.SQL;
  using AMCS.Data.Server.SQL.Querying;
  using Data.Util.Extension;

  /// <summary>
  /// The Business Service Manager.
  /// </summary>
  public static class BusinessServiceManager
  {
    private static readonly object[] EmptyArguments = new object[0];

    #region Enumerations

    /// <summary>
    /// Business Service Manager Strings.
    /// </summary>
    public enum BusinessServiceManagerStrings
    {
      [StringValue("You do not have permission to perform this operation.")]
      YouDoNoHavePermission
    }

    /// <summary>
    /// Access Types.
    /// </summary>
    private enum AccessType
    {
      Create, /*Read,*/
      Write,
      Delete
    }

    #endregion Enumerations

    #region Methods

    /// <summary>
    ///   Deletes the specified entity by id.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="id">The id.</param>
    /// <param name="isUndelete">if set to <c>true</c> [is undelete].</param>
    public static void Delete<T>(ISessionToken userId, int id, bool isUndelete)
        where T : EntityObject
    {
      Delete<T>(userId, id, isUndelete, null);
    }

    /// <summary>
    ///   Deletes the specified entity by id.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="id">The id.</param>
    /// <param name="isUndelete">if set to <c>true</c> [is undelete].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    [Obsolete("Use extension methods on IDataSession")]
    public static void Delete<T>(ISessionToken userId, int id, bool isUndelete, IDataSession existingDataSession)
        where T : EntityObject
    {
      T entity = GetById<T>(userId, id, existingDataSession);
      Delete(userId, entity, isUndelete, existingDataSession);
    }

    /// <summary>
    ///   Deletes the specified entity.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="id">The id.</param>
    /// <param name="isUndelete">if set to <c>true</c> [is undelete].</param>
    public static void Delete(ISessionToken userId, Type entityType, int id, bool isUndelete)
    {
      Delete(userId, entityType, id, isUndelete, null);
    }

    /// <summary>
    ///   Deletes the specified entity.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="id">The id.</param>
    /// <param name="isUndelete">if set to <c>true</c> [is undelete].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    [Obsolete("Use extension methods on IDataSession")]
    public static void Delete(ISessionToken userId, Type entityType, int id, bool isUndelete, IDataSession existingDataSession)
    {
      EntityObject entity = GetById(userId, entityType, id, existingDataSession);
      Delete(userId, entity, isUndelete, existingDataSession);
    }

    /// <summary>
    ///   Deletes the specified entity.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="isUndelete">if set to <c>true</c> [is undelete].</param>
    public static void Delete(ISessionToken userId, EntityObject entity, bool isUndelete)
    {
      Delete(userId, entity, isUndelete, null);
    }

    /// <summary>
    ///   Deletes the specified entity.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="isUndelete">if set to <c>true</c> [is undelete].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    [Obsolete("Use extension methods on IDataSession")]
    public static void Delete(ISessionToken userId, EntityObject entity, bool isUndelete, IDataSession existingDataSession)
    {
      ThrowExceptionIfUserAccessDenied(userId, entity.GetType(), AccessType.Delete, existingDataSession);

      IEntityObjectService eos = GetService(entity.GetType());
      if (!isUndelete)
      {
        eos.Delete(userId, entity, existingDataSession);
      }
      else
      {
        eos.UnDelete(userId, entity, existingDataSession);
      }
    }

    /// <summary>
    /// Deletes all by template.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    [Obsolete("Use extension methods on IDataSession")]
    public static void DeleteAllByTemplate<T>(ISessionToken userId, T template, bool andTemplateProperties, IDataSession existingDataSession)
        where T : EntityObject
    {
      ThrowExceptionIfUserAccessDenied(userId, typeof(T), AccessType.Delete, existingDataSession);

      IEntityObjectService<T> eos = GetService<T>();
      eos.DeleteAllByTemplate(userId, template, andTemplateProperties, existingDataSession);
    }

    /// <summary>
    /// Deletes all by template.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    public static void DeleteAllByTemplate<T>(ISessionToken userId, T template, bool andTemplateProperties)
        where T : EntityObject
    {
      DeleteAllByTemplate(userId, template, andTemplateProperties, null);
    }

    /// <summary>
    /// Deletes all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    [Obsolete("Use extension methods on IDataSession")]
    public static void DeleteAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, IDataSession existingDataSession)
    {
      ThrowExceptionIfUserAccessDenied(userId, template.GetType(), AccessType.Delete, existingDataSession);

      IEntityObjectService eos = GetService(template.GetType());
      eos.DeleteAllByTemplate(userId, template, andTemplateProperties, existingDataSession);
    }

    /// <summary>
    /// Deletes all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    public static void DeleteAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties)
    {
      DeleteAllByTemplate(userId, template, andTemplateProperties, null);
    }

    /// <summary>
    ///   Gets all entities.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <returns></returns>
    public static IList<T> GetAll<T>(ISessionToken userId, bool includeDeleted)
        where T : EntityObject
    {
      return GetAll<T>(userId, includeDeleted, null);
    }

    /// <summary>
    ///   Gets all entities.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    [Obsolete("Use extension methods on IDataSession")]
    public static IList<T> GetAll<T>(ISessionToken userId, bool includeDeleted, IDataSession existingDataSession)
        where T : EntityObject
    {
      IEntityObjectService<T> eos = GetService<T>();
      return eos.GetAllById(userId, 0, includeDeleted, existingDataSession);
    }

    /// <summary>
    ///   Gets all entities.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <returns></returns>
    public static IList<EntityObject> GetAll(ISessionToken userId, Type entityType, bool includeDeleted)
    {
      return GetAll(userId, entityType, includeDeleted, null);
    }

    /// <summary>
    ///   Gets all entities.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    [Obsolete("Use extension methods on IDataSession")]
    public static IList<EntityObject> GetAll(ISessionToken userId, Type entityType, bool includeDeleted, IDataSession existingDataSession)
    {
      IEntityObjectService eos = GetService(entityType);
      return eos.GetAllById(userId, 0, includeDeleted, existingDataSession);
    }

    /// <summary>
    ///   Gets all entities, always including the one specified by id (even if it's deleted).
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="id">The id.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <returns></returns>
    public static IList<T> GetAllById<T>(ISessionToken userId, int id, bool includeDeleted)
        where T : EntityObject
    {
      return GetAllById<T>(userId, id, includeDeleted, null);
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
    [Obsolete("Use extension methods on IDataSession")]
    public static IList<T> GetAllById<T>(ISessionToken userId, int id, bool includeDeleted, IDataSession existingDataSession)
        where T : EntityObject
    {
      IEntityObjectService<T> eos = GetService<T>();
      return eos.GetAllById(userId, id, includeDeleted, existingDataSession);
    }

    /// <summary>
    ///   Gets all entities, always including the one specified by id (even if it's deleted).
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="id">The id.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <returns></returns>
    public static IList<EntityObject> GetAllById(ISessionToken userId, Type entityType, int id, bool includeDeleted)
    {
      return GetAllById(userId, entityType, id, includeDeleted, null);
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
    [Obsolete("Use extension methods on IDataSession")]
    public static IList<EntityObject> GetAllById(ISessionToken userId, Type entityType, int id, bool includeDeleted, IDataSession existingDataSession)
    {
      IEntityObjectService eos = GetService(entityType);
      return eos.GetAllById(userId, id, includeDeleted, existingDataSession);
    }

    /// <summary>
    ///   Gets all by parent entity id.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="parentId">The parent id.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <returns></returns>
    public static IList<T> GetAllByParentId<T>(ISessionToken userId, int parentId, bool includeDeleted)
        where T : EntityObject
    {
      return GetAllByParentId<T>(userId, parentId, includeDeleted, null);
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
    [Obsolete("Use extension methods on IDataSession")]
    public static IList<T> GetAllByParentId<T>(ISessionToken userId, int parentId, bool includeDeleted, IDataSession existingDataSession)
        where T : EntityObject
    {
      IEntityObjectService<T> eos = GetService<T>();
      return eos.GetAllByParentId(userId, parentId, includeDeleted, existingDataSession);
    }

    /// <summary>
    ///   Gets all by parent entity id.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="parentId">The parent id.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <returns></returns>
    public static IList<EntityObject> GetAllByParentId(ISessionToken userId, Type entityType, int parentId, bool includeDeleted)
    {
      return GetAllByParentId(userId, entityType, parentId, includeDeleted, null);
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
    [Obsolete("Use extension methods on IDataSession")]
    public static IList<EntityObject> GetAllByParentId(ISessionToken userId, Type entityType, int parentId, bool includeDeleted, IDataSession existingDataSession)
    {
      IEntityObjectService eos = GetService(entityType);
      return eos.GetAllByParentId(userId, parentId, includeDeleted, existingDataSession);
    }

    /// <summary>
    /// Gets a single entity by template.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="template"></param>
    /// <param name="andTemplateProperties"></param>
    /// <param name="throwExceptionIfNotFound"></param>
    /// <returns></returns>
    public static EntityObject GetByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, bool throwExceptionIfNotFound)
    {
      return GetByTemplate(userId, template, andTemplateProperties, throwExceptionIfNotFound, null);
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
    [Obsolete("Use extension methods on IDataSession")]
    public static EntityObject GetByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, bool throwExceptionIfNotFound, IDataSession existingDataSession)
    {
      IList<EntityObject> entities = GetAllByTemplate(userId, template, true, existingDataSession);
      if (entities != null)
      {
        if (entities.Count == 1)
          return entities[0];
        if (entities.Count > 1)
          throw new EntitySearchException(string.Format("Expected to find no more than 1 {0} but found {1}", template.GetType().FullName, entities.Count));
      }
      if (throwExceptionIfNotFound)
        throw new EntitySearchException(string.Format("Could not find {0} matching template", template.GetType().FullName));
      return null;
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
    [Obsolete("Use extension methods on IDataSession")]
    public static T GetByTemplate<T>(ISessionToken userId, T template, bool andTemplateProperties, bool throwExceptionIfNotFound, IDataSession existingDataSession)
      where T : EntityObject
    {
      return (T)GetByTemplate(userId, (EntityObject)template, andTemplateProperties, throwExceptionIfNotFound, existingDataSession);
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
    public static T GetByTemplate<T>(ISessionToken userId, T template, bool andTemplateProperties, bool throwExceptionIfNotFound = false)
        where T : EntityObject
    {
      return GetByTemplate(userId, template, true, throwExceptionIfNotFound, null);
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
    [Obsolete("Use extension methods on IDataSession")]
    public static IList<T> GetAllByTemplate<T>(ISessionToken userId, T template, bool andTemplateProperties, IDataSession existingDataSession)
        where T : EntityObject
    {
      IEntityObjectService<T> eos = GetService<T>();
      return eos.GetAllByTemplate(userId, template, andTemplateProperties, existingDataSession);
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
    [Obsolete("Use extension methods on IDataSession")]
    public static IList<T> GetAllByTemplate<T>(ISessionToken userId, T template, bool andTemplateProperties, IList<string> ignorePropertyNames, IList<string> includeOnlyPropertyNames, IDataSession existingDataSession)
      where T : EntityObject
    {
      IEntityObjectService<T> eos = GetService<T>();
      return eos.GetAllByTemplate(userId, template, andTemplateProperties, ignorePropertyNames, includeOnlyPropertyNames, existingDataSession);
    }

    /// <summary>
    /// Gets all by template.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <returns></returns>
    public static IList<T> GetAllByTemplate<T>(ISessionToken userId, T template, bool andTemplateProperties)
        where T : EntityObject
    {
      return GetAllByTemplate(userId, template, andTemplateProperties, null);
    }

    /// <summary>
    /// Gets all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    [Obsolete("Use extension methods on IDataSession")]
    public static IList<EntityObject> GetAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, IDataSession existingDataSession)
    {
      IEntityObjectService eos = GetService(template.GetType());
      return eos.GetAllByTemplate(userId, template, andTemplateProperties, existingDataSession);
    }

    /// <summary>
    /// Gets all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <returns></returns>
    public static IList<EntityObject> GetAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties)
    {
      return GetAllByTemplate(userId, template, andTemplateProperties, null);
    }

    /// <summary>
    ///   Gets the entity by Guid.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="guid">The guid.</param>
    /// <returns></returns>
    public static T GetByGuid<T>(ISessionToken userId, Guid guid)
        where T : EntityObject
    {
      return (T)GetByGuid(userId, typeof(T), guid, null);
    }

    /// <summary>
    ///   Gets the entity by Guid.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="guid">The guid.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    [Obsolete("Use extension methods on IDataSession")]
    public static T GetByGuid<T>(ISessionToken userId, Guid guid, IDataSession existingDataSession)
        where T : EntityObject
    {
      return (T)GetByGuid(userId, typeof(T), guid, existingDataSession);
    }

    /// <summary>
    ///   Gets the entity by Guid.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="guid">The guid.</param>
    /// <returns></returns>
    public static EntityObject GetByGuid(ISessionToken userId, Type entityType, Guid guid)
    {
      return GetByGuid(userId, entityType, guid, null);
    }

    /// <summary>
    /// Gets the entity by Guid
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="guid">The unique identifier.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    [Obsolete("Use extension methods on IDataSession")]
    public static EntityObject GetByGuid(ISessionToken userId, Type entityType, Guid guid, IDataSession existingDataSession)
    {
      IEntityObjectService eos = GetService(entityType);
      return eos.GetByGuid(userId, guid, existingDataSession);
    }

    /// <summary>
    ///   Gets the entity by id.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="id">The id.</param>
    /// <returns></returns>
    public static T GetById<T>(ISessionToken userId, int id)
        where T : EntityObject
    {
      return (T)GetById(userId, typeof(T), id, null);
    }

    /// <summary>
    ///   Gets the entity by id.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="id">The id.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    [Obsolete("Use extension methods on IDataSession")]
    public static T GetById<T>(ISessionToken userId, int id, IDataSession existingDataSession)
        where T : EntityObject
    {
      return (T)GetById(userId, typeof(T), id, existingDataSession);
    }

    /// <summary>
    ///   Gets the entity by id.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="id">The id.</param>
    /// <returns></returns>
    public static EntityObject GetById(ISessionToken userId, Type entityType, int id)
    {
      return GetById(userId, entityType, id, null);
    }

    /// <summary>
    ///   Gets the entity by id.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="id">The id.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    [Obsolete("Use extension methods on IDataSession")]
    public static EntityObject GetById(ISessionToken userId, Type entityType, int id, IDataSession existingDataSession)
    {
      IEntityObjectService eos = GetService(entityType);
      return eos.GetById(userId, id, existingDataSession);
    }

    /// <summary>
    ///   Gets a new entity.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <returns></returns>
    public static T GetNew<T>(ISessionToken userId)
      where T : EntityObject
    {
      return GetNew<T>(userId, EmptyArguments);
    }

    /// <summary>
    ///   Gets a new entity.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="args">The args.</param>
    /// <returns></returns>
    public static T GetNew<T>(ISessionToken userId, params object[] args)
      where T : EntityObject
    {
      return (T)GetNew(userId, typeof(T), args);
    }

    /// <summary>
    ///   Gets a new entity.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    [Obsolete("Use extension methods on IDataSession")]
    public static T GetNew<T>(ISessionToken userId, IDataSession existingDataSession)
      where T : EntityObject
    {
      return GetNew<T>(userId, existingDataSession, EmptyArguments);
    }

    /// <summary>
    ///   Gets a new entity.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="args">The args.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    [Obsolete("Use extension methods on IDataSession")]
    public static T GetNew<T>(ISessionToken userId, IDataSession existingDataSession, params object[] args)
      where T : EntityObject
    {
      return (T)GetNew(userId, typeof(T), existingDataSession, args);
    }

    /// <summary>
    ///   Gets a new entity.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="args">The args.</param>
    /// <returns></returns>
    public static EntityObject GetNew(ISessionToken userId, Type entityType)
    {
      return GetNew(userId, entityType, EmptyArguments);
    }

    /// <summary>
    ///   Gets a new entity.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="args">The args.</param>
    /// <returns></returns>
    public static EntityObject GetNew(ISessionToken userId, Type entityType, params object[] args)
    {
      return GetNew(userId, entityType, null, args);
    }

    /// <summary>
    ///   Gets a new entity.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    [Obsolete("Use extension methods on IDataSession")]
    public static EntityObject GetNew(ISessionToken userId, Type entityType, IDataSession existingDataSession)
    {
      return GetNew(userId, entityType, existingDataSession, EmptyArguments);
    }

    /// <summary>
    ///   Gets a new entity.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="args">The args.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    [Obsolete("Use extension methods on IDataSession")]
    public static EntityObject GetNew(ISessionToken userId, Type entityType, IDataSession existingDataSession, params object[] args)
    {
      ThrowExceptionIfUserAccessDenied(userId, entityType, AccessType.Create, existingDataSession);
      IEntityObjectService eos = GetService(entityType);
      return eos.GetNew(userId, existingDataSession, args);
    }

    /// <summary>
    /// Gets the new as copy.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    public static EntityObject GetNewAsCopy(ISessionToken userId, EntityObject entity, IDataSession existingDataSession = null)
    {
      ThrowExceptionIfUserAccessDenied(userId, entity.GetType(), AccessType.Create, existingDataSession);
      IEntityObjectService eos = GetService(entity.GetType());
      return eos.GetNewAsCopy(userId, entity);
    }

    /// <summary>
    ///   Gets the entity object service.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <returns></returns>
    public static IEntityObjectService<T> GetService<T>()
        where T : EntityObject
    {
      return (IEntityObjectService<T>)GetService(typeof(T));
    }

    /// <summary>
    /// Gets the entity object service.
    /// </summary>
    /// <param name="entityObjectType">Type of the entity object.</param>
    /// <returns></returns>
    public static IEntityObjectService GetService(Type entityObjectType)
    {
      return (IEntityObjectService)DataServices.Resolve(typeof(IEntityObjectService<>).MakeGenericType(entityObjectType));
    }

    /// <summary>
    ///   Saves the specified entity.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entity">The entity.</param>
    /// <returns></returns>
    public static int? Save(ISessionToken userId, EntityObject entity)
    {
      return Save(userId, entity, null);
    }

    /// <summary>
    ///   Saves the specified entity.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    [Obsolete("Use extension methods on IDataSession")]
    public static int? Save(ISessionToken userId, EntityObject entity, IDataSession existingDataSession)
    {
      ThrowExceptionIfUserAccessDenied(userId, entity.GetType(), AccessType.Write, existingDataSession);

      IEntityObjectService eos = GetService(entity.GetType());
      return eos.Save(userId, entity, existingDataSession);
    }

    /// <summary>
    ///   Saves the specified entities
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entities">The entities.</param>
    /// <returns></returns>
    public static bool Save(ISessionToken userId, IList<EntityObject> entities)
    {
      return Save(userId, entities, null);
    }

    /// <summary>
    ///   Saves the specified entities
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="entities">The entities.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    [Obsolete("Use extension methods on IDataSession")]
    public static bool Save(ISessionToken userId, IList<EntityObject> entities, IDataSession existingDataSession)
    {
      if (entities == null || entities.Count == 0)
        return false;

      Type entityType = entities[0].GetType();

      ThrowExceptionIfUserAccessDenied(userId, entityType, AccessType.Write, existingDataSession);

      IEntityObjectService eos = GetService(entityType);
      eos.Save(userId, entities, existingDataSession);
      return true;
    }

    /// <summary>
    ///   Saves the specified user id.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="entities">The entities.</param>
    /// <returns></returns>
    public static bool Save<T>(ISessionToken userId, IList<T> entities)
        where T : EntityObject
    {
      return Save(userId, entities, null);
    }

    /// <summary>
    ///   Saves the specified user id.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="entities">The entities.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    [Obsolete("Use extension methods on IDataSession")]
    public static bool Save<T>(ISessionToken userId, IList<T> entities, IDataSession existingDataSession)
        where T : EntityObject
    {
      if (entities == null || entities.Count == 0)
        return false;

      Type entityType = entities[0].GetType();

      ThrowExceptionIfUserAccessDenied(userId, entityType, AccessType.Write, existingDataSession);

      IEntityObjectService eos = GetService(entityType);
      eos.Save(userId, new List<EntityObject>(entities), existingDataSession);
      return true;
    }

    public static bool BulkSave<T>(ISessionToken userId, IList<T> entities)
        where T : EntityObject
    {
      return BulkSave(userId, entities, null);
    }

    /// <summary>
    ///   Bulk Saves the list of new entities
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="entities">The entities.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    [Obsolete("Use extension methods on IDataSession")]
    public static bool BulkSave<T>(ISessionToken userId, IList<T> entities, IDataSession existingDataSession)
      where T : EntityObject
    {
      if (entities == null || entities.Count == 0)
        return false;

      Type entityType = entities[0].GetType();

      ThrowExceptionIfUserAccessDenied(userId, entityType, AccessType.Write, existingDataSession);

      IEntityObjectService eos = GetService(entityType);
      eos.BulkSave(userId, new List<EntityObject>(entities), existingDataSession);
      return true;
    }

    /// <summary>
    ///   Bulk Saves the list of new entities
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="userId">The user id.</param>
    /// <param name="entities">The entities.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    public static bool BulkSaveWithAudit<T>(ISessionToken userId, IList<T> entities, IDataSession existingDataSession)
      where T : EntityObject
    {
      if (entities == null || entities.Count == 0)
        return false;

      Type entityType = entities[0].GetType();

      ThrowExceptionIfUserAccessDenied(userId, entityType, AccessType.Write, existingDataSession);

      IEntityObjectService eos = GetService(entityType);
      eos.BulkSaveWithAudit(userId, new List<EntityObject>(entities), existingDataSession);
      return true;
    }

    public static IDictionary<EntityObjectRequest, IList<EntityObject>> GetMultipleByRequest(ISessionToken userId, ICollection<EntityObjectRequest> requests)
    {
      var metadata = DataServices.Resolve<EntityObjectManager>().Entities;

      Dictionary<EntityObjectRequest, IList<EntityObject>> results = new Dictionary<EntityObjectRequest, IList<EntityObject>>();
      if (requests != null && requests.Count > 0)
      {
        foreach (EntityObjectRequest request in requests)
        {
          Type entityType = metadata[request.EntityTypeFullName].Type;

          switch (request.RequestType)
          {
            case EntityObjectRequest.WebRequestType.GetById:
              EntityObject getByIdEntity = GetById(userId, entityType, request.Id);
              results.Add(request, new List<EntityObject> { getByIdEntity });
              break;

            case EntityObjectRequest.WebRequestType.GetAll:
              IList<EntityObject> getAllEntities = GetAll(userId, entityType, request.IncludeDeleted);
              results.Add(request, getAllEntities);
              break;

            case EntityObjectRequest.WebRequestType.GetAllById:
              IList<EntityObject> getAllByIdEntities = GetAllById(userId, entityType, request.Id, request.IncludeDeleted);
              results.Add(request, getAllByIdEntities);
              break;

            case EntityObjectRequest.WebRequestType.GetAllByParentId:
              IList<EntityObject> getAllByParentIdEntities = GetAllByParentId(userId, entityType, request.Id, request.IncludeDeleted);
              results.Add(request, getAllByParentIdEntities);
              break;
          }
        }
      }

      return results;
    }

    public static IList<T> GetAllByCriteria<T>(ISessionToken userId, ICriteria criteria) where T : EntityObject
    {
      return GetAllByCriteria<T>(userId, criteria, null);
    }

    [Obsolete("Use extension methods on IDataSession")]
    public static IList<T> GetAllByCriteria<T>(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession) where T : EntityObject
    {
      IEntityObjectService<T> eos = GetService<T>();
      return eos.GetAllByCriteria(userId, criteria, existingDataSession);
    }

    public static int GetCountByCriteria<T>(ISessionToken userId, ICriteria criteria) where T : EntityObject
    {
      return GetCountByCriteria<T>(userId, criteria, null);
    }

    [Obsolete("Use extension methods on IDataSession")]
    public static int GetCountByCriteria<T>(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession) where T : EntityObject
    {
      IEntityObjectService<T> eos = GetService<T>();
      return eos.GetCountByCriteria(userId, criteria, existingDataSession);
    }

    public static bool GetExistsByCriteria<T>(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession) where T : EntityObject
    {
      if (userId == null)
        throw new ArgumentNullException(nameof(userId));
      if (criteria == null)
        throw new ArgumentNullException(nameof(criteria));
      if (existingDataSession == null)
        throw new ArgumentNullException(nameof(existingDataSession));

      IEntityObjectService<T> eos = GetService<T>();
      if (eos == null)
        throw new ConfigurationException($"Service {typeof(IEntityObjectService<T>)} is not configured");

      return eos.GetExistsByCriteria(userId, criteria, existingDataSession);
    }

    public static IList<EntityHistory> GetAllHistoryByCriteria<T>(ISessionToken userId, ICriteria criteria) where T : EntityObject
    {
      return GetAllHistoryByCriteria<T>(userId, criteria, null);
    }

    public static IList<EntityHistory> GetAllHistoryByCorrelation(ISessionToken userId, string correlationId, IDataSession existingDataSession)
    {
      var criteria = Criteria.For(typeof(EntityHistory))
        .Add(Expression.Eq(nameof(EntityHistory.CorrelationId), correlationId));

      var eos = GetService<EntityHistory>();
      return eos.GetAllByCriteria(userId, criteria, existingDataSession);
    }

    public static IList<EntityHistory> GetAllHistoryByParent(ISessionToken userId, Type parentEntityType, int parentTableId, IDataSession existingDataSession)
    {
      var criteria = Criteria.For(typeof(EntityHistory))
        .Add(Expression.Eq(nameof(EntityHistory.ParentTable), EntityHistoryBuilder.GetParentTableName(parentEntityType)))
        .Add(Expression.Eq(nameof(EntityHistory.ParentTableId), parentTableId));

      var eos = GetService<EntityHistory>();
      return eos.GetAllByCriteria(userId, criteria, existingDataSession);
    }

    [Obsolete("Use extension methods on IDataSession")]
    public static IList<EntityHistory> GetAllHistoryByCriteria<T>(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession) where T : EntityObject
    {
      IEntityObjectService<T> eos = GetService<T>();
      return eos.GetAllHistoryByCriteria(userId, criteria, existingDataSession);
    }

    public static EntityHistoryBuilder GetHistoryBuilder(EntityObject entity)
    {
      return new EntityHistoryBuilder(entity, DataUpdateKind.Update);
    }

    public static void CreateEntityHistory(IDataSession existingDataSession, ISessionToken userId, EntityHistoryBuilder builder)
    {
      builder.ManualSave(existingDataSession, userId);
    }

    public static void CreateEntityHistory(IDataSession existingDataSession, ISessionToken userId, EntityObject entity, bool forceWhenNoChanges = false)
    {
      var builder = new EntityHistoryBuilder(entity, DataUpdateKind.Update);
      builder.ForceWriteWhenNoChanges(forceWhenNoChanges);
      CreateEntityHistory(existingDataSession, userId, builder);
    }

    /// <summary>
    /// Get the integer identifier from guid.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="guid">The unique identifier.</param>
    /// <returns></returns>
    public static int? GetIntIdFromGUID(ISessionToken userId, Type entityType, Guid? guid)
    {
      return GetIntIdFromGUID(null, userId, entityType, guid);
    }

    /// <summary>
    /// Get the integer identifier from guid.
    /// </summary>
    /// <param name="dataSession">The dataSession.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="guid">The unique identifier.</param>
    /// <returns></returns>
    [Obsolete("Use extension methods on IDataSession")]
    public static int? GetIntIdFromGUID(IDataSession dataSession, ISessionToken userId, Type entityType, Guid? guid)
    {
      // TODO make factory decide on sysuserid
      if (guid != null)
      {
        if (dataSession == null)
        {
          int? result;

          using (IDataSession dataSessionInternal = BslDataSessionFactory.GetDataSession(userId))
          using (var transaction = dataSessionInternal.CreateTransaction())
          {
            result = SQLDataAccessHelper.GetIntIdFromGUID(guid, entityType, dataSessionInternal);

            transaction.Commit();
          }

          return result;
        }
        else
        {
          return SQLDataAccessHelper.GetIntIdFromGUID(guid, entityType, dataSession);
        }
      }
      else
      {
        return null;
      }
    }

    public static Stream GetBlob(EntityBlob blob, IDataSession dataSession)
    {
      return SQLBlobHelper.GetBlob(blob, dataSession);
    }

    public static byte[] GetBlobAsArray(EntityBlob blob, IDataSession dataSession)
    {
      return SQLBlobHelper.GetBlobAsArray(blob, dataSession);
    }

    public static void SaveBlob(EntityBlob blob, Stream stream)
    {
      SQLBlobHelper.SaveBlob(blob, stream);
    }

    public static void SaveBlob(EntityBlob blob, byte[] buffer)
    {
      SQLBlobHelper.SaveBlob(blob, buffer);
    }

    public static void ClearBlob(EntityBlob blob)
    {
      SQLBlobHelper.ClearBlob(blob);
    }

    // Throws the exception if user access denied.
    // It's safest if we only call this for Create, Write & Delete operations.  There will be times
    // when Read operations (like GetById) are called but these won't be because a user wants to edit the record.  The UI
    // knows about restrictions
    // so it will deal with the reads/edits.
    // TODO: Need to be able to translate the exception messages

    /// <summary>
    /// Throws the exception if user access denied.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="accessTypeRequired">The access type required.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    private static void ThrowExceptionIfUserAccessDenied(ISessionToken userId, Type entityType, AccessType accessTypeRequired, IDataSession existingDataSession = null)
    {
      bool throwException = false;
      var restriction = DataServices.Resolve<IRestrictionService>().GetEntityRestriction(userId, entityType, existingDataSession);
      if (restriction == null)
        return;

      if (accessTypeRequired == AccessType.Create && restriction.IsNewDenied)
        throwException = true;

      if ((accessTypeRequired == AccessType.Write || accessTypeRequired == AccessType.Delete) && restriction.IsEditDenied)
        throwException = true;

      if (accessTypeRequired == AccessType.Delete && restriction.IsDeleteDenied)
        throwException = true;

      if (throwException)
        throw BslUserExceptionFactory<BslUserAccessDeniedException>.CreateException(typeof(BusinessServiceManager), typeof(BusinessServiceManagerStrings), (int)BusinessServiceManagerStrings.YouDoNoHavePermission);
    }

    #endregion Methods
  }
}