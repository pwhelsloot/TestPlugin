using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Abstractions;
using AMCS.ApiService.Elemos;

namespace AMCS.ApiService
{
  /// <summary>
  /// Manager for setting up MVC (i.e. controllers) and API explorer (i.e. Swagger)
  /// public REST routes.
  /// </summary>
  public interface IRouteCollectionManager
  {
    /// <summary>
    /// Get a route builder for a specific (entity object) controller.
    /// </summary>
    /// <param name="controllerName">The name of the controller.</param>
    /// <param name="routeName">The parent route path to register the routes underneath.</param>
    /// <param name="controllerType">The specific <see cref="EntityObjectServiceController{T}"/>
    /// that handles the routes.</param>
    /// <returns>The <see cref="IApiRouteBuilder"/>.</returns>
    IApiRouteBuilder GetRouteBuilder(string controllerName, string routeName, Type controllerType);

    /// <summary>
    /// Maps a service route (i.e. an <see cref="IMessageService{TRequest, TResponse}"/>
    /// route).
    /// </summary>
    /// <param name="controllerName">The name of the controller.</param>
    /// <param name="route">The route to register the controller method.</param>
    /// <param name="controllerType">The specific <see cref="MessageServiceController{TService, TRequest, TResponse}"/>.</param>
    /// <param name="responseType">The type the service returns for API explorer registration.</param>
    void MapServiceRoute(string controllerName, string route, Type controllerType, Type responseType);
  }
}
