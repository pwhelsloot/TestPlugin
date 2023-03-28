//-----------------------------------------------------------------------------
// <copyright file="IEntityObjectAccess.cs" company="AMCS Group">
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
using AMCS.Data.Server.SQL;
using AMCS.Data.Server.SQL.Querying;

namespace AMCS.Data.Server
{
  #region IEntityObjectAccess

  public interface IEntityObjectAccess
  {
    IList<EntityObject> GetAllById(IDataSession dataSession, ISessionToken userId, int id, bool includeDeleted, string orderByCsvFieldNames = null);

    ISQLReadable GetByCriteria(IDataSession dataSession, ISessionToken userId, ICriteria criteria, CriteriaQueryType queryType);

    IList<EntityHistory> GetAllHistoryByCriteria(IDataSession dataSession, ISessionToken userId, ICriteria criteria);

    IList<EntityObject> GetAllByParentId(IDataSession dataSession, ISessionToken userId, int parentId, bool includeDeleted);

    IList<EntityObject> GetAllByTemplate(IDataSession dataSession, ISessionToken userId, EntityObject template, bool andTemplateProperties, IList<string> ignorePropertyNames = null);

    EntityObject GetById(IDataSession dataSession, ISessionToken userId, int id, string orderByCsvFieldNames = null);

    EntityObject GetByGuid(IDataSession dataSession, ISessionToken userId, Guid guid);

    int? Save(IDataSession dataSession, ISessionToken userId, EntityObject entity);

    void Delete(IDataSession dataSession, ISessionToken userId, EntityObject entity, bool isUndelete);
  }

  #endregion IEntityObjectAccess

  #region IEntityObjectAccess<T>

  public interface IEntityObjectAccess<T> : IEntityObjectAccess where T : EntityObject
  {
    new IList<T> GetAllById(IDataSession dataSession, ISessionToken userId, int id, bool includeDeleted, string orderByCsvFieldNames = null);

    new IList<T> GetAllByParentId(IDataSession dataSession, ISessionToken userId, int parentId, bool includeDeleted);

    IList<T> GetAllByTemplate(IDataSession dataSession, ISessionToken userId, T template, bool andTemplateProperties, IList<string> ignorePropertyNames = null, IList<string> includeOnlyPropertyNames = null);

    new IList<EntityHistory> GetAllHistoryByCriteria(IDataSession dataSession, ISessionToken userId, ICriteria criteria);

    new T GetById(IDataSession dataSession, ISessionToken userId, int id, string orderByCsvFieldNames = null);

    new T GetByGuid(IDataSession dataSession, ISessionToken userId, Guid guid);

    int? Save(IDataSession dataSession, ISessionToken userId, T entity, string[] restrictToFields = null);

    void BulkSave(IDataSession dataSession, ISessionToken userId, IList<T> entities);

    void BulkSaveWithAudit(IDataSession dataSession, ISessionToken userId, IList<T> entities);

    void Delete(IDataSession dataSession, ISessionToken userId, T entity, bool isUndelete);
  }

  #endregion IEntityObjectAccess<T>
}