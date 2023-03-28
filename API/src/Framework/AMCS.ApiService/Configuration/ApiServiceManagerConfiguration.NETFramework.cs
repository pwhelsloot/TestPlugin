#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using AMCS.ApiService.Abstractions.MvcSetup;
using AMCS.ApiService.Documentation.Abstractions.Swagger;
using AMCS.ApiService.MvcSetup;
using AMCS.ApiService.Support;
using AMCS.Data.Configuration;
using AMCS.Data.Server.Services;

namespace AMCS.ApiService.Configuration
{
  internal class ApiServiceManagerConfiguration
  {
    public const string SSOAuthenticationType = "SSO";

    private readonly IControllerProviderFactory[] controllerProviderFactories;
    private readonly IApiExplorerConfiguration apiExplorer;
    private readonly ApiVersionProvider apiVersionProvider;
    private readonly TypeManager controllerTypes;

    public ApiServiceManagerConfiguration(IControllerProviderFactory[] controllerProviderFactories, TypeManager controllerTypes, IAppSetupService setupService, IMvcSetupService mvcSetup, IApiExplorerConfiguration apiExplorer, ApiVersionProvider apiVersionProvider)
    {
      this.controllerProviderFactories = controllerProviderFactories;
      this.apiExplorer = apiExplorer;
      this.apiVersionProvider = apiVersionProvider;
      this.controllerTypes = controllerTypes;

      mvcSetup.Register(SetupMvc);
    }

    private void SetupMvc(IMvcSetup setup)
    {
      var controllerProviders = controllerProviderFactories.Select(p => p.Create()).ToArray();

      RegisterGlobalFilters(setup.Filters);
      RegisterRoutes(setup.Routes, controllerProviders);
    }

    private void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new CurrentUICultureAttribute());
      filters.Add(new HandleApiErrorAttribute());
      filters.Add(new AllowCrossSiteJsonAttribute());
      filters.Add(new ApiVersionAttribute(apiVersionProvider));
    }

    private void RegisterRoutes(RouteCollection routes, IControllerProvider[] controllerProviders)
    {
      ControllerBuilder.Current.SetControllerFactory(new ControllerFactory(controllerProviders));

      var routesManager = new RouteCollectionManager(routes, apiExplorer);

      foreach (var provider in controllerProviders)
      {
        provider.RegisterRoutes(routesManager);
      }
    }

    private class ControllerFactory : DefaultControllerFactory
    {
      private readonly IControllerProvider[] controllerProviders;

      public ControllerFactory(IControllerProvider[] controllerProviders)
      {
        this.controllerProviders = controllerProviders;
      }

      protected override Type GetControllerType(RequestContext requestContext, string controllerName)
      {
        foreach (var provider in controllerProviders)
        {
          var type = provider.GetControllerType(controllerName);
          if (type != null)
            return type;
        }

        return base.GetControllerType(requestContext, controllerName);
      }
    }
  }
}

#endif
