using System.Collections.Generic;

namespace AMCS.Data.Schema.Discovery
{
  public interface IViewSchemaDiscoverer: IQueryableObjectSchemaDiscoverer
  {
    new IView Discover(string schema, string viewName);
    new IList<IView> Discover(string schema);
  }
}
