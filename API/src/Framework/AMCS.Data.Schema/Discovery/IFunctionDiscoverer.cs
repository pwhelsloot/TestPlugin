
using System.Collections.Generic;
namespace AMCS.Data.Schema.Discovery
{
  /// <summary>
  /// Move this into a lower level library if we ever work with other DB type, this isn't SQL Server specific
  /// </summary>
  public interface IFunctionDiscoverer
  {
    IList<IFunction> Discover();
    IList<IFunction> Discover(FunctionType functionType);
    IList<IFunction> Discover(string schema);
    IList<IFunction> Discover(string schema, FunctionType functionType);
    IFunction Discover(string schema, string name);
  }
}
