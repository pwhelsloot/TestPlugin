using AMCS.Data.Schema.Discovery.Sql;
using AMCS.Data.Schema.Sql;
using System.Collections.Generic;
using AMCS.Data.Schema.Access;
using AMCS.Data.Schema.Access.SqlServer;
using AMCS.Data.Schema.Configuration;

namespace AMCS.Data.Schema.Discovery.Wcf.Service
{
  public class SchemaDiscoveryService : ISchemaDiscoveryService
  {
    #region Properties/Variables

    private IDatabaseInterface _dbInterface;

    #endregion Properties/Variables

    #region ctors

    public SchemaDiscoveryService()
    {
      var connectionString = DataServices.Resolve<ISchemaDiscoveryConfiguration>().ConnectionString;
      _dbInterface = new SqlServerInterface(connectionString.GetConnectionString());
    }

    #endregion ctors

    #region Table

    public ITable DiscoverTable(string schema, string tableName)
    {
      return (SqlTable)new SqlTableSchemaDiscoverer(_dbInterface, new SqlColumnSchemaDiscoverer(_dbInterface, new SqlKeyDiscoverer(_dbInterface, new SqlForeignKeyDiscoverer(_dbInterface)))).Discover(schema, tableName);
    }

    public IList<ITable> DiscoverTables(string schema)
    {
      return new SqlTableSchemaDiscoverer(_dbInterface, new SqlColumnSchemaDiscoverer(_dbInterface, new SqlKeyDiscoverer(_dbInterface, new SqlForeignKeyDiscoverer(_dbInterface)))).Discover(schema);
    }

    #endregion Table

    #region View

    public IView DiscoverView(string schema, string viewName)
    {
      return new SqlViewSchemaDiscoverer(_dbInterface, new SqlColumnSchemaDiscoverer(_dbInterface, new SqlViewKeyDiscoverer(_dbInterface, new SqlViewForeignKeyDiscoverer(_dbInterface)))).Discover(schema, viewName);
    }

    public IList<IView> DiscoverViews(string schema)
    {
      return new SqlViewSchemaDiscoverer(_dbInterface, new SqlColumnSchemaDiscoverer(_dbInterface, new SqlViewKeyDiscoverer(_dbInterface, new SqlViewForeignKeyDiscoverer(_dbInterface)))).Discover(schema);
    }

    #endregion View

    #region Column

    public IColumn DiscoverColumn(string schema, string queryableObject, string column)
    {
      return new SqlColumnSchemaDiscoverer(_dbInterface, new SqlKeyDiscoverer(_dbInterface, new SqlForeignKeyDiscoverer(_dbInterface))).Discover(schema, queryableObject, column);
    }

    public IList<IColumn> DiscoverColumns(string schema, string queryableObject)
    {
      return new SqlColumnSchemaDiscoverer(_dbInterface, new SqlKeyDiscoverer(_dbInterface, new SqlForeignKeyDiscoverer(_dbInterface))).Discover(schema, queryableObject);
    }

    #endregion Column

    #region Key

    public IList<IKey> DiscoverKeysForColumn(string schema, string table, string column, IEnumerable<KeyType> restrictToKeyTypes = null)
    {
      return new SqlKeyDiscoverer(_dbInterface, new SqlForeignKeyDiscoverer(_dbInterface)).Discover(schema, table, column, restrictToKeyTypes);
    }

    public IList<IKey> DiscoverKeysForTable(string schema, string table, IEnumerable<KeyType> restrictToKeyTypes = null)
    {
      return new SqlKeyDiscoverer(_dbInterface, new SqlForeignKeyDiscoverer(_dbInterface)).Discover(schema, table, restrictToKeyTypes);
    }

    #endregion Key

    #region Foreign Key

    public IList<IForeignKey> DiscoverForeignKeysForColumn(string schema, string table, string column)
    {
      return new SqlForeignKeyDiscoverer(_dbInterface).Discover(schema, table, column);
    }

    public IList<IForeignKey> DiscoverForeignKeysForTable(string schema, string table)
    {
      return new SqlForeignKeyDiscoverer(_dbInterface).Discover(schema, table);
    }

    #endregion Foreign Key

    #region Routine

    public IList<IRoutine> DiscoverAllRoutines()
    {
      return new SqlRoutineDiscoverer(_dbInterface).Discover();
    }

    public IList<IRoutine> DiscoverRoutinesInSchema(string schema)
    {
      return new SqlRoutineDiscoverer(_dbInterface).Discover(schema);
    }

    public IList<IRoutine> DiscoverRoutinesOfTypeInSchema(string schema, RoutineType routineType)
    {
      return new SqlRoutineDiscoverer(_dbInterface).Discover(schema, routineType);
    }

    public IList<IRoutine> DiscoverRoutinesOfType(RoutineType routineType)
    {
      return new SqlRoutineDiscoverer(_dbInterface).Discover(routineType);
    }

    public IRoutine DiscoverRoutine(string schema, RoutineType routineType, string name)
    {
      return new SqlRoutineDiscoverer(_dbInterface).Discover(schema, routineType, name);
    }

    #endregion Routine

    #region Stored Procedure

    public IList<IStoredProcedure> DiscoverAllStoredProcedures()
    {
      return new SqlStoredProcedureDiscoverer(_dbInterface).Discover();
    }

    public IList<IStoredProcedure> DiscoverStoredProceduresInSchema(string schema)
    {
      return new SqlStoredProcedureDiscoverer(_dbInterface).Discover(schema);
    }

    public IStoredProcedure DiscoverStoredProcedure(string schema, string name)
    {
      return new SqlStoredProcedureDiscoverer(_dbInterface).Discover(schema, name);
    }

    #endregion Stored Procedure

    #region Function

    public IList<IFunction> DiscoverAllFunctions()
    {
      return new SqlFunctionDiscoverer(_dbInterface).Discover();
    }

    public IList<IFunction> DiscoverFunctionsOfType(FunctionType functionType)
    {
      return new SqlFunctionDiscoverer(_dbInterface).Discover(functionType);
    }

    public IList<IFunction> DiscoverFunctionsInSchema(string schema)
    {
      return new SqlFunctionDiscoverer(_dbInterface).Discover(schema);
    }

    public IList<IFunction> DiscoverFunctionsOfTypeInSchema(string schema, FunctionType functionType)
    {
      return new SqlFunctionDiscoverer(_dbInterface).Discover(schema, functionType);
    }

    public IFunction DiscoverFunction(string schema, string name)
    {
      return new SqlFunctionDiscoverer(_dbInterface).Discover(schema, name);
    }

    #endregion Function
  }
}
