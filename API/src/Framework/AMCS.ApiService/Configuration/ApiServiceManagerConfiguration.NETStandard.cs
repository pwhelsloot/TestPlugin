#if !NETFRAMEWORK

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AMCS.ApiService.Abstractions.MvcSetup;
using AMCS.ApiService.Documentation.Abstractions.Swagger;
using AMCS.ApiService.Formatters;
using AMCS.ApiService.MvcSetup;
using AMCS.Data;
using AMCS.Data.Configuration;
using AMCS.Data.Server.Plugin;
using AMCS.Data.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace AMCS.ApiService.Configuration
{
  internal class ApiServiceManagerConfiguration
  {
    private readonly ControllerProviderManager controllerProviderManager;
    private readonly IApiExplorerConfiguration apiExplorer;

    public ApiServiceManagerConfiguration(IControllerProviderFactory[] controllerProviderFactories, TypeManager controllerTypes, IAppSetupService setupService, IMvcSetupService mvcSetup, IApiExplorerConfiguration apiExplorer, ApiVersionProvider apiVersionProvider)
    {
      this.controllerProviderManager = new ControllerProviderManager(controllerProviderFactories);
      this.apiExplorer = apiExplorer;

      setupService.RegisterConfigureServices(
        services =>
        {
          services
            .AddAuthentication()
            .AddScheme<ApiServiceAuthenticationOptions, ApiServiceAuthenticationHandler>(
              ApiServiceAuthenticationHandler.SchemeName,
              p =>
              {
                p.AuthenticationService = DataServices.Resolve<IAuthenticationService>();
              });

          services.AddAuthorization(options =>
          {
            options.AddPolicy(ApiPolicy.RequiresCoreIdentity,
              policy => policy.Requirements.Add(new CoreIdentityRequirement()));
          });
        },
        -1000);

      setupService.RegisterConfigure(
        app =>
        {
          app.UseMiddleware<CurrentUICultureMiddleware>();
          app.UseAuthentication();
          app.UseAuthorization();
        },
        -1000);

      mvcSetup.RegisterOptions(p =>
      {
        p.Filters.Add<HandleApiErrorAttribute>();
        p.Filters.Add(new ApiVersionAttribute(apiVersionProvider));
      });

      mvcSetup.RegisterSetup(p =>
      {
        p.AddMvcOptions(p1 => p1.Conventions.Add(new GenericControllerModelConvention(controllerProviderManager)));
        p.AddMvcOptions(p1 => p1.InputFormatters.Insert(0, new RawJsonBodyInputFormatter()));
        p.AddMvcOptions(p1 => p1.InputFormatters.Insert(1, new PluginInputFormatter()));

        p.ConfigureApplicationPartManager(p1 =>
        {
          p1.FeatureProviders.Add(new GenericControllerFeatureProvider(controllerProviderManager));
        });

        foreach(var assembly in controllerTypes.Assemblies)
        {
          p.AddApplicationPart(assembly);
        }
      });

      mvcSetup.RegisterRoutes(RegisterRoutes);
    }

    private void RegisterRoutes(IRouteBuilder routes)
    {
      var routesManager = new RouteCollectionManager(routes, apiExplorer);

      foreach(var provider in controllerProviderManager.GetControllerProviders())
      {
        provider.RegisterRoutes(routesManager);
      }

      controllerProviderManager.BuildTypeMap();
    }

    private class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
      private readonly ControllerProviderManager controllerProviderManager;

      public GenericControllerFeatureProvider(ControllerProviderManager controllerProviderManager)
      {
        this.controllerProviderManager = controllerProviderManager;
      }

      public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
      {
        foreach(var controllerType in controllerProviderManager.Types.Keys)
        {
          feature.Controllers.Add(controllerType);
        }
      }
    }

    private class GenericControllerModelConvention : IControllerModelConvention
    {
      private readonly ControllerProviderManager controllerProviderManager;

      public GenericControllerModelConvention(ControllerProviderManager controllerProviderManager)
      {
        this.controllerProviderManager = controllerProviderManager;
      }

      public void Apply(ControllerModel controller)
      {
        if(controllerProviderManager.Types.TryGetValue(controller.ControllerType, out string name))
        {
          controller.ControllerName = name;
          controller.RouteValues["Controller"] = name;
        }
      }
    }

    private class ControllerProviderManager
    {
      private readonly IControllerProviderFactory[] controllerProviderFactories;
      private IControllerProvider[] controllerProviders;

      public Dictionary<TypeInfo, string> Types { get; private set; }

      public ControllerProviderManager(IControllerProviderFactory[] controllerProviderFactories)
      {
        this.controllerProviderFactories = controllerProviderFactories;
      }

      public IControllerProvider[] GetControllerProviders()
      {
        if(controllerProviders == null)
          controllerProviders = controllerProviderFactories.Select(p => p.Create()).ToArray();

        return controllerProviders;
      }

      public void BuildTypeMap()
      {
        Types = controllerProviders
          .SelectMany(p => p.GetControllerNames().Select(p1 => (Name: p1, Type: p.GetControllerType(p1).GetTypeInfo())))
          .ToDictionary(p => p.Type, p => p.Name);
      }
    }
  }
}

#endif
