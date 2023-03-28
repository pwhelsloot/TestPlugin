using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Abstractions.MvcSetup;
using AMCS.ApiService.CommsServer;
using AMCS.ApiService.Documentation.Abstractions.Swagger;
using AMCS.ApiService.Elemos;
using AMCS.ApiService.MvcSetup;
using AMCS.Data.Configuration;
using AMCS.Data.Server.Broadcast;
using AMCS.Data.Server.Configuration;
using AMCS.Data.Server.Services;
using AMCS.WebDiagnostics;
using Autofac;

namespace AMCS.ApiService.Configuration
{
  public static class DataConfigurationExtensions
  {
    public static void ConfigureApiServices(this DataConfiguration self, TypeManager controllerTypes, ApiVersionProvider apiVersionProvider, params IControllerProviderFactory[] controllerProviderFactories)
    {
      ConfigureApiServices<AppSetupService>(self, controllerTypes, apiVersionProvider, controllerProviderFactories);
    }

    public static void ConfigureApiServices<TAppSetupService>(this DataConfiguration self, TypeManager controllerTypes,
      ApiVersionProvider apiVersionProvider, params IControllerProviderFactory[] controllerProviderFactories)
      where TAppSetupService : IAppSetupService
    {
      self.ContainerBuilder
        .RegisterType<AuthenticationService>()
        .SingleInstance()
        .As<IAuthenticationService>();

      self.ContainerBuilder
        .RegisterInstance(new EntityObjectMetadataManager())
        .As<IEntityObjectMetadataManager>();

      EnsureOwinSetupService<TAppSetupService>(self);

      EnsureMvcSetupService(self);

      self.ContainerBuilder
        .Register(p => new ApiServiceManagerConfiguration(controllerProviderFactories, controllerTypes, p.Resolve<IAppSetupService>(), p.Resolve<IMvcSetupService>(), p.ResolveOptional<IApiExplorerConfiguration>(), apiVersionProvider))
        .SingleInstance()
        .AutoActivate()
        .AsSelf();
    }

    internal static void ConfigureSetupServices(this DataConfiguration self)
    {
      EnsureOwinSetupService<AppSetupService>(self);

      EnsureMvcSetupService(self);
    }

    public static void ConfigureCommsServerProtocols(this DataConfiguration self, TypeManager protocolTypes, ICommsServerProtocolsConfiguration configuration, bool allowAlternateTransport)
    {
      self.ContainerBuilder
        .Register(p => new CommsServerProtocolManager(protocolTypes, configuration, p.Resolve<IServiceRootResolver>(), p.Resolve<IDiagnosticsManager>(), allowAlternateTransport, p.Resolve<IBroadcastService>()))
        .SingleInstance()
        .AutoActivate()
        .AsSelf()
        .As<ICommsServerProtocolManager>();

      self.ContainerBuilder
        .Register(p => new CommsServerProtocolService(p.Resolve<CommsServerProtocolManager>(), p.Resolve<IBroadcastService>(), protocolTypes))
        .SingleInstance()
        .AutoActivate()
        .AsSelf()
        .As<ICommsServerProtocolService>();

      foreach (var type in protocolTypes.GetTypes().Where(p => p.IsClass && !p.IsAbstract))
      {
        if (!typeof(ICommsServerProtocolFactory).IsAssignableFrom(type))
          continue;

        if (type.IsGenericTypeDefinition)
        {
          self.ContainerBuilder
            .RegisterGeneric(type)
            .As(type.GetInterfaces())
            .ExternallyOwned()
            .SingleInstance();
        }
        else
        {
          self.ContainerBuilder
            .RegisterType(type)
            .As(type.GetInterfaces())
            .ExternallyOwned()
            .SingleInstance();
        }
      }
    }

    public static void ConfigureCommsServerSyncSatus<TCommsServerSyncStatusService>(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<TCommsServerSyncStatusService>()
        .SingleInstance()
        .AutoActivate()
        .As<ICommsServerSyncStatusService>()
        .AsSelf();
    }

    /// <summary>
    /// This should only ever be called when we are setting up an API.
    /// It should never be called from a Test project
    /// </summary>
    /// <param name="self"></param>
    private static void EnsureMvcSetupService(DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<MvcSetupService>()
        .SingleInstance()
        .AsSelf()
        .As<IMvcSetupService>()
        .IfNotRegistered(typeof(IMvcSetupService));
    }

    /// <summary>
    /// This should only ever be called when we are setting up an API.
    /// It should never be called from a Test project
    /// </summary>
    /// <param name="self"></param>
    private static void EnsureOwinSetupService<TAppSetupService>(DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<TAppSetupService>()
        .SingleInstance()
        .AsSelf()
        .As<IAppSetupService>()
        .IfNotRegistered(typeof(IAppSetupService));
    }

    public static void EnsureWebSocketsEnabled(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<WebSocketsService>()
        .SingleInstance()
        .AutoActivate()
        .AsSelf()
        .IfNotRegistered(typeof(WebSocketsService));
    }
  }
}
