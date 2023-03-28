using System.Collections.Generic;
using System.ServiceModel;
using AMCS.Data.Schema.Sql;

namespace AMCS.Data.Schema.Discovery.Wcf
{
  #region ServiceKnownType

  [ServiceKnownType(typeof(SqlColumn))]
  [ServiceKnownType(typeof(SqlConstraint))]
  [ServiceKnownType(typeof(SqlDatabaseObject))]
  [ServiceKnownType(typeof(SqlDataType))]
  [ServiceKnownType(typeof(SqlForeignKey))]
  [ServiceKnownType(typeof(SqlFunction))]
  [ServiceKnownType(typeof(SqlKey))]
  [ServiceKnownType(typeof(SqlPrimaryKey))]
  [ServiceKnownType(typeof(SqlQueryableObject))]
  [ServiceKnownType(typeof(SqlRoutine))]
  [ServiceKnownType(typeof(SqlStoredProcedure))]
  [ServiceKnownType(typeof(SqlTable))]
  [ServiceKnownType(typeof(SqlUniqueKey))]
  [ServiceKnownType(typeof(SqlView))]

  #endregion ServiceKnownType

  [ServiceContract]
  public interface ISchemaDiscoveryService
  {
    #region Table

    [OperationContract]
    ITable DiscoverTable(string schema, string tableName);

    [OperationContract]
    IList<ITable> DiscoverTables(string schema);

    #endregion Table

    #region View

    [OperationContract]
    IView DiscoverView(string schema, string viewName);

    [OperationContract]
    IList<IView> DiscoverViews(string schema);

    #endregion View

    #region Column

    [OperationContract]
    IColumn DiscoverColumn(string schema, string table, string column);

    [OperationContract]
    IList<IColumn> DiscoverColumns(string schema, string table);

    #endregion Column

    #region Key

    [OperationContract]
    IList<IKey> DiscoverKeysForColumn(string schema, string table, string column, IEnumerable<KeyType> restrictToKeyTypes = null);

    [OperationContract]
    IList<IKey> DiscoverKeysForTable(string schema, string table, IEnumerable<KeyType> restrictToKeyTypes = null);

    #endregion Key

    #region Foreign Key

    [OperationContract]
    IList<IForeignKey> DiscoverForeignKeysForColumn(string schema, string table, string column);

    [OperationContract]
    IList<IForeignKey> DiscoverForeignKeysForTable(string schema, string table);

    #endregion Foreign Key

    #region Routine

    [OperationContract]
    IList<IRoutine> DiscoverAllRoutines();

    [OperationContract]
    IList<IRoutine> DiscoverRoutinesInSchema(string schema);

    [OperationContract]
    IList<IRoutine> DiscoverRoutinesOfTypeInSchema(string schema, RoutineType routineType);

    [OperationContract]
    IList<IRoutine> DiscoverRoutinesOfType(RoutineType routineType);

    [OperationContract]
    IRoutine DiscoverRoutine(string schema, RoutineType routineType, string name);

    #endregion Routine

    #region Stored Procedure

    [OperationContract]
    IList<IStoredProcedure> DiscoverAllStoredProcedures();

    [OperationContract]
    IList<IStoredProcedure> DiscoverStoredProceduresInSchema(string schema);

    [OperationContract]
    IStoredProcedure DiscoverStoredProcedure(string schema, string name);

    #endregion Stored Procedure

    #region Function

    [OperationContract]
    IList<IFunction> DiscoverAllFunctions();

    [OperationContract]
    IList<IFunction> DiscoverFunctionsOfType(FunctionType functionType);

    [OperationContract]
    IList<IFunction> DiscoverFunctionsInSchema(string schema);

    [OperationContract]
    IList<IFunction> DiscoverFunctionsOfTypeInSchema(string schema, FunctionType functionType);

    [OperationContract]
    IFunction DiscoverFunction(string schema, string name);

    #endregion Function
  }
}
