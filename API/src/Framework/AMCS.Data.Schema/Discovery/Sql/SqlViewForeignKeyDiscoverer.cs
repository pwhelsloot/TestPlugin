using AMCS.Data.Schema.Access;

namespace AMCS.Data.Schema.Discovery.Sql
{
  public class SqlViewForeignKeyDiscoverer : SqlForeignKeyDiscoverer
  {
    #region Sql

    protected override string GetForeignKeysSql
    {
      get
      {
        return
@"SELECT
	[FK].[name] AS [ForeignKeyName], 
	'OF' AS [ForeignKeyType], -- [O]utbound [F]oreign-key
	SCHEMA_NAME([ParentFKTable].[schema_id]) AS [ParentTableSchema], [ParentFKTable].[name] AS [ParentTableName], [ParentFKCol].[name] AS [ParentColumnName],
	SCHEMA_NAME([ReferencedFKTable].[schema_id]) AS [ChildTableSchema], [ReferencedFKTable].[name] AS [ChildTableName], [ReferencedFKCol].[name] AS [ChildColumnName]
FROM
	[sys].[views] AS [View]
	JOIN [sys].[columns] AS [ViewColumn] ON [View].[object_id] = [ViewColumn].[object_id]
	JOIN [sys].[sql_dependencies] [Depends] ON [View].[object_id] = [Depends].[object_id]
	JOIN [sys].[tables] [Table] ON [Depends].[referenced_major_id] = [Table].[object_id]
	JOIN [sys].[columns] [Column] ON [Table].[object_id] = [Column].[object_id]
		AND [Depends].[referenced_minor_id] = [Column].[column_id]
		AND [ViewColumn].[name] = [Column].[name]
	JOIN [sys].[foreign_key_columns] [FKCol] ON [Column].[object_id] = [FKCol].[parent_object_id]
		AND [FKCol].[parent_column_id] = [Column].[column_id]
	JOIN [sys].[foreign_keys] [FK] ON [FKCol].[constraint_object_id] = [FK].[object_id]
	JOIN [sys].[tables] [ParentFKTable] ON [FKCol].[parent_object_id] = [ParentFKTable].[object_id]
	JOIN [sys].[columns] [ParentFKCol] ON [FKCol].[parent_object_id] = [ParentFKCol].[object_id]
		AND [FKCol].[parent_column_id] = [ParentFKCol].[column_id]
	JOIN [sys].[tables] [ReferencedFKTable] ON [FKCol].[referenced_object_id] = [ReferencedFKTable].[object_id]
	JOIN [sys].[columns] [ReferencedFKCol] ON [FKCol].[referenced_object_id] = [ReferencedFKCol].[object_id]
		AND [FKCol].[referenced_column_id] = [ReferencedFKCol].[column_id]
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

    public SqlViewForeignKeyDiscoverer(IDatabaseInterface dbInterface) : base(dbInterface) { }

    #endregion ctors
  }
}