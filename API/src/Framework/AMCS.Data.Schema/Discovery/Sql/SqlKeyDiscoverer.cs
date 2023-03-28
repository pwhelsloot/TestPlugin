using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AMCS.Data.Schema.Access;
using AMCS.Data.Schema.Sql;

namespace AMCS.Data.Schema.Discovery.Sql
{
  public class SqlKeyDiscoverer: SqlDiscoverer, IKeyDiscoverer
  {
    #region Constants

    private const string ByPassColumnName = "@@@Bypass@@@";
    private const string PrimaryKeyTypeIdentifier = "PK";
    private const string UniqueKeyTypeIdentifier = "UQ";

    protected virtual string GetKeyDetailsSql
    {
      get
      {
        return
      @"SELECT
		    SCHEMA_NAME([Table].[schema_id]) AS [TableSchema], [Table].[name] AS [TableName], [Column].[name] AS [ColumnName], [Key].[name] AS [KeyName], [Key].[type] AS [KeyType]
	    FROM
		    [sys].[tables] [Table]
		    JOIN [sys].[columns] [Column] ON [Table].[object_id] = [Column].[object_id]
		    JOIN [sys].[index_columns] [Index] ON [Table].[object_id] = [Index].[object_id]
			    AND [Column].[column_id] = [Index].[column_id]
		    JOIN [sys].[key_constraints] [Key] ON [Table].[object_id] = [Key].[parent_object_id]
			    AND [Index].[index_id] = [Key].[unique_index_id]
	    WHERE
        SCHEMA_NAME([Table].[schema_id]) = @Schema
		    AND [Table].[name] = @QueryableObjectName
		    AND 
		    (
			    @Column = NULL
			    OR [Column].[name] = @Column
		    )";
      }
    }

    #endregion Constants

    #region Properties/Variables

    private IForeignKeyDiscoverer _fkDiscoverer;
    private IDatabaseInterface _dbInterface;

    #endregion Properties/Variables

    #region ctors

    public SqlKeyDiscoverer(IDatabaseInterface dbInterface, IForeignKeyDiscoverer fkDiscoverer = null)
    {
      if (dbInterface == null)
        throw new ArgumentNullException(nameof(dbInterface));
      _dbInterface = dbInterface;
      _fkDiscoverer = fkDiscoverer;
    }

    #endregion ctors

    #region IKeyDiscoverer

    public IList<IKey> Discover(string schema, string table, IEnumerable<KeyType> restrictToKeyTypes = null)
    {
      return Discover(schema, table, ByPassColumnName, restrictToKeyTypes);
    }

    public IList<IKey> Discover(string schema, string table, string column, IEnumerable<KeyType> restrictToKeyTypes = null)
    {
      if (schema == null)
        throw new ArgumentNullException(nameof(schema));
      if (table == null)
        throw new ArgumentNullException(nameof(table));
      if (column == null)
        throw new ArgumentNullException(nameof(column));

      if (_fkDiscoverer == null && (restrictToKeyTypes == null || !restrictToKeyTypes.Any() || restrictToKeyTypes.FirstOrDefault(kt => kt == KeyType.ForeignKey) != default(KeyType)))
        throw new ArgumentNullException(string.Format("Unless you specify a restricted list of key types (which doesn't include '{0}') then you must provide an '{1}'", KeyType.ForeignKey.ToString(), typeof(IForeignKeyDiscoverer)));

      List<IKey> keys = new List<IKey>();

      schema = SanitiseObjectName(schema);
      table = SanitiseObjectName(table);
      column = SanitiseObjectName(column);

      Dictionary<string, object> parameters = new Dictionary<string, object>();
      parameters.Add("@Schema", schema);
      parameters.Add("@QueryableObjectName", table);
      parameters.Add("@Column", column == ByPassColumnName ? (object)DBNull.Value : column);
      
      using(DataTable dataTable = _dbInterface.GetDataTable(GetKeyDetailsSql, parameters))
      {
        if (dataTable != null && dataTable.Rows != null && dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in dataTable.Rows)
          {
            try
            {
              IKey key = ConstructKey(row.Field<string>("KeyType"), row.Field<string>("KeyName"), row.Field<string>("TableSchema"), row.Field<string>("TableName"), row.Field<string>("ColumnName"), restrictToKeyTypes);
              if (key != null)
              {
                if (!TryStoreCompositeKey(key, keys))
                  keys.Add(key);
              }
            }
            catch (Exception ex)
            {
              throw new DataException(string.Format("Failed to read foreign key data for '{0}.{1}{2}'", schema, table, column == ByPassColumnName ? "" : "." + column), ex);
            }
          }
        }
      }

      if (restrictToKeyTypes == null || restrictToKeyTypes.FirstOrDefault(kt => kt == KeyType.ForeignKey) == default(KeyType))
      {
        IList<IForeignKey> foreignKeys = _fkDiscoverer.Discover(schema, table, column);
        if (foreignKeys != null && foreignKeys.Count > 0)
        {
          foreach (IForeignKey fk in foreignKeys)
            keys.Add(fk);
        }
      }
      return keys;
    }

    #endregion IKeyDiscoverer

    #region Helper

    private IKey ConstructKey(string keyType, string keyName, string tableSchema, string tableName, string columnName, IEnumerable<KeyType> restrictToKeyTypes)
    {
      IKey key = null;
      if (keyType == PrimaryKeyTypeIdentifier)
      {
        if (restrictToKeyTypes == null || restrictToKeyTypes.FirstOrDefault(kt => kt == KeyType.PrimaryKey) == default(KeyType))
          key = new SqlPrimaryKey(keyName, tableSchema, tableName, new List<string>() { columnName });
      }
      else if (keyType == UniqueKeyTypeIdentifier)
      {
        if (restrictToKeyTypes == null || restrictToKeyTypes.FirstOrDefault(kt => kt == KeyType.UniqueKey) == default(KeyType))
          key = new SqlUniqueKey(keyName, tableSchema, tableName, new List<string>() { columnName });
      }
      else
        throw new DataException(string.Format("Encountered unexpected key type of '{0}'", keyType));
      return key;
    }

    private bool TryStoreCompositeKey(IKey key, IEnumerable<IKey> knownKeys)
    {
      if (key == null)
        throw new ArgumentNullException(nameof(key));
      if (knownKeys == null)
        throw new ArgumentNullException(nameof(knownKeys));

      foreach (IKey knownKey in knownKeys)
      {
        if (knownKey.Name == key.Name)
        {
          if (key.ColumnNames == null || key.ColumnNames.Count != 1)
            throw new InvalidOperationException(string.Format("Unexpected data encountered when constructing composite key '{0}'", key.Name));
          knownKey.ColumnNames.Add(key.ColumnNames[0]);
          return true;
        }
      }
      return false;
    }

    #endregion Helper
  }
}
