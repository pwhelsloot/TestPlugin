using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService
{
  /// <summary>
  /// Builder to register a number of routes for a single controller.
  /// </summary>
  public interface IApiRouteBuilder
  {
    /// <summary>
    /// Map both an MVC route and an API explorer route using <see cref="MapMvcRoute(string, string, string)"/>
    /// and <see cref="MapExplorerRoute(string, string, string, string, Type)"/>.
    /// </summary>
    /// <param name="name">The name of the action.</param>
    /// <param name="route">The route to register the action with.</param>
    /// <param name="method">The HTTP method to register the route with.</param>
    /// <param name="responseType">The response type of the method for metadata generation.</param>
    void MapRoute(string name, string route, string method, Type responseType);

    /// <summary>
    /// Map an MVC route (i.e. a real REST API route).
    /// </summary>
    /// <param name="name">The name of the action.</param>
    /// <param name="route">The route to register the action with.</param>
    /// <param name="method">The HTTP method to register the route with.</param>
    void MapMvcRoute(string name, string route, string method);

    /// <summary>
    /// Map an API explorer route (i.e. a Swagger route).
    /// </summary>
    /// <param name="name">The name of the action.</param>
    /// <param name="methodName">The name of the method on the controller that handles the
    /// route, if it deviates from the name of the action.</param>
    /// <param name="route">The route to register the action with.</param>
    /// <param name="method">The HTTP method to register the route with.</param>
    /// <param name="responseType">The response type of the method for metadata generation.</param>
    void MapExplorerRoute(string name, string methodName, string route, string method, Type responseType);
  }
}
