//-----------------------------------------------------------------------------
// <copyright file="IEntityObjectService.cs" company="AMCS Group">
//   Copyright © 2010-12 AMCS Group. All rights reserved.
// </copyright>
//
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Entity.History;
using AMCS.Data.Server.SQL.Querying;

namespace AMCS.Data.Server
{
  #region IEntityObjectService<T>

  /// <summary>
  /// The Entity Object Service Interface.
  /// </summary>
  /// <typeparam name="T">The entity type.</typeparam>
  public interface IEntityObjectService<T> : IEntityObjectService
   where T : EntityObject
  {
    /// <summary>
    /// Gets the new.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns></returns>
    new T GetNew(ISessionToken userId, IDataSession existingDataSession);

    /// <summary>
    /// Gets the new.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="args">The arguments.</param>
    /// <returns></returns>
    new T GetNew(ISessionToken userId, IDataSession existingDataSession, params object[] args);

    /// <summary>
    /// Gets the new as copy.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entity">The entity.</param>
    /// <returns></returns>
    T GetNewAsCopy(ISessionToken userId, T entity);

    /// <summary>
    /// Gets the by identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    new T GetById(ISessionToken userId, int id, IDataSession existingDataSession = null);

    /// <summary>
    /// Gets the by unique identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="guid">The unique identifier.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    new T GetByGuid(ISessionToken userId, Guid guid, IDataSession existingDataSession = null);

    /// <summary>
    /// Gets all by identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    new IList<T> GetAllById(ISessionToken userId, int id, bool includeDeleted, IDataSession existingDataSession = null);

    /// <summary>
    /// Gets all by parent identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="parentId">The parent identifier.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    new IList<T> GetAllByParentId(ISessionToken userId, int parentId, bool includeDeleted, IDataSession existingDataSession = null);

    /// <summary>
    /// Gets all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    IList<T> GetAllByTemplate(ISessionToken userId, T template, bool andTemplateProperties, IDataSession existingDataSession = null);

    /// <summary>
    /// Gets all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="ignorePropertyNames">The ignore property names.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    IList<T> GetAllByTemplate(ISessionToken userId, T template, bool andTemplateProperties, IList<string> ignorePropertyNames, IDataSession existingDataSession = null);

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
    IList<T> GetAllByTemplate(ISessionToken userId, T template, bool andTemplateProperties, IList<string> ignorePropertyNames, IList<string> includeOnlyPropertyNames, IDataSession existingDataSession = null);

    /// <summary>
    /// Saves the specified user identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    int? Save(ISessionToken userId, T entity, IDataSession existingDataSession = null);

    /// <summary>
    /// Saves the specified user identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entities">The entities.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    void Save(ISessionToken userId, IList<T> entities, IDataSession existingDataSession = null);

    /// <summary>
    /// Deletes the specified user identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    void Delete(ISessionToken userId, T entity, IDataSession existingDataSession = null);

    /// <summary>
    /// Deletes all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    void DeleteAllByTemplate(ISessionToken userId, T template, bool andTemplateProperties, IDataSession existingDataSession = null);

    /// <summary>
    /// Undo the delete.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    void UnDelete(ISessionToken userId, T entity, IDataSession existingDataSession = null);

    /// <summary>
    /// Get all entities matching the provided criteria.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="criteria">The criteria to filter by.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns>All entities matching the criteria</returns>
    new IList<T> GetAllByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession = null);
  }

  #endregion IEntityObjectService<T>

  #region IEntityObjectService

  /// <summary>
  /// The Entity Object Service.
  /// </summary>
  public interface IEntityObjectService : ITranslatableService
  {
    /// <summary>
    /// Gets the new.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns></returns>
    EntityObject GetNew(ISessionToken userId, IDataSession existingDataSession);

    /// <summary>
    /// Gets the new.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="args">The arguments.</param>
    /// <returns></returns>
    EntityObject GetNew(ISessionToken userId, IDataSession existingDataSession, params object[] args);

    /// <summary>
    /// Gets the new as copy.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entity">The entity.</param>
    /// <returns></returns>
    EntityObject GetNewAsCopy(ISessionToken userId, EntityObject entity);

    /// <summary>
    /// Gets the by identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    EntityObject GetById(ISessionToken userId, int id, IDataSession existingDataSession = null);

    /// <summary>
    /// Gets the by unique identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="guid">The unique identifier.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    EntityObject GetByGuid(ISessionToken userId, Guid guid, IDataSession existingDataSession = null);

    /// <summary>
    /// Gets all by identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    IList<EntityObject> GetAllById(ISessionToken userId, int id, bool includeDeleted, IDataSession existingDataSession = null);

    /// <summary>
    /// Gets all by parent identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="parentId">The parent identifier.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    IList<EntityObject> GetAllByParentId(ISessionToken userId, int parentId, bool includeDeleted, IDataSession existingDataSession = null);

    /// <summary>
    /// Gets all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    IList<EntityObject> GetAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, IDataSession existingDataSession = null);

    /// <summary>
    /// Gets all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="ignorePropertyNames">The ignore property names.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    IList<EntityObject> GetAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, IList<string> ignorePropertyNames, IDataSession existingDataSession = null);

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
    IList<EntityObject> GetAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, IList<string> ignorePropertyNames, IList<string> includeOnlyPropertyNames, IDataSession existingDataSession = null);

    /// <summary>
    /// Saves the specified user identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    int? Save(ISessionToken userId, EntityObject entity, IDataSession existingDataSession = null);

    /// <summary>
    /// Saves the specified user identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entities">The entities.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    void Save(ISessionToken userId, IList<EntityObject> entities, IDataSession existingDataSession = null);

    /// <summary>
    /// Bulk Saves the list of new entities.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entities">The entities.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    void BulkSave(ISessionToken userId, IList<EntityObject> entities, IDataSession existingDataSession = null);

    /// <summary>
    /// Bulk Saves the list of new entities and audit records.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entities">The entities.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    void BulkSaveWithAudit(ISessionToken userId, IList<EntityObject> entities, IDataSession existingDataSession = null);

    /// <summary>
    /// Deletes the specified user identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    void Delete(ISessionToken userId, EntityObject entity, IDataSession existingDataSession = null);

    /// <summary>
    /// Deletes all by template.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="existingDataSession">The existing data session.</param>
    void DeleteAllByTemplate(ISessionToken userId, EntityObject template, bool andTemplateProperties, IDataSession existingDataSession = null);

    /// <summary>
    /// Undo the delete.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    void UnDelete(ISessionToken userId, EntityObject entity, IDataSession existingDataSession = null);

    /// <summary>
    /// Executes a filtered collection query for the API.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="criteria">The criteria to filter by.</param>
    /// <param name="includeCount">Whether to include a count based on the criteria in the response.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns>The results of the executed query.</returns>
    ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null);

    /// <summary>
    /// Get all entities matching the provided criteria.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="criteria">The criteria to filter by.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns>All entities matching the criteria</returns>
    IList<EntityObject> GetAllByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession = null);

    /// <summary>
    /// Get all entity history records for the provided criteria
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="criteria">The criteria to filter by.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns></returns>
    IList<EntityHistory> GetAllHistoryByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession = null);

    /// <summary>
    /// Get the number of entities matching the provided criteria.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="criteria">The criteria to filter by.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns>The number of entities matching the criteria.</returns>
    int GetCountByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession = null);

    /// <summary>
    /// Returns true if there are items for the provided criteria otherwise false.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="criteria">The criteria to filter by.</param>
    /// <param name="existingDataSession">The existing data session.</param>
    /// <returns>Returns true if records exists otherwise false.</returns>
    bool GetExistsByCriteria(ISessionToken userId, ICriteria criteria, IDataSession existingDataSession = null);
    }

  #endregion IEntityObjectService
}