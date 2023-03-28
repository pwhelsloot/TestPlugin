using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMCS.Data.Entity;
using AMCS.Data.Entity.History;
using AMCS.Data.Entity.SQL;
using AMCS.Data.Server.SQL.Fetch;
using AMCS.Data.Server.SQL.Querying;

namespace AMCS.Data.Server.SQL
{
  public class SQLEntityObjectAccess<T> : IEntityObjectAccess<T>, IEntityObjectAccess where T : EntityObject
  {
    #region Get

    /// <summary>
    /// Gets the data.
    /// </summary>
    /// <param name="dataSession">The data session.</param>
    /// <param name="all">if set to <c>true</c> [all].</param>
    /// <param name="id">The id.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <param name="orderByCsvFieldNames">The order by CSV field names.</param>
    /// <returns></returns>
    private ISQLReadable GetData(IDataSession dataSession, bool all, int? id, Guid? guid, bool includeDeleted, string orderByCsvFieldNames = null)
    {
      var accessor = EntityObjectAccessor.ForType(typeof(T));

      bool hasIsDeletedProp = accessor.CanUndelete;
      var parameters = new List<Parameter>();
      var sql = new StringBuilder();

      sql.Append("SELECT * FROM ").Append('[').Append(accessor.SchemaName).Append("].[").Append(accessor.TableName).Append(']');
      if (guid.HasValue)
      {
        sql.Append(" WHERE GUID = @Guid");
        parameters.Add(new Parameter("@Guid", guid));
      }
      else if (id.GetValueOrDefault(0) > 0)
      {
        string keyFieldName = accessor.KeyName;

        sql
          .Append(" WHERE ")
          .Append(keyFieldName)
          .Append(" = @")
          .Append(keyFieldName);

        parameters.Add(new Parameter("@" + keyFieldName, id.Value));

        if (all)
        {
          if (hasIsDeletedProp)
          {
            if (!includeDeleted)
              sql.Append(" OR IsDeleted = 0");
          }
          else
            sql
              .Append(" OR ")
              .Append(keyFieldName)
              .Append(" != @")
              .Append(keyFieldName);
        }
      }
      else if (all)
      {
        if (hasIsDeletedProp)
        {
          if (!includeDeleted)
            sql.Append(" WHERE IsDeleted = 0");
        }
      }

      if (!string.IsNullOrWhiteSpace(orderByCsvFieldNames))
        sql.Append(" ORDER BY ").Append(orderByCsvFieldNames);

      var cmd = dataSession.Query(sql.ToString());

      foreach (var parameter in parameters)
      {
        cmd.SetObject(parameter.Name, parameter.Value);
      }

      return cmd.Execute();
    }

    /// <summary>
    /// Gets the data.
    /// </summary>
    /// <param name="dataSession">The data session.</param>
    /// <param name="all">if set to <c>true</c> [all].</param>
    /// <param name="id">The id.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    /// <param name="orderByCsvFieldNames">The order by CSV field names.</param>
    /// <returns></returns>
    protected virtual ISQLReadable GetData(IDataSession dataSession, bool all, int id, bool includeDeleted, string orderByCsvFieldNames = null)
    {
      return GetData(dataSession, all, id, null, includeDeleted, orderByCsvFieldNames);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dataSession"></param>
    /// <param name="userId"></param>
    /// <param name="id"></param>
    /// <param name="includeDeleted"></param>
    /// <returns></returns>
    public virtual IList<T> GetAllById(IDataSession dataSession, ISessionToken userId, int id, bool includeDeleted, string orderByCsvFieldNames = null)
    {
      return GetData(dataSession, true, id, includeDeleted, orderByCsvFieldNames).List<T>();
    }

    public virtual ISQLReadable GetByCriteria(IDataSession dataSession, ISessionToken userId, ICriteria criteria, CriteriaQueryType queryType)
    {
      var queryBuilder = new SQLFetchCriteriaQueryBuilder(criteria, queryType);

      var query = dataSession.Query(queryBuilder.GetSql(), queryBuilder.FetchInfo);

      queryBuilder.SetParameters(query);

      return query.Execute();
    }

    public IList<EntityHistory> GetAllHistoryByCriteria(IDataSession dataSession, ISessionToken userId, ICriteria criteria)
    {
      using (var logger = CreatePerformanceLogger("GetAllHistoryByCriteria", typeof(T)))
      {
        var accessor = EntityObjectAccessor.ForType(typeof(T));

        string tableName = new SQLTextBuilder().TableName(accessor.SchemaName, accessor.TableName).ToString();
        var tableCriteria = FieldUtils.ExtractFromCriteria(criteria, nameof(EntityHistory.Table), FieldComparison.Eq);

        if (tableCriteria == null)
        {
          criteria.Expressions.Add(Expression.Eq(nameof(EntityHistory.Table), tableName));
        }
        else if (tableCriteria.Value.ToString() != tableName)
        {
          throw new ArgumentException($"Invalid table name provided, can only search for table {tableName}");
        }

        IList<EntityHistory> historyEntries = DataAccessManager.GetAccessForEntity<EntityHistory>().GetByCriteria(
          dataSession,
          userId,
          criteria,
          CriteriaQueryType.Select).List<EntityHistory>();

        foreach (EntityHistory history in historyEntries)
        {
          SQLDataAccessHistory.DeserializeEntityHistory(history);
        }
        return historyEntries;
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dataSession"></param>
    /// <param name="userId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public virtual T GetById(IDataSession dataSession, ISessionToken userId, int id, string orderByCsvFieldNames = null)
    {
      var entity = GetData(dataSession, false, id, true, orderByCsvFieldNames)
        .FirstOrDefault<T>();

      if (entity == null)
        throw new EntityRecordNotFoundException("System Error: Cannot locate " + typeof(T).FullName + " with Id of " + id.ToString() + " in database");

      return entity;
    }

    /// <summary>
    /// Gets the T by GUID.
    /// </summary>
    /// <param name="dataSession">The data session.</param>
    /// <param name="userId">The user id.</param>
    /// <param name="guid">The GUID.</param>
    /// <returns></returns>
    public virtual T GetByGuid(IDataSession dataSession, ISessionToken userId, Guid guid)
    {
      using (CreatePerformanceLogger("GetByGuid", typeof(T)))
      {
        var result = GetData(dataSession, false, null, guid, true)
          .FirstOrDefault<T>();
        if (result == null)
          throw new EntityRecordNotFoundException("System Error: Cannot locate " + typeof(T).FullName + " with Guid of " + guid.ToString() + " in database");
        return result;
      }
    }

    /// <summary>
    //  Returns a list of entities that match a given template.
    /// </summary>
    /// <param name="dataSession">The data session.</param>
    /// <param name="userId">The user id.</param>
    /// <param name="template">The template.</param>
    /// <param name="andTemplateProperties">if set to <c>true</c> [and template properties].</param>
    /// <param name="ignorePropertyNames">The ignore property names.</param>
    /// <param name="includeOnlyPropertyNames">List of only property names to search on (if provided ignorePropertyNames must be null!)</param>
    /// <returns></returns>
    public virtual IList<T> GetAllByTemplate(IDataSession dataSession, ISessionToken userId, T template, bool andTemplateProperties, IList<string> ignorePropertyNames = null, IList<string> includeOnlyPropertyNames = null)
    {
      using (CreatePerformanceLogger("GetAllByTemplate", typeof(T)))
      {
        if (ignorePropertyNames != null && includeOnlyPropertyNames != null)
        {
          // Developer exception
          throw new ArgumentException("Only provide a list of ignorePropertyNames OR includeOnlyPropertyNames. There should not be both.");
        }

        var accessor = EntityObjectAccessor.ForType(typeof(T));

        var sql = new StringBuilder()
          .Append("SELECT * FROM ")
          .Append(accessor.TableNameWithSchema)
          .Append(" WHERE ");

        var sqlParams = new List<Parameter>();
        bool isAfterFirstWhereCondition = false;
        foreach (var property in accessor.Properties)
        {
          if (property.Column == null)
            continue;

          if (ignorePropertyNames != null && ignorePropertyNames.Contains(property.Name))
            continue;

          if (includeOnlyPropertyNames != null && !includeOnlyPropertyNames.Contains(property.Name))
            continue;

          object propValue = property.GetValue(template);
          if (property.UseInTemplateQuery(propValue, includeOnlyPropertyNames != null))
          {
            string fieldName = property.Column.ColumnName;
            if (isAfterFirstWhereCondition)
            {
              if (andTemplateProperties)
                sql.Append(" AND ");
              else
                sql.Append(" OR ");
            }
            isAfterFirstWhereCondition = true;

            sql.Append('[').Append(fieldName).Append("] = @").Append(property.Name);
            sqlParams.Add(new Parameter("@" + property.Name, propValue));
          }
        }

        if (sqlParams.Count == 0)
          throw new Exception("System Error: Suspected coding error.  Attempt to GetAllByTemplate but no properties given values on the template.");

        var cmd = dataSession.Query(sql.ToString());

        foreach (var parameter in sqlParams)
        {
          cmd.SetObject(parameter.Name, parameter.Value);
        }

        return cmd.Execute().List<T>();
      }
    }

    /// <summary>
    /// This is intentionally not implemented, if a child class needs it then it must provide it's own implementation
    /// </summary>
    /// <param name="dataSession"></param>
    /// <param name="userId"></param>
    /// <param name="parentId"></param>
    /// <param name="includeDeleted"></param>
    /// <returns></returns>
    public virtual IList<T> GetAllByParentId(IDataSession dataSession, ISessionToken userId, int parentId, bool includeDeleted)
    {
      throw new NotSupportedException("System Error: SQLEntityObjectAccess.GetAllByParentId is not implemented.  Implementation must be provided by classes that descend from SQLEntityObjectAccess and require this functionality.");
    }

    EntityObject IEntityObjectAccess.GetById(IDataSession dataSession, ISessionToken userId, int id, string orderByCsvFieldNames)
    {
      return this.GetById(dataSession, userId, id, orderByCsvFieldNames);
    }

    EntityObject IEntityObjectAccess.GetByGuid(IDataSession dataSession, ISessionToken userId, Guid guid)
    {
      return this.GetByGuid(dataSession, userId, guid);
    }

    IList<EntityObject> IEntityObjectAccess.GetAllById(IDataSession dataSession, ISessionToken userId, int id, bool includeDeleted, string orderByCsvFieldNames)
    {
      return this.GetAllById(dataSession, userId, id, includeDeleted, orderByCsvFieldNames).ToList<EntityObject>();
    }

    IList<EntityObject> IEntityObjectAccess.GetAllByParentId(IDataSession dataSession, ISessionToken userId, int parentId, bool includeDeleted)
    {
      return this.GetAllByParentId(dataSession, userId, parentId, includeDeleted).ToList<EntityObject>();
    }

    IList<EntityObject> IEntityObjectAccess.GetAllByTemplate(IDataSession dataSession, ISessionToken userId, EntityObject template, bool andTemplateProperties, IList<string> ignorePropertyNames)
    {
      return this.GetAllByTemplate(dataSession, userId, (T)template, andTemplateProperties, ignorePropertyNames).ToList<EntityObject>();
    }

    #endregion Get

    #region Save

    /// <summary>
    /// Cache is cleared in SQLDataAccessHelper
    /// </summary>
    /// <param name="dataSession"></param>
    /// <param name="userId"></param>
    /// <param name="entity"></param>
    /// <param name="restrictToFields"></param>
    /// <returns></returns>
    public virtual int? Save(IDataSession dataSession, ISessionToken userId, T entity, string[] restrictToFields = null)
    {
      SQLStrictMode.ValidateInTransaction(dataSession);

      using (var logger = CreatePerformanceLogger("Save", typeof(T)))
      {
        if (logger != null)
          logger.AdditionalInformation = "Is Update = " + (entity.Id32 <= 0);

        bool isNewTransaction = false;
        if (!dataSession.IsTransaction())
        {
          dataSession.StartTransaction();
          isNewTransaction = true;
        }
        try
        {
          var returnId = DoSave(dataSession, userId, entity, restrictToFields);

          if (returnId.GetValueOrDefault(0) <= 0 && !EntityObjectAccessor.ForType(typeof(T)).InsertOnNullId)
            throw new Exception("System Error: Save failed, did not receive an id for an object of type " + typeof(T).FullName);

          if (isNewTransaction)
            dataSession.CommitTransaction();

          return returnId;
        }
        catch
        {
          if (isNewTransaction)
            dataSession.RollbackTransaction();
          throw;
        }
      }
    }

    protected virtual int? DoSave(IDataSession dataSession, ISessionToken userId, T entity, string[] restrictToFields)
    {
      var accessor = EntityObjectAccessor.ForType(typeof(T));
      bool hasNoId;
      if (accessor.InsertOnNullId)
        hasNoId = entity.GetId() == null;
      else
        hasNoId = entity.Id32 <= 0;

      switch (accessor.IdentityInsertMode)
      {
        case IdentityInsertMode.On:
          {
            if (hasNoId)
            {
              return DoInsert(dataSession, userId, entity, restrictToFields);
            }

            return dataSession.FindById<T>(userId, entity.Id32) != null
              ? DoUpdate(dataSession, userId, entity, restrictToFields)
              : DoInsertWithIdentity(dataSession, userId, entity);
          }

        case IdentityInsertMode.OnWithOverride:
          {

            if (hasNoId)
              throw new BslUserException($"{ typeof(T).Name } must have an Id.");

            return dataSession.FindById<T>(userId, entity.Id32) != null
              ? DoUpdate(dataSession, userId, entity, restrictToFields)
              : DoInsertWithIdentityOverride(dataSession, userId, entity);
          }

        default:
          {
            return hasNoId
              ? DoInsert(dataSession, userId, entity, restrictToFields)
              : DoUpdate(dataSession, userId, entity, restrictToFields);
          }
      }
    }

    protected virtual int? DoUpdate(IDataSession dataSession, ISessionToken userId, T entity, string[] restrictToFields)
    {
      var returnId = entity.Id;
      SQLDataAccessHelper.UpdateEntityObject(dataSession, userId, entity, null, true, restrictToFields, false, true);
      return returnId;
    }

    protected virtual int? DoInsert(IDataSession dataSession, ISessionToken userId, T entity, string[] restrictToFields)
    {
      return SQLDataAccessHelper.InsertEntityObjectReturnIdentity(dataSession, userId, entity, restrictToFields, true);
    }

    protected virtual int? DoInsertWithIdentity(IDataSession dataSession, ISessionToken userId, T entity)
    {
      SQLDataAccessHelper.InsertEntityObjectWithIdentity(dataSession, userId, entity);
      return entity.Id;
    }

    protected virtual int? DoInsertWithIdentityOverride(IDataSession dataSession, ISessionToken userId, T entity)
    {
      SQLDataAccessHelper.InsertEntityObjectOverrideSetIdentity(dataSession, userId, entity);
      return entity.Id;
    }

    /// <summary>
    /// Bulk Save - Implemented for saving a list of new entities (No Audit) (No synchronisation of record insertion)
    /// </summary>
    /// <param name="dataSession"></param>
    /// <param name="userId"></param>
    /// <param name="entities"></param>
    public virtual void BulkSave(IDataSession dataSession, ISessionToken userId, IList<T> entities)
    {
      SQLStrictMode.ValidateInTransaction(dataSession);

      using (CreatePerformanceLogger("Bulk Save", typeof(T)))
      {
        bool isNewTransaction = false;
        if (!dataSession.IsTransaction())
        {
          dataSession.StartTransaction();
          isNewTransaction = true;
        }
        try
        {
          if (entities.All(e => e.Id == null || e.Id <= 0))
          {
            List<EntityObject> entityObjects = new List<EntityObject>(entities);
            SQLDataAccessHelper.BulkInsertEntityObjects(dataSession, userId, entityObjects);
            //// No synchronisation of record insertion
          }
          else
          {
            throw new Exception("System Error: Bulk update is not implemented.");
          }

          if (isNewTransaction)
            dataSession.CommitTransaction();
        }
        catch
        {
          if (isNewTransaction)
            dataSession.RollbackTransaction();
          throw;
        }
      }
    }

    public virtual void BulkSaveWithAudit(IDataSession dataSession, ISessionToken userId, IList<T> entities)
    {
      SQLStrictMode.ValidateInTransaction(dataSession);

      using (CreatePerformanceLogger("Bulk Save with Audit", typeof(T)))
      {
        bool isNewTransaction = false;
        if (!dataSession.IsTransaction())
        {
          dataSession.StartTransaction();
          isNewTransaction = true;
        }
        try
        {
          if (entities.All(e => e.Id == null || e.Id <= 0))
          {
            List<EntityObject> entityObjects = new List<EntityObject>(entities);
            List<Guid> guids = SQLDataAccessHelper.BulkInsertEntityObjectsReturnIdentity(dataSession, userId, entityObjects);
            if (guids != null && guids.Count > 0)
            {
              string tablename = entityObjects.First().GetTableName();
              string keyname = entityObjects.First().GetKeyName();
              SQLDataAccessAudit.InsertAuditRecordBatchByGuid(userId, tablename, keyname, guids, SQLDataAuditChangeType.Insert, dataSession);
            }
            else
            {
              throw new Exception("System Error: Bulk update auditing is not implemented.");
            }
          }
          else
          {
            throw new Exception("System Error: Bulk update is not implemented.");
          }

          if (isNewTransaction)
            dataSession.CommitTransaction();
        }
        catch
        {
          if (isNewTransaction)
            dataSession.RollbackTransaction();
          throw;
        }
      }
    }

    int? IEntityObjectAccess.Save(IDataSession dataSession, ISessionToken userId, EntityObject entity)
    {
      return this.Save(dataSession, userId, (T)entity);
    }

    #endregion Save

    #region Delete

    /// <summary>
    /// Cache is cleared in SQLDataAccessHelper
    /// </summary>
    /// <param name="dataSession"></param>
    /// <param name="userId"></param>
    /// <param name="entity"></param>
    /// <param name="isUndelete"></param>
    public virtual void Delete(IDataSession dataSession, ISessionToken userId, T entity, bool isUndelete)
    {
      SQLStrictMode.ValidateInTransaction(dataSession);

      using (CreatePerformanceLogger("Delete", typeof(T)))
      {
        bool isNewTransaction = false;
        if (!dataSession.IsTransaction())
        {
          dataSession.StartTransaction();
          isNewTransaction = true;
        }
        try
        {
          DoDelete(dataSession, userId, entity, isUndelete);

          if (isNewTransaction)
            dataSession.CommitTransaction();
        }
        catch
        {
          if (isNewTransaction)
            dataSession.RollbackTransaction();
          throw;
        }
      }
    }

    protected virtual void DoDelete(IDataSession dataSession, ISessionToken userId, T entity, bool isUndelete)
    {
      SQLDataAccessHelper.DeleteEntityObject(dataSession, userId, entity, isUndelete, true);
    }

    void IEntityObjectAccess.Delete(IDataSession dataSession, ISessionToken userId, EntityObject entity, bool isUndelete)
    {
      this.Delete(dataSession, userId, (T)entity, isUndelete);
    }

    #endregion Delete

    #region Logging

    private static SQLPerformanceLogger CreatePerformanceLogger(string operationType, Type entityType)
    {
      // Removed this for now - need to move this form of logging out into a seperate static/singleton as there appears
      // to be a fair amount of overhead assocated with creating logger instances.
      return null;
    }

    #endregion Logging

    private struct Parameter
    {
      public string Name { get; }

      public object Value { get; }

      public Parameter(string name, object value)
      {
        Name = name;
        Value = value;
      }
    }
  }
}