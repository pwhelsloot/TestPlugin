namespace AMCS.TestPlugin.Server.Configuration
{
  using System.Diagnostics.CodeAnalysis;
  using AMCS.Data.Configuration;
  using AMCS.Data.Server.Configuration;
  using AMCS.TestPlugin.Server.Entity;
  using AMCS.Data.Server.Services;
  using AMCS.ApiService;
  using AMCS.ApiService.Configuration;
  using AMCS.TestPlugin.Server.Services;
  using AMCS.WebDiagnostics;
  using Autofac;

  public static class DataConfigurationExtensions
  {
    [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Suppressed for the template project only.")]
    public static void ConfigureTestPlugin(this DataConfiguration self, ITestPluginConfiguration configuration, IConnectionString connectionString)
    {
      self.ContainerBuilder
        .RegisterInstance(configuration)
        .As<ITestPluginConfiguration>();

      self.ContainerBuilder
        .RegisterType<UserService>()
        .As<IUserService>()
        .SingleInstance();

      self.ConfigureEntityObjectMapper();
    }

    public static void ConfigurePlatform(this DataConfiguration self, IPlatformConfiguration configuration, IPlatformUIConfiguration uiConfiguration)
    {
      self.ContainerBuilder
        .RegisterInstance(configuration)
        .As<IPlatformConfiguration>();

      self.ContainerBuilder
        .RegisterInstance(uiConfiguration)
        .As<IPlatformUIConfiguration>();
    }

    public static void SetApiServices(this DataConfiguration self, TypeManager serverTypes)
    {
      self.ConfigureApiServices(
        serverTypes,
        LoadApiVersions(),
        LoadApiMetadata());
    }

    private static IControllerProviderFactory LoadApiMetadata()
    {
      return new ResourceControllerProviderFactory(
        typeof(UserEntity).Assembly,
        typeof(ErrorCode).Namespace + ".ApiMetadata.xml");
    }

    private static ApiVersionProvider LoadApiVersions()
    {
      return ApiVersionProvider.LoadVersions(
        typeof(ErrorCode).Assembly,
        typeof(ErrorCode).Namespace + ".ApiMetadata.xml");
    }

    public static void SetDiagnostics(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<DiagnosticsManager>()
        .SingleInstance()
        .As<IDiagnosticsManager>();
    }

    public static void ConfigureEntityObjectMapper(this DataConfiguration self)
    {
    }
  }
}
