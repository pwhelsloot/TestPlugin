using System;
using System.Collections.Generic;
using System.Data;
using AMCS.Data.Schema.Access;
using AMCS.Data.Schema.Sql;

namespace AMCS.Data.Schema.Discovery.Sql
{
  public class SqlRoutineDiscoverer : SqlDiscoverer, IRoutineDiscoverer
  {
    #region Constants

    protected virtual string GetRoutineSql
    {
      get
      {
        return
@"SELECT
	SCHEMA_NAME([Routine].[schema_id]) AS [Schema],
	[Routine].[name] AS RoutineName,
	CASE	
		WHEN [Routine].[type] = N'P' THEN 'StoredProcedure'
		WHEN [Routine].[type] = N'FN' THEN 'ScalarFunction'
		WHEN [Routine].[type] = N'IF' THEN 'TableValuedFunction'
		ELSE [Routine].[type]
	END AS RoutineType,
	[Sql].[definition] AS Text
FROM
	[sys].[objects] AS [Routine]
	JOIN [sys].[sql_modules] AS [Sql] ON [Sql].[object_id] = [Routine].[object_id]
WHERE
	(
		@Schema IS NULL 
		OR SCHEMA_NAME([Routine].[schema_id]) = @Schema
	)
	AND
	(
		@RoutineName IS NULL
		OR [Routine].[name] = @RoutineName
	)
	AND
	(
		(
			@RoutineNameType IS NULL
			AND [Routine].[type] IN (N'P', N'IF', N'FN')
		)
		OR
		(
			@RoutineNameType = 'StoredProcedure'
			AND [Routine].[type] = N'P'
		)
		OR
		(
			@RoutineNameType = 'Function'
			AND [Routine].[type] IN (N'IF', N'FN')
		)
		OR
		(
			@RoutineNameType = 'ScalarFunction'
			AND [Routine].[type] = N'FN'
		)
		OR
		(
			@RoutineNameType = 'TableValuedFunction'
			AND [Routine].[type] = N'IF'
		)
	)
ORDER BY 
  [Routine].[name]";
      }
    }

    #endregion Constants

    #region Properties/Variables

    private IDatabaseInterface _dbInterface;

    #endregion Properties/Variables

    #region ctors

    public SqlRoutineDiscoverer(IDatabaseInterface dbInterface)
    {
      if (dbInterface == null)
        throw new ArgumentNullException(nameof(dbInterface));
      _dbInterface = dbInterface;
    }

    #endregion ctors

    #region IRoutineDiscoverer

    public IList<IRoutine> Discover()
    {
      return _Discover(null, null, null);
    }

    public IList<IRoutine> Discover(string schema)
    {
      if (schema == null)
        throw new ArgumentNullException(nameof(schema));

      return _Discover(schema, null, null);
    }

    public IList<IRoutine> Discover(string schema, RoutineType routineType)
    {
      if (schema == null)
        throw new ArgumentNullException(nameof(schema));

      return _Discover(schema, routineType, null);
    }

    public IList<IRoutine> Discover(RoutineType routineType)
    {
      return _Discover(null, routineType, null);
    }

    public IRoutine Discover(string schema, RoutineType routineType, string name)
    {
      if (schema == null)
        throw new ArgumentNullException(nameof(schema));
      if (name == null)
        throw new ArgumentNullException(nameof(name));

      IList<IRoutine> result = _Discover(schema, routineType, name);
      if (result == null || result.Count == 0)
        return null;
      if (result.Count > 1)
        throw new DataException(string.Format("Not not retreive expected result from database for '{0}' '{1}.{2}'", routineType.ToString(), schema, name));
      return result[0];
    }

    private IList<IRoutine> _Discover(string schema, RoutineType? routineType, string name)
    {
      if (!string.IsNullOrWhiteSpace(schema))
        schema = SanitiseObjectName(schema);
      if (!string.IsNullOrWhiteSpace(name))
        name = SanitiseObjectName(name);

      Dictionary<string, object> parameters = new Dictionary<string, object>();
      parameters.Add("@Schema", schema == null ? DBNull.Value : (object)schema);
      parameters.Add("@RoutineNameType", !routineType.HasValue ? DBNull.Value : (object)routineType.ToString());
      parameters.Add("@RoutineName", name == null ? DBNull.Value : (object)name);

      using (DataTable dataTable = _dbInterface.GetDataTable(GetRoutineSql, parameters))
      {
        List<IRoutine> result = new List<IRoutine>();
        if (dataTable != null && dataTable.Rows != null && dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in dataTable.Rows)
          {
            string resSchema = row.Field<string>("Schema");
            string resRoutineType = row.Field<string>("RoutineType");
            string resRoutineName = row.Field<string>("RoutineName");
            string resText = row.Field<string>("Text");

            if (string.IsNullOrWhiteSpace(resSchema) || string.IsNullOrWhiteSpace(resRoutineType) || string.IsNullOrWhiteSpace(resRoutineName) || string.IsNullOrWhiteSpace(resText))
              throw new DataException("Retreived incomplete routine information from database");

            RoutineType rt = (RoutineType)Enum.Parse(typeof(RoutineType), resRoutineType);
            result.Add(new SqlRoutine(rt, string.Format("{0}.{1}", resSchema, resRoutineName), resText));
          }
        }
        return result;
      }
    }

    #endregion IRoutineDiscoverer
  }
}
