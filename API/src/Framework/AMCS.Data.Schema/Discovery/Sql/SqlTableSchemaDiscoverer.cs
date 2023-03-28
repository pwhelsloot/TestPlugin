using System.Collections.Generic;
using System.Linq;
using AMCS.Data.Schema.Access;
using AMCS.Data.Schema.Sql;

namespace AMCS.Data.Schema.Discovery.Sql
{
  public class SqlTableSchemaDiscoverer : SqlQueryableObjectSchemaDiscoverer, ITableSchemaDiscoverer
  {
    #region Properties/Variables

    protected override bool GetTables { get { return true; } }
    protected override bool GetViews { get { return false; } }

    #endregion Properties/Variables

    #region ctors

    public SqlTableSchemaDiscoverer(IDatabaseInterface dbInterface, IColumnSchemaDiscoverer columnSchemaDiscoverer)
      : base(dbInterface, columnSchemaDiscoverer)
    {
    }

    #endregion ctors

    #region Methods

    protected override IQueryableObject ConstructQueryableObject(string schema, string queryableObjectName, IList<IColumn> columns)
    {
      return new SqlTable(string.Format("{0}.{1}", schema, queryableObjectName), columns);
    }

    public new ITable Discover(string schema, string tableName)
    {
      return (ITable)base.Discover(schema, tableName);
    }

    public new IList<ITable> Discover(string schema)
    {
      return base.Discover(schema).Cast<ITable>().ToList();
    }

    #endregion Methods
  }
}
