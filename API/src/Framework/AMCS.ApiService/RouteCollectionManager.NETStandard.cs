﻿#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Documentation.Abstractions.Swagger;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing.Constraints;

namespace AMCS.ApiService
{
  internal class RouteCollectionManager : IRouteCollectionManager
  {
    private readonly IRouteBuilder routes;
    private readonly IApiExplorerConfiguration apiExplorer;

    public RouteCollectionManager(IRouteBuilder routes, IApiExplorerConfiguration apiExplorer)
    {
      this.routes = routes;
      this.apiExplorer = apiExplorer;
    }

    public IApiRouteBuilder GetRouteBuilder(string controllerName, string routeName, Type controllerType)
    {
      return new ApiRouteBuilder(routes, controllerName, routeName, controllerType, apiExplorer);
    }

    public void MapServiceRoute(string controllerName, string route, Type controllerType, Type responseType)
    {
      routes.MapRoute(
        controllerName + "_Perform",
        route,
        new { controller = controllerName, action = "Perform" },
        new { httpMethod = new HttpMethodRouteConstraint("POST") });

      apiExplorer?.RegisterMessageService(controllerType, responseType, route, controllerName);
    }

    private class ApiRouteBuilder : IApiRouteBuilder
    {
      private readonly IRouteBuilder routes;
      private readonly string controllerName;
      private readonly string routeName;
      private readonly Type controllerType;
      private readonly IApiExplorerConfiguration apiExplorer;

      public ApiRouteBuilder(IRouteBuilder routes, string controllerName, string routeName, Type controllerType, IApiExplorerConfiguration apiExplorer)
      {
        this.routes = routes;
        this.controllerName = controllerName;
        this.routeName = routeName;
        this.controllerType = controllerType;
        this.apiExplorer = apiExplorer;
      }

      public void MapRoute(string name, string route, string method, Type responseType)
      {
        MapMvcRoute(name, route, method);
        MapExplorerRoute(name, null, route, method, responseType);
      }

      public void MapMvcRoute(string name, string route, string method)
      {
        string routeName = this.routeName;
        if (route != null)
          routeName += "/" + route;

        routes.MapRoute(
          controllerName + "_" + name,
          routeName,
          new { controller = controllerName, action = name },
          new { httpMethod = new HttpMethodRouteConstraint(method) });
      }

      public void MapExplorerRoute(string name, string methodName, string route, string method, Type responseType)
      {
        string routeName = this.routeName;
        if (route != null)
          routeName += "/" + route;

        apiExplorer?.RegisterApiAction(new ApiExplorerAction
        {
          ControllerType = controllerType,
          ResponseType = responseType,
          ActionName = name,
          MethodName = methodName,
          HttpVerb = method,
          Route = routeName,
          ControllerName = controllerName
        });
      }
    }
  }
}

#endif
