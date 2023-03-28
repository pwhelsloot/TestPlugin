using System;
using System.Collections.Generic;
using System.Data;
using AMCS.Data.Schema.Access;

namespace AMCS.Data.Schema.Discovery.Sql
{
  public abstract class SqlQueryableObjectSchemaDiscoverer : SqlDiscoverer, IQueryableObjectSchemaDiscoverer
  {
    #region Constants

    protected virtual string GetQueryableObjectNamesSql
    {
      get
      {
        return
        @"SELECT 
	        [Object].[name] AS [ObjectName] 
        FROM 
	        [sys].[objects] [Object] 
        WHERE 
	        [Object].[schema_id] = SCHEMA_ID(@Schema)
          AND 
          (
            (
              @GetTables = 1 
              AND [Object].[type] = 'U'
            )
            OR
            (
              @GetViews = 1 
              AND [Object].[type] = 'V'
            )
          )
        ORDER BY 
	        [Object].[type], [Object].[name]";
      }
    }

    #endregion Constants

    #region Properties/Variables

    private IDatabaseInterface _dbInterface;
    private IColumnSchemaDiscoverer _columnSchemaDiscoverer;
    protected abstract bool GetTables { get; }
    protected abstract bool GetViews { get; }

    
    #endregion Properties/Variables

    #region ctors

    public SqlQueryableObjectSchemaDiscoverer(IDatabaseInterface dbInterface, IColumnSchemaDiscoverer columnSchemaDiscoverer)
    {
      if (dbInterface == null)
        throw new ArgumentNullException(nameof(dbInterface));
      if (columnSchemaDiscoverer == null)
        throw new ArgumentNullException(nameof(columnSchemaDiscoverer));
      _dbInterface = dbInterface;
      _columnSchemaDiscoverer = columnSchemaDiscoverer;
    }

    #endregion ctors

    #region Abstract

    protected abstract IQueryableObject ConstructQueryableObject(string schema, string queryableObjectName, IList<IColumn> columns);

    #endregion Abstract

    #region IQueryableObjectSchemaDiscoverer

    public IQueryableObject Discover(string schema, string queryableObjectName)
    {
      if (schema == null)
        throw new ArgumentNullException(nameof(schema));
      if (queryableObjectName == null)
        throw new ArgumentNullException(nameof(queryableObjectName));

      schema = SanitiseObjectName(schema);
      queryableObjectName = SanitiseObjectName(queryableObjectName);

      IList<IColumn> columns = _columnSchemaDiscoverer.Discover(schema, queryableObjectName);
      return ConstructQueryableObject(schema, queryableObjectName, columns);
    }

    public IList<IQueryableObject> Discover(string schema)
    {
      if (schema == null)
        throw new ArgumentNullException(nameof(schema));

      schema = SanitiseObjectName(schema);

      List<string> objectNames = new List<string>();
      List<IQueryableObject> queryableObjects = new List<IQueryableObject>();

      IDictionary<string, object> parameters = new Dictionary<string, object>();
      parameters.Add("@Schema", schema);
      parameters.Add("@GetTables", GetTables);
      parameters.Add("@GetViews", GetViews);

      using (DataTable dataTable = _dbInterface.GetDataTable(GetQueryableObjectNamesSql, parameters))
      {
        if (dataTable != null && dataTable.Rows != null && dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in dataTable.Rows)
          {
            string objectName = row.Field<string>("ObjectName");
            if (string.IsNullOrWhiteSpace(objectName))
              throw new NullReferenceException(string.Format("Failed to read a queryable object name from the schema '{0}'", schema));
            objectNames.Add(objectName);

            IList<IColumn> columns = _columnSchemaDiscoverer.Discover(schema, objectName);
            queryableObjects.Add(ConstructQueryableObject(schema, objectName, columns));
          }
        }
      }
      return queryableObjects;
    }

    #endregion IQueryableObjectSchemaDiscoverer
  }
}
