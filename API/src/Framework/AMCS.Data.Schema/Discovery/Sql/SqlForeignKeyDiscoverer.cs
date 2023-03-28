using System;
using System.Collections.Generic;
using System.Data;
using AMCS.Data.Schema.Access;
using AMCS.Data.Schema.Sql;

namespace AMCS.Data.Schema.Discovery.Sql
{
  public class SqlForeignKeyDiscoverer : SqlDiscoverer, IForeignKeyDiscoverer
  {
    #region Constants

    private const string ByPassColumnName = "@@@Bypass@@@";

    protected virtual string GetForeignKeysSql
    {
      get
      {
        return
      @"SELECT  
        [ForeignKey].[name] AS [ForeignKeyName],
        'OF' AS [ForeignKeyType], -- [O]utbound [F]oreign-key
        SCHEMA_NAME([ParentTable].[schema_id]) AS [ParentTableSchema],
        [ParentTable].[name] AS [ParentTableName],
        [ParentColumn].[name] AS [ParentColumnName],
        SCHEMA_NAME([ChildTable].[schema_id]) AS [ChildTableSchema],
        [ChildTable].[name] AS [ChildTableName],
        [ChildColumn].[name] AS [ChildColumnName]
      FROM 
	      [sys].[objects] [ForeignKey]
	      JOIN [sys].[foreign_key_columns] [ForeignKeyColumn] ON [ForeignKey].[object_id] = [ForeignKeyColumn].[constraint_object_id]
	      JOIN [sys].[tables] [ParentTable] ON [ForeignKeyColumn].[parent_object_id] = [ParentTable].[object_id]
	      JOIN [sys].[columns] [ParentColumn] ON [ParentTable].[object_id] = [ParentColumn].[object_id]
		      AND [ForeignKeyColumn].[parent_column_id] = [ParentColumn].[column_id]
	      JOIN [sys].[tables] [ChildTable] ON [ForeignKeyColumn].[referenced_object_id] = [ChildTable].[object_id]  
	      JOIN [sys].[columns] [ChildColumn] ON [ChildTable].[object_id] = [ChildColumn].[object_id] 
		      AND [ForeignKeyColumn].[referenced_column_id] = [ChildColumn].[column_id]
      WHERE
        SCHEMA_NAME([ParentTable].[schema_id]) = @Schema
	      AND [ParentTable].[name] = @QueryableObjectName
	      AND 
	      (
		      @Column IS NULL
		      OR [ParentColumn].[name] = @Column
	      )

      UNION ALL

      SELECT  
        [ForeignKey].[name] AS [ForeignKeyName],
        'IF' AS [ForeignKeyType], -- [I]nbound [F]oreign-key
        SCHEMA_NAME([ParentTable].[schema_id]) AS [ParentTableSchema],
        [ParentTable].[name] AS [ParentTableName],
        [ParentColumn].[name] AS [ParentColumnName],
        SCHEMA_NAME([ChildTable].[schema_id]) AS [ChildTableSchema],
        [ChildTable].[name] AS [ChildTableName],
        [ChildColumn].[name] AS [ChildColumnName]
      FROM 
	      [sys].[objects] [ForeignKey]
	      JOIN [sys].[foreign_key_columns] [ForeignKeyColumn] ON [ForeignKey].[object_id] = [ForeignKeyColumn].[constraint_object_id]
	      JOIN [sys].[tables] [ParentTable] ON [ForeignKeyColumn].[parent_object_id] = [ParentTable].[object_id]
	      JOIN [sys].[columns] [ParentColumn] ON [ParentTable].[object_id] = [ParentColumn].[object_id]
		      AND [ForeignKeyColumn].[parent_column_id] = [ParentColumn].[column_id]
	      JOIN [sys].[tables] [ChildTable] ON [ForeignKeyColumn].[referenced_object_id] = [ChildTable].[object_id]  
	      JOIN [sys].[columns] [ChildColumn] ON [ChildTable].[object_id] = [ChildColumn].[object_id] 
		      AND [ForeignKeyColumn].[referenced_column_id] = [ChildColumn].[column_id]
      WHERE
        SCHEMA_NAME([ParentTable].[schema_id]) = @Schema
	      AND [ChildTable].[name] = @QueryableObjectName
	      AND 
	      (
		      @Column IS NULL
		      OR [ChildColumn].[name] = @Column
	      )";
      }
    }

    #endregion Constants

    #region Properties/Variables

    private IDatabaseInterface _dbInterface;

    #endregion Properties/Variables

    #region ctors

    public SqlForeignKeyDiscoverer(IDatabaseInterface dbInterface)
    {
      if (dbInterface == null)
        throw new ArgumentNullException(nameof(dbInterface));
      _dbInterface = dbInterface;
    }

    #endregion ctors

    #region IForeignKeyDiscoverer

    public IList<IForeignKey> Discover(string schema, string table, string column)
    {
      IList<IForeignKey> result = new List<IForeignKey>();
      Action<IForeignKey> onForeignKeyRead = new Action<IForeignKey>((col) => result.Add(col));
      Discover(schema, table, column, onForeignKeyRead);
      return result;
    }

    public IList<IForeignKey> Discover(string schema, string table)
    {
      return Discover(schema, table, ByPassColumnName);
    }

    #endregion IForeignKeyDiscoverer

    #region Helper

    private void Discover(string schema, string table, string column, Action<IForeignKey> onForeignKeyRead)
    {
      if (schema == null)
        throw new ArgumentNullException(nameof(schema));
      if (table == null)
        throw new ArgumentNullException(nameof(table));
      if (column == null)
        throw new ArgumentNullException(nameof(column));

      schema = SanitiseObjectName(schema);
      table = SanitiseObjectName(table);
      column = SanitiseObjectName(column);

      Dictionary<string, object> parameters = new Dictionary<string, object>();
      parameters.Add("@Schema", schema);
      parameters.Add("@QueryableObjectName", table);
      parameters.Add("@Column", column == ByPassColumnName ? (object)DBNull.Value : column);

      using (DataTable dataTable = _dbInterface.GetDataTable(GetForeignKeysSql, parameters))
      {
        if (dataTable != null && dataTable.Rows != null && dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in dataTable.Rows)
          {
            try
            {
              string fkName = row.Field<string>("ForeignKeyName");
              string fkType = row.Field<string>("ForeignKeyType");
              string parentTableSchema = row.Field<string>("ParentTableSchema");
              string parentTable = row.Field<string>("ParentTableName");
              string parentColumn = row.Field<string>("ParentColumnName");
              string childTableSchema = row.Field<string>("ChildTableSchema");
              string childTable = row.Field<string>("ChildTableName");
              string childColumn = row.Field<string>("ChildColumnName");

              //not expecting or dealing with composite foreign keys.  This may turn out to be a bug in the future but not worth spending time on now 
              //because it probably won't happen

              onForeignKeyRead(new SqlForeignKey(fkName, parentTableSchema, parentTable, new List<string>() { parentColumn }, new SqlKey(fkName, childTableSchema, childTable, new List<string>() { childColumn })));
            }
            catch (Exception ex)
            {
              throw new DataException(string.Format("Failed to read foreign key data for '{0}.{1}{2}'", schema, table, column == ByPassColumnName ? "" : "." + column), ex);
            }
          }
        }
      }
    }

    #endregion Helper
  }
}
