using System.Collections.Generic;
using System.Linq;
using AMCS.Data.Schema.Access;
using AMCS.Data.Schema.Sql;

namespace AMCS.Data.Schema.Discovery.Sql
{
  public class SqlViewSchemaDiscoverer : SqlQueryableObjectSchemaDiscoverer, IViewSchemaDiscoverer
  {
    #region Properties/Variables

    protected override bool GetTables { get { return false; } }
    protected override bool GetViews { get { return true; } }

    #endregion Properties/Variables

    #region ctors

    public SqlViewSchemaDiscoverer(IDatabaseInterface dbInterface, IColumnSchemaDiscoverer columnSchemaDiscoverer)
      : base(dbInterface, columnSchemaDiscoverer)
    {
    }

    #endregion ctors

    #region Methods

    protected override IQueryableObject ConstructQueryableObject(string schema, string queryableObjectName, IList<IColumn> columns)
    {
      return new SqlView(string.Format("{0}.{1}", schema, queryableObjectName), columns);
    }

    public new IView Discover(string schema, string viewName)
    {
      return (IView)base.Discover(schema, viewName);
    }

    public new IList<IView> Discover(string schema)
    {
      return base.Discover(schema).Cast<IView>().ToList();
    }

    #endregion Methods
  }
}
