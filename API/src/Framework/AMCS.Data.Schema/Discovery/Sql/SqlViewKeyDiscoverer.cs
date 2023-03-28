using AMCS.Data.Schema.Access;

namespace AMCS.Data.Schema.Discovery.Sql
{
  public class SqlViewKeyDiscoverer: SqlKeyDiscoverer
  {
    #region Sql

    protected override string GetKeyDetailsSql
    {
      get
      {
        return
@"SELECT 
	SCHEMA_NAME([Table].[schema_id]) AS [TableSchema], [Table].[name] AS [TableName], [Column].[name] AS [ColumnName], [Key].[name] AS [KeyName], [Key].[type] AS [KeyType]
FROM
	[sys].[views] AS [View]
	JOIN [sys].[columns] AS [ViewColumn] ON [View].[object_id] = [ViewColumn].[object_id]
	JOIN [sys].[sql_dependencies] [Depends] ON [View].[object_id] = [Depends].[object_id]
	JOIN [sys].[tables] [Table] ON [Depends].[referenced_major_id] = [Table].[object_id]
	JOIN [sys].[columns] [Column] ON [Table].[object_id] = [Column].[object_id]
		AND [Depends].[referenced_minor_id] = [Column].[column_id]
		AND [ViewColumn].[name] = [Column].[name]
	JOIN [sys].[index_columns] [Index] ON [Table].[object_id] = [Index].[object_id]
		AND [Column].[column_id] = [Index].[index_column_id]
	JOIN [sys].[key_constraints] [Key] ON [Index].[index_id] = [Key].[unique_index_id]
		AND [Table].[object_id] = [Key].[parent_object_id]
		AND [Key].[type] = 'PK'
WHERE
	SCHEMA_NAME([View].[schema_id]) = @Schema
	AND [View].[name] = @QueryableObjectName
	AND 
	(
		@Column IS NULL 
		OR [ViewColumn].[name] = @Column
	)";
      }
    }

    #endregion Sql

    #region ctors

    public SqlViewKeyDiscoverer(IDatabaseInterface dbInterface, IForeignKeyDiscoverer foreignKeyDiscoverer = null) : base(dbInterface, foreignKeyDiscoverer) { }

    #endregion ctors
  }
}
