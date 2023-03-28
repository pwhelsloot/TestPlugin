using System;
using System.Collections.Generic;
using System.Data;
using AMCS.Data.Schema.Access;
using AMCS.Data.Schema.Sql;

namespace AMCS.Data.Schema.Discovery.Sql
{
  public class SqlColumnSchemaDiscoverer : SqlDiscoverer, IColumnSchemaDiscoverer
  {
    #region Constants

    private const string ByPassColumnName = "@@@Bypass@@@";

    protected virtual string GetColumnDetailsSql
    {
      get
      {
        return
@"SELECT 
	[Column].[name] AS ColumnName, [Type].[name] AS TypeName, [Column].[is_nullable] AS IsNullable,
	CASE
		WHEN [Type].[name] IN (N'char', N'varchar', N'nchar', N'nvarchar', N'binary', N'varbinary') THEN '(' + 
			CASE 
				WHEN [Column].[max_length] = -1 THEN 'MAX'
				ELSE CAST([Column].[max_length] AS NVARCHAR) 
			END
			+ ')'
			WHEN [Type].[name] IN (N'float', N'real') THEN '(' + CAST([Column].[precision] AS NVARCHAR) + ')'
			WHEN [Type].[name] IN (N'decimal', N'numeric') THEN '(' + CAST([Column].[precision] AS NVARCHAR) + ',' + CAST([Column].[scale] AS NVARCHAR) + ')'
			ELSE ''
		END AS TypeAdditional,
    ISNULL(SCHEMA_NAME([Table].[schema_id]), SCHEMA_NAME([ViewReferencedTable].[ReferencedTableSchemaId])) AS [ParentTableSchema],
		ISNULL([Table].[name], [ViewReferencedTable].[ReferencedTable]) AS [ParentTableName]
FROM
	[sys].[objects] [Object] 
	JOIN [sys].[columns] [Column] ON [Object].[object_id] = [Column].[object_id]
	JOIN [sys].[types] AS [Type] ON ([Type].[user_type_id] = [Column].[system_type_id] 
    AND [Type].user_type_id = [Type].system_type_id) 
  LEFT JOIN [sys].[tables] [Table] ON [Object].[type] = N'U'
		AND [Column].[object_id] = [Table].[object_id]
	LEFT JOIN
	(
		SELECT 
			[Depends].[object_id] AS [DependencyObjectId], [RefTable].[schema_id] AS [ReferencedTableSchemaId], [RefTable].[name] AS [ReferencedTable],
			[RefColumn].[name] AS [ReferencedColumn]
		FROM
			[sys].[sql_dependencies] [Depends] 
			JOIN [sys].[tables] [RefTable] ON [Depends].[referenced_major_id] = [RefTable].[object_id]
			JOIN [sys].[columns] [RefColumn] ON [RefTable].[object_id] = [RefColumn].[object_id]
				AND [Depends].[referenced_minor_id] = [RefColumn].[column_id]
			-- risk duplicating results if selecting fields like CustomerId and joining on Customer and CustomerSite.
			-- don't try to link based on FK because of this, we might miss the odd thing but better than having duplicates
			LEFT JOIN [sys].[foreign_key_columns] [FKCol] ON [RefColumn].[object_id] = [FKCol].[parent_object_id]
				AND [FKCol].[parent_column_id] = [RefColumn].[column_id]
		WHERE
			[FKCol].[parent_object_id] IS NULL
	) [ViewReferencedTable] ON [Object].[type] = N'V' 
				AND [Object].[object_id] = [ViewReferencedTable].[DependencyObjectId]
				AND [Column].[name] = [ViewReferencedTable].[ReferencedColumn]
WHERE
	[Object].[type] IN (N'U',N'V')
	AND SCHEMA_NAME([Object].[schema_id]) = @Schema
	AND [Object].[name] = @QueryableObject
	AND 
	(
		@Column IS NULL
		OR [Column].[name] = @Column
	)
ORDER BY 
  [Column].[name]";
      }
    }

    #endregion Constants

    #region Properties/Variables

    private IDatabaseInterface _dbInterface;
    private IKeyDiscoverer _keyDiscoverer;

    #endregion Properties/Variables

    #region ctors

    public SqlColumnSchemaDiscoverer(IDatabaseInterface dbInterface, IKeyDiscoverer keyDiscoverer = null)
    {
      if (dbInterface == null)
        throw new ArgumentNullException(nameof(dbInterface));
      _dbInterface = dbInterface;
      _keyDiscoverer = keyDiscoverer;
    }

    #endregion ctors

    #region IColumnSchemaDiscoverer

    public IColumn Discover(string schema, string queryableObject, string column)
    {
      IColumn result = null;
      Action<IColumn> onColumnRead = new Action<IColumn>((col) =>
        {
          if (result != null)
            throw new DataException("Multiple columns encountered but only one expected");
          TryAddColumnKeys(schema, queryableObject, col);
          result = col;
        });
      Discover(schema, queryableObject, column, onColumnRead);
      if (result == null)
        throw new DataException(string.Format("Expected to find one colum record for '{0}.{1}.{2}' but did not find any", schema, queryableObject, column));
      return result;
    }

    public IList<IColumn> Discover(string schema, string queryableObject)
    {
      IList<IColumn> result = new List<IColumn>();
      Action<IColumn> onColumnRead = new Action<IColumn>((col) =>
        {
          TryAddColumnKeys(schema, queryableObject, col);
          result.Add(col);
        });
      Discover(schema, queryableObject, ByPassColumnName, onColumnRead);
      if (result == null || result.Count == 0)
        throw new DataException(string.Format("Expected to find at least one colum record for '{0}.{1}' but did not find any", schema, queryableObject));
      return result;
    }

    #endregion IColumnSchemaDiscoverer

    #region Helper

    private void Discover(string schema, string queryableObject, string column, Action<IColumn> onColumnRead)
    {
      if (schema == null)
        throw new ArgumentNullException(nameof(schema));
      if (queryableObject == null)
        throw new ArgumentNullException(nameof(queryableObject));
      if (column == null)
        throw new ArgumentNullException(nameof(column));

      schema = SanitiseObjectName(schema);
      queryableObject = SanitiseObjectName(queryableObject);
      column = SanitiseObjectName(column);

      Dictionary<string, object> parameters = new Dictionary<string, object>();
      parameters.Add("@Schema", schema);
      parameters.Add("@QueryableObject", queryableObject);
      parameters.Add("@Column", column == ByPassColumnName ? (object)DBNull.Value : column);

      using (DataTable dataTable = _dbInterface.GetDataTable(GetColumnDetailsSql, parameters))
      {
        if (dataTable != null && dataTable.Rows != null && dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in dataTable.Rows)
          {
            try
            {
              onColumnRead(new SqlColumn(row.Field<string>("ParentTableSchema"), row.Field<string>("ParentTableName"), row.Field<string>("ColumnName"), row.Field<string>("TypeName"), row.Field<bool>("IsNullable"), row.Field<string>("TypeAdditional")));
            }
            catch (Exception ex)
            {
              throw new DataException(string.Format("Failed to read column data for '{0}.{1}{2}'", schema, queryableObject, column == ByPassColumnName ? "" : "." + column), ex);
            }
          }
        }
      }
    }

    private void TryAddColumnKeys(string schemaName, string tableName, IColumn column)
    {
      if (column == null)
        throw new ArgumentNullException(nameof(column));
      if (_keyDiscoverer != null)
      {
        IList<IKey> columnKeys = _keyDiscoverer.Discover(schemaName, tableName, column.Name);
        column.AddKeys(columnKeys);
      }
    }

    #endregion Helper
  }
}
