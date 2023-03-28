namespace AMCS.Data.Server.Configuration
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Reflection;
  using AMCS.Data.Configuration;
  using AMCS.Data.Entity;
  using AMCS.Data.Entity.Validation;
  using AMCS.Data.Server.Api;
  using AMCS.Data.Server.ApplicationInsights;
  using AMCS.Data.Server.Azure.Helpers;
  using AMCS.Data.Server.AzureServiceBus;
  using AMCS.Data.Server.Broadcast;
  using AMCS.Data.Server.BslTriggers;
  using AMCS.Data.Server.Configuration.Support;
  using AMCS.Data.Server.DataSets;
  using AMCS.Data.Server.DataSets.Support;
  using AMCS.Data.Server.PlatformCredentials;
  using AMCS.Data.Server.Services;
  using AMCS.Data.Server.Synchronisation;
  using AMCS.Data.Server.SystemConfiguration;
  using AMCS.Data.Server.TestData;
  using AMCS.Data.Support;
  using AMCS.Encryption;
  using AMCS.Encryption.BouncyCastle;
  using AMCS.JobSystem;
  using AMCS.JobSystem.Agent;
  using AMCS.JobSystem.Scheduler.Api;
  using AMCS.JobSystem.Scheduler.Data;
  using AMCS.WebDiagnostics;
  using Autofac;
  using Http;
  using Plugin;
  using Plugin.MetadataRegistry.BusinessObject;

  public static partial class DataConfigurationExtensions
  {
    public static void SetRestrictionsService(this DataConfiguration self, IRestrictionService restrictionService)
    {
      self.ContainerBuilder
        .RegisterInstance(restrictionService)
        .As<IRestrictionService>();
    }

    public static void SetSettingsService(this DataConfiguration self, ISettingsService settingsService)
    {
      self.ContainerBuilder
        .RegisterInstance(settingsService)
        .As<ISettingsService>();
    }

    public static void SetUserService(this DataConfiguration self, IUserService userService)
    {
      self.ContainerBuilder
        .RegisterInstance(userService)
        .As<IUserService>();
    }

    public static void SetSystemConfigurationSetupConfiguration(
      this DataConfiguration self,
      IServerConfiguration configuration,
      IConnectionString connectionString,
      TypeManager entityTypes,
      ILanguageResources languageResources,
      StrictModeType strictMode)
    {
      self.ContainerBuilder
        .RegisterInstance(configuration)
        .As<IServerConfiguration>();

      self.ConfigureStrictMode(strictMode);

      self.ContainerBuilder
        .RegisterInstance(new DatabaseTranslationService(languageResources))
        .SingleInstance()
        .AutoActivate()
        .As<IDatabaseTranslationsService>();

      if (!string.IsNullOrEmpty(configuration.DefaultCulture))
      {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture(configuration.DefaultCulture);
      }

      if (!string.IsNullOrEmpty(configuration.DefaultUICulture))
      {
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CreateSpecificCulture(configuration.DefaultUICulture);
      }

      self.ConfigureSqlDataSessionFactory(
        new SQLDataSessionConfiguration(
          connectionString,
          connectionString,
          configuration.CommandTimeout,
          configuration.CommandTimeoutExtended,
          configuration.BulkCopyTimeout,
          configuration.PerformanceMonitoringEnabled,
          configuration.AzureCompatibilityEnabled,
          configuration.ParallelDataSessionThreadCount,
          configuration.AuditTableEnabled.GetValueOrDefault(true)));

      self.SetDataAccessIdService(new SQLDataAccessIdService());

      self.ConfigureMappingManager(
        languageResources,
        entityTypes);

      self.ConfigureEncryptionService(configuration.EncryptionKey);

      self.ConfigureSecurityTokenManager(configuration.EncryptionKey);
      self.ConfigureSessionTokenManager(configuration.SessionExpiration);

      self.ConfigurePlatformCredentialsSecurityTokenManager(configuration);

      self.ConfigurePlatformCredentialsTokenManager(configuration.SessionExpiration);
    }

    public static void SetServerConfiguration(
      this DataConfiguration self,
      IServerConfiguration configuration,
      IConnectionString connectionString,
      IConnectionString reportingConnectionString,
      IConnectionString azureBlobStorageConnectionString,
      string defaultQueue,
      TypeManager entityTypes,
      ILanguageResources languageResources,
      StrictModeType strictMode)
    {
      self.ConfigureStrictMode(strictMode);

      self.ConfigureILGeneration(configuration.DisableILGeneration);

      self.ContainerBuilder
        .RegisterInstance(configuration)
        .As<IServerConfiguration>();

      if (!string.IsNullOrEmpty(configuration.DefaultCulture))
      {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture(configuration.DefaultCulture);
      }

      if (!string.IsNullOrEmpty(configuration.DefaultUICulture))
      {
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CreateSpecificCulture(configuration.DefaultUICulture);
      }

      self.ConfigureDataSessionEventsBuilderService();

      self.ConfigureDataMetricsEventsBuilderService();

      if (configuration.DataAccessType == "SQL")
      {
        self.ConfigureSqlDataSessionFactory(
          new SQLDataSessionConfiguration(
            connectionString,
            reportingConnectionString,
            configuration.CommandTimeout,
            configuration.CommandTimeoutExtended,
            configuration.BulkCopyTimeout,
            configuration.PerformanceMonitoringEnabled,
            configuration.AzureCompatibilityEnabled,
            configuration.ParallelDataSessionThreadCount,
            configuration.AuditTableEnabled.GetValueOrDefault(true)));

        self.SetDataAccessIdService(new SQLDataAccessIdService());
      }
      else if (!string.Equals(configuration.DataAccessType, "none", StringComparison.InvariantCultureIgnoreCase))
      {
        self.ConfigureFakeDataSessionFactory();
      }

      if (!configuration.DataSynchronizationDisabled.GetValueOrDefault())
      {
        self.ContainerBuilder
          .RegisterType<SynchronizationEventRegistrationService>()
          .SingleInstance()
          .AutoActivate();
      }

      if (connectionString != null)
      {
        self.ConfigureMappingManager(
          languageResources,
          entityTypes);
      }

      self.ConfigureEncryptionService(configuration.EncryptionKey);

      self.ConfigureSecurityTokenManager(configuration.EncryptionKey);
      self.ConfigureSessionTokenManager(configuration.SessionExpiration);

      self.ConfigurePlatformCredentialsSecurityTokenManager(configuration);
      self.ConfigurePlatformCredentialsTokenManager(configuration.SessionExpiration);

      self.ConfigureBlobStorage(configuration, azureBlobStorageConnectionString);
      self.ConfigureTempFileService(configuration.TempFileStorage, azureBlobStorageConnectionString);
      self.ConfigureApplicationInsights(configuration.ApplicationInsightsInstrumentationKey);

      self.ConfigureRestApiService();
      self.ConfigureHttpServices();
      self.ConfigureAmcsHttpClient();

#if !NETFRAMEWORK
      if (!string.IsNullOrEmpty(configuration.StaticFilesPath))
      {
        self.ConfigureStaticFiles(configuration.StaticFilesPath, configuration.StaticFilesOutputPath);
      }
#endif

      self.ConfigureAzureServiceBusConnectionManager();
      self.ConfigurePrefixDiagnostic();

      self.EnableEntityHistory();
    }

    public static void ConfigureStrictMode(this DataConfiguration self, StrictModeType strictMode)
    {
      StrictMode.SetStrictMode(strictMode);
    }

    public static void EnableEntityHistory(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<EntityHistoryEventRegistrationService>()
        .SingleInstance()
        .AutoActivate();
    }
  
    public static void ConfigureILGeneration(this DataConfiguration self, bool disableILGeneration)
    {
      ReflectionHelper.DisableILGeneration(disableILGeneration);
    }

    public static void ConfigureDataSessionEventsBuilderService(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterInstance(new DataEventsBuilderService())
        .As<IDataEventsBuilderService>();
    }

    public static void ConfigureBusinessObjects(this DataConfiguration self, Assembly assembly, ITypeManager typeManager)
    {
      self.ContainerBuilder
        .Register(p => new BusinessObjectService(assembly, typeManager, "BusinessObjects.xml"))
        .AsSelf()
        .AutoActivate()
        .As<IBusinessObjectService>()
        .SingleInstance();

      self.AddBusinessObjectMetadataRegistry<BusinessObjectMetadataRegistryService>();
    }

    public static void ConfigurePrefixDiagnostic(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<QueuePrefixDiagnosticService>()
        .As<QueuePrefixDiagnosticService>();
    }

    public static void ConfigureDataMetricsEventsBuilderService(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterInstance(new DataMetricsEventsBuilderService())
        .As<IDataMetricsEventsBuilderService>();
    }

    private static void ConfigureBlobStorage(this DataConfiguration self, IServerConfiguration configuration,
      IConnectionString azureBlobStorageConnectionString)
    {
      // Should only really be set to Enabled / Disabled but there is an option for ReadOnly if we get into a state of
      // the blob storage has been on but it is no longer required to save to the blob storage system and revert back
      // to storing in the database but we still want to be able to access the blobs that have already been saved to
      // the external storage.

      var blobStorageMode = ParseBlobStorageMode(configuration.BlobStorage?.Mode);

      // By default we disable both Azure BLOB storage and File System BLOB storage. Only if the mode is set
      // to Enabled or ReadOnly, do we load the Azure BLOB Storage connection string or File System path.

      string azureBlobStorageConnectionStringValue = null;
      string fileSystemStore = null;

      if (blobStorageMode != BlobStorageMode.Disabled)
      {
        azureBlobStorageConnectionStringValue = azureBlobStorageConnectionString?.GetConnectionString();
        fileSystemStore = configuration.BlobStorage?.FileSystem?.Store;
      }

      // We only enable external storage (i.e. writing to external storage) when BLOB storage is enabled.
      // If it's read only, we still pass the Azure BLOB Storage connection string or File System path,
      // but set ExternalStorage on the IBlobStorageService interface to false. This ensures that
      // writes go to the database, and reads try the storage service if applicable.

      bool enableExternalStorage = blobStorageMode == BlobStorageMode.Enabled;

      if (!string.IsNullOrEmpty(azureBlobStorageConnectionStringValue))
        self.ConfigureAzureBlobStorage(azureBlobStorageConnectionString, configuration.BlobStorage.Azure.Container,
          enableExternalStorage);
      else if (!string.IsNullOrEmpty(fileSystemStore))
        self.ConfigureFileSystemBlobStorage(fileSystemStore, enableExternalStorage);
      else
        self.ConfigureDatabaseBlobStorage();

      BlobStorageMode ParseBlobStorageMode(string value)
      {
        if (string.IsNullOrEmpty(value))
          return BlobStorageMode.Disabled;
        return (BlobStorageMode)Enum.Parse(typeof(BlobStorageMode), value, true);
      }
    }

    public static void ConfigureSqlDataSessionFactory(this DataConfiguration self, SQLDataSessionConfiguration configuration)
    {
      self.ContainerBuilder
        .Register(p => new SQLDataSessionFactory(configuration, p.Resolve<ISetupService>(), p.Resolve<IDataEventsBuilderService>(), p.Resolve<IDataMetricsEventsBuilderService>()))
        .SingleInstance()
        .AutoActivate()
        .AsSelf()
        .As<IDataSessionFactory>();
    }

    public static void ConfigureFakeDataSessionFactory(this DataConfiguration self, StrictModeType strictMode = StrictModeType.None)
    {
      self.SetDataSessionFactory(new FakeDataSessionFactory(strictMode));
    }

    public static void SetDataSessionFactory(this DataConfiguration self, IDataSessionFactory dataSessionFactory)
    {
      self.ContainerBuilder
        .RegisterInstance(dataSessionFactory)
        .As<IDataSessionFactory>();
    }

    public static void ConfigureDataServer(this DataConfiguration self, TypeManager entityTypes, TypeManager serviceTypes)
    {
      self.ContainerBuilder
        .RegisterInstance(new EntityLoadedValuesService())
        .SingleInstance()
        .As<IEntityLoadedValuesService>();

      var fallthroughTypes = new Dictionary<Type, List<Type>>();

      try
      {
        foreach (var type in entityTypes.GetTypes())
        {
          if (typeof(EntityObject).IsAssignableFrom(type))
          {
            Attribute attribute = type.GetCustomAttribute(typeof(FallThroughToParentService), false);

            if (attribute != null)
            {
              var baseType = FindBaseEntityObjectType(type);
              if (!fallthroughTypes.TryGetValue(baseType, out var types))
              {
                types = new List<Type>();
                fallthroughTypes.Add(baseType, types);
              }
              types.Add(type);
            }
          }
        }

        foreach (var type in serviceTypes.GetTypes().Where(p => p.IsClass && !p.IsAbstract))
        {
          if (typeof(IEntityObjectAccess).IsAssignableFrom(type))
          {
            if (type.IsGenericTypeDefinition)
            {
              self.ContainerBuilder
                .RegisterGeneric(type)
                .As(GetInterfaces(type, typeof(IEntityObjectAccess)))
                .ExternallyOwned();
            }
            else
            {
              self.ContainerBuilder
                .RegisterType(type)
                .As(GetInterfaces(type, typeof(IEntityObjectAccess)))
                .ExternallyOwned();
            }
          }

          if (typeof(IEntityObjectService).IsAssignableFrom(type))
          {
            if (type.IsGenericTypeDefinition)
            {
              self.ContainerBuilder
                .RegisterGeneric(type)
                .As(GetInterfaces(type, typeof(IEntityObjectService)))
                .ExternallyOwned();
            }
            else
            {
              self.ContainerBuilder
                .RegisterType(type)
                .As(GetInterfaces(type, typeof(IEntityObjectService)))
                .ExternallyOwned();
            }

            var entityType = type.GetGenericTypeArguments(typeof(IEntityObjectService<>))[0];
            if (fallthroughTypes.TryGetValue(entityType, out var types))
            {
              foreach (var fallthroughType in types)
              {
                var adapterType = typeof(EntityObjectServiceAdapter<,>).MakeGenericType(fallthroughType, entityType);
                self.ContainerBuilder
                  .RegisterType(adapterType)
                  .As(GetInterfaces(adapterType, typeof(IEntityObjectService)))
                  .ExternallyOwned();
              }
            }
          }

          if (typeof(ISearchDataAccessProvider).IsAssignableFrom(type))
          {
            self.ContainerBuilder
              .RegisterType(type)
              .As(GetInterfaces(type, typeof(ISearchDataAccessProvider)))
              .ExternallyOwned();
          }
        }

        self.ContainerBuilder
          .RegisterInstance(new EntityObjectManager(entityTypes))
          .AsSelf();

        self.ContainerBuilder
          .RegisterInstance(new EntityObjectValidator())
          .As<IEntityObjectValidator>();

        self.ConfigureEntityObjectMapperService();
      }
      catch (ReflectionTypeLoadException ex)
      {
        foreach (var item in ex.LoaderExceptions)
        {
          throw item;
        }
      }
    }

    private static Type FindBaseEntityObjectType(Type type)
    {
      if (type.BaseType == typeof(EntityObject))
        throw new InvalidOperationException($"FallThroughToParentService attribute is invalid on '{type}' because it inherits from EntityObject");

      if (type.BaseType.GetCustomAttribute(typeof(FallThroughToParentService), false) != null)
        return FindBaseEntityObjectType(type.BaseType);

      return type.BaseType;
    }

    private static Type[] GetInterfaces(Type type, Type limit)
    {
      if (limit.IsGenericTypeDefinition)
      {
        return type.GetInterfaces()
          .Where(p => p != limit && limit.IsAssignableFromGeneric(p))
          .ToArray();
      }

      return type.GetInterfaces()
        .Where(p => p != limit && limit.IsAssignableFrom(p))
        .ToArray();
    }

    public static void ConfigureMappingManager(this DataConfiguration self, ILanguageResources languageResources,
      TypeManager entityTypes)
    {
      self.ContainerBuilder
        .Register(p => new MappingManagerAccessor(
          p.Resolve<IProjectConfiguration>().ProjectId, languageResources, entityTypes))
        .SingleInstance()
        .AsSelf()
        .As<IMappingManagerAccessor>();
    }

    public static void ConfigureBslTriggers(this DataConfiguration self, string defaultQueue, TypeManager entityTypes,
      TypeManager actionTypes)
    {
      self.ContainerBuilder
        .Register(p => new BslTriggerManager(
          p.ResolveOptional<SchedulerClient>(),
          p.Resolve<IBroadcastService>(),
          defaultQueue,
          entityTypes,
          actionTypes,
          p.Resolve<ISetupService>(),
          p.Resolve<IDataEventsBuilderService>()
        ))
        .SingleInstance()
        .AutoActivate()
        .AsSelf()
        .As<IBslTriggerManager>();

      self.ContainerBuilder
        .RegisterType<BslTriggerCacheService>()
        .SingleInstance()
        .AsSelf()
        .AutoActivate()
        .As<ICacheCoherentEntityService<BslTriggerEntity>>();
    }

    public static void ConfigureJobSystem(this DataConfiguration self, IJobSystemConfiguration configuration,
      IAgentConfiguration agentConfiguration, IConnectionString connectionString,
      IConnectionString messageQueueConnectionString, TypeManager serverTypes)
    {
      var jobExpiredPeriod = TimeSpan.FromSeconds(60);

      var store = SqlServerDataStore.Create(connectionString.GetConnectionString(), jobExpiredPeriod);
      ISchedulerTransportFactory transportFactory;

      // Create a local or remote transport depending on whether we have a message
      // queue connection string.
      if (messageQueueConnectionString != null)
      {
        transportFactory = new SchedulerRemoteTransportFactory(new SchedulerRemoteTransportFactoryConfiguration
        {
          ConnectionString = messageQueueConnectionString.GetConnectionString(),
          InstanceName = AzureHelpers.GenerateInstanceName(),
          QueuePrefix = GetNonEmptyString(configuration.QueuePrefix, agentConfiguration?.QueuePrefix),
          AutoDeleteOnIdle = TimeSpan.FromHours(1)
        });
      }
      else
      {
        transportFactory = new SchedulerLocalTransportFactory();
      }

      // Connect the scheduler client.
      var client = new SchedulerClient(store, transportFactory.CreateClientTransport());
      client.Open();

      TimeSpan? scheduledJobQueueInterval = null;
      if (configuration.ScheduledJobQueueInterval.HasValue)
        scheduledJobQueueInterval = TimeSpan.FromSeconds(configuration.ScheduledJobQueueInterval.Value);
      var scheduledJobUserId = configuration.ScheduledJobUserId;
      if (string.IsNullOrEmpty(scheduledJobUserId))
        scheduledJobUserId = null;

      self.SetJobSystemClient(client, scheduledJobUserId, scheduledJobQueueInterval);

      // Start the agent if we have an agent configuration section.
      if (agentConfiguration != null)
      {
        var agentConfig = AgentConfiguration.FromConfiguration(agentConfiguration);
        agentConfig.Store = store;
        agentConfig.Transport = transportFactory.CreateAgentTransport();

        self.ConfigureJobSystemAgent(agentConfig, serverTypes);
      }

      // Register the transport factory to ensure it's cleaned up on shutdown.
      self.ContainerBuilder
        .RegisterInstance(transportFactory)
        .As<ISchedulerTransportFactory>();

      string GetNonEmptyString(params string[] values)
      {
        return values.FirstOrDefault(p => !string.IsNullOrEmpty(p));
      }
    }

    public static void ConfigureAmcsHttpClient(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<AmcsHttpClient>()
        .As<IHttpClient>();
    }

    public static void ConfigureJobSystemAgent(this DataConfiguration self, AgentConfiguration agentConfiguration, TypeManager serviceTypes)
    {
      foreach (var assembly in serviceTypes.Assemblies)
      {
        if (agentConfiguration.HandlerAssemblies.All(p => p.FullName != assembly.FullName))
          agentConfiguration.HandlerAssemblies.Add(assembly);
      }

      var agent = new AgentService(agentConfiguration);

      self.ContainerBuilder
        .RegisterInstance(agent)
        .AsSelf();

      self.ContainerBuilder
        .Register(context => new AgentServiceManager(agent,context.Resolve<ISetupService>()))
        .AsSelf()
        .AutoActivate();

      self.ContainerBuilder
        .Register(p => new AgentServiceDiagnostics(p.Resolve<AgentService>(), p.ResolveOptional<IDiagnosticsManager>()))
        .SingleInstance()
        .AutoActivate();
    }

    public static void SetJobSystemClient(this DataConfiguration self, SchedulerClient client,
      string scheduledJobUserId = null, TimeSpan? scheduledJobQueueInterval = null)
    {
      self.ContainerBuilder
        .RegisterInstance(client)
        .AsSelf();

      // If we have an interval configured, setup the timer for scheduled
      // jobs. Otherwise, we hook into the diagnostics framework to use
      // the availability tests as a trigger to queue jobs.

      if (scheduledJobQueueInterval.HasValue)
      {
        client.SetupScheduledJobsTimer(scheduledJobUserId, scheduledJobQueueInterval.Value);
        scheduledJobUserId = null;
      }

      self.ContainerBuilder
        .Register(p => new SchedulerClientDiagnostics(p.Resolve<SchedulerClient>(),
          p.ResolveOptional<IDiagnosticsManager>(), scheduledJobUserId))
        .SingleInstance()
        .AutoActivate();
    }

    public static void ConfigureJobSystemStatusMonitor(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<JobSystemMapper>()
        .SingleInstance()
        .AutoActivate()
        .AsSelf();
    }

    public static void ConfigureEncryptionService(this DataConfiguration self, string encryptionKey)
    {
      self.SetEncryptionService(BouncyEncryptionServiceFactory.Create(encryptionKey));
    }

    public static void SetEncryptionService(this DataConfiguration self, IEncryptionService service)
    {
      self.ContainerBuilder
        .RegisterInstance(service)
        .As<IEncryptionService>();
    }

    public static void ConfigureSecurityTokenManager(this DataConfiguration self, string encryptionKey)
    {
      self.SetSecurityTokenManager(new SecurityTokenManager(new SecurityTokenConfiguration(
        null,
        null,
        encryptionKey
      )));
    }

    public static void SetSecurityTokenManager(this DataConfiguration self, ISecurityTokenManager service)
    {
      self.ContainerBuilder
        .RegisterInstance(service)
        .As<ISecurityTokenManager>();
    }

    public static void ConfigureSessionTokenManager(this DataConfiguration self, TimeSpan expiration)
    {
      self.ContainerBuilder
        .Register(p => new SessionTokenManager(p.Resolve<ISecurityTokenManager>(), expiration))
        .SingleInstance()
        .As<ISessionTokenManager>();
    }

    public static void ConfigurePlatformCredentialsSecurityTokenManager(this DataConfiguration self,
      IServerConfiguration configuration)
    {
      self.SetPlatformCredentialsSecurityTokenManager(new PlatformCredentialsSecurityTokenManager(
        new PlatformCredentialsSecurityTokenConfiguration(
          null,
          null,
          configuration.CertificateSubjectName,
          configuration.CertificatePath,
          configuration.EncryptionKey
        )));
    }

    public static void SetPlatformCredentialsSecurityTokenManager(this DataConfiguration self,
      IPlatformCredentialsSecurityTokenManager service)
    {
      self.ContainerBuilder
        .RegisterInstance(service)
        .As<IPlatformCredentialsSecurityTokenManager>();
    }

    public static void ConfigurePlatformCredentialsTokenManager(this DataConfiguration self, TimeSpan expiration)
    {
      self.ContainerBuilder
        .Register(p => new PlatformCredentialsTokenManager(p.Resolve<IPlatformCredentialsSecurityTokenManager>(), expiration))
        .SingleInstance()
        .As<IPlatformCredentialsTokenManager>();
    }

    public static void SetBlobStorageService(this DataConfiguration self, IBlobStorageService service)
    {
      self.ContainerBuilder
        .RegisterInstance(service)
        .As<IBlobStorageService>();
    }

    public static void ConfigureDatabaseBlobStorage(this DataConfiguration self)
    {
      self.SetBlobStorageService(new DatabaseBlobStorageService());
    }

    public static void ConfigureAzureBlobStorage(this DataConfiguration self, IConnectionString connectionString,
      string container, bool enableExternalStorage)
    {
      self.SetBlobStorageService(new AzureBlobStorageService(connectionString, container, enableExternalStorage));
    }

    public static void ConfigureFileSystemBlobStorage(this DataConfiguration self, string store, bool enableExternalStorage)
    {
      self.SetBlobStorageService(new FileSystemBlobStorageService(store, enableExternalStorage));
    }

    public static void ConfigureSystemConfiguration(this DataConfiguration self, Type configurationType)
    {
      self.SetSystemConfigurationService(new SystemConfigurationService(configurationType));
    }

    public static void SetSystemConfigurationService(this DataConfiguration self, ISystemConfigurationService service)
    {
      self.ContainerBuilder
        .RegisterInstance(service)
        .As<ISystemConfigurationService>()
        .AutoActivate()
        .SingleInstance()
        .AsSelf();
    }

    public static void ConfigureTempFileService(this DataConfiguration self,
      ITempFileStorageConfiguration configuration, IConnectionString blobStorageConnectionString)
    {
      self.ContainerBuilder
        .RegisterInstance(new TempFileService(configuration, blobStorageConnectionString))
        .As<ITempFileService>();
    }

    public static void SetDiagnosticsRenderer(this DataConfiguration self, IDiagnosticsRenderer renderer)
    {
      self.ContainerBuilder
        .RegisterInstance(renderer)
        .As<IDiagnosticsRenderer>();
    }

    public static void SetDiagnosticsRenderer<T>(this DataConfiguration self)
      where T : IDiagnosticsRenderer
    {
      self.ContainerBuilder
        .RegisterType<T>()
        .SingleInstance()
        .As<IDiagnosticsRenderer>();
    }

    public static void ConfigureApplicationInsights(this DataConfiguration self, string applicationInsightsInstrumentationKey)
    {
      if (!string.IsNullOrEmpty(applicationInsightsInstrumentationKey))
      {
        self.ContainerBuilder
            .Register(p => new ApplicationInsightsConfiguration(p.Resolve<IAppSetupService>(), applicationInsightsInstrumentationKey))
            .SingleInstance()
            .AutoActivate()
            .AsSelf();
      }

      self.ContainerBuilder
        .Register(p => new ApplicationInsightsDiagnostics(applicationInsightsInstrumentationKey, p.ResolveOptional<IDiagnosticsManager>()))
        .SingleInstance()
        .AutoActivate();
    }

#if !NETFRAMEWORK
    public static void ConfigureStaticFiles(this DataConfiguration self, string filesPath, string outputPath)
    {
      self.ContainerBuilder
        .Register(p => new StaticFilesService(p.Resolve<IAppSetupService>(), filesPath, outputPath))
        .SingleInstance()
        .AutoActivate()
        .AsSelf();
    }

    public static void ConfigureServiceProviderProxy<T>(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<ServiceProviderProxy<T>>()
        .SingleInstance()
        .AutoActivate()
        .AsSelf();

      self.ContainerBuilder
        .Register(p => p.Resolve<ServiceProviderProxy<T>>().Resolve())
        .ExternallyOwned();
    }
#endif

    public static void ConfigureAzureServiceBusConnectionManager(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterInstance(new AzureServiceBusConnectionManager())
        .As<IAzureServiceBusConnectionManager>();
    }

    public static void SetDataAccessIdService(this DataConfiguration self, IDataAccessIdService idService)
    {
      self.ContainerBuilder
        .RegisterInstance(idService)
        .As<IDataAccessIdService>();
    }

    public static void ConfigureDataSetManager(this DataConfiguration self, IDataSetsConfiguration configuration,
      IConnectionString blobStorageConnectionString, TypeManager dataSetTypes)
    {
      ConfigureDataSetManager(self, configuration, new DatabaseJobReader(), blobStorageConnectionString, dataSetTypes);
    }

    public static void ConfigureDataSetManager(this DataConfiguration self, IDataSetsConfiguration configuration,
      IJobReader jobReader, IConnectionString blobStorageConnectionString, TypeManager dataSetTypes)
    {
      self.ContainerBuilder
        .Register(p => new DataSetService(dataSetTypes, configuration, jobReader, blobStorageConnectionString,
          p.ResolveOptional<SchedulerClient>()))
        .SingleInstance()
        .AutoActivate()
        .AsSelf()
        .As<IDataSetService>();
    }

    public static void ConfigureTestData(this DataConfiguration self, TypeManager typeManager)
    {
      self.ContainerBuilder
        .RegisterTypes(typeManager.GetTypes().ToArray())
        .AsClosedTypesOf(typeof(ITestDataGenerator<,>));

      self.ContainerBuilder
        .RegisterInstance(new TestDataService(typeManager))
        .SingleInstance()
        .AutoActivate()
        .AsSelf()
        .As<ITestDataService>();
    }

    private class SchedulerClientDiagnostics
    {
      private readonly SchedulerClient client;
      private readonly string scheduledJobUserId;

      public SchedulerClientDiagnostics(SchedulerClient client, IDiagnosticsManager diagnosticsManager, string scheduledJobUserId)
      {
        this.client = client;
        this.scheduledJobUserId = scheduledJobUserId;

        diagnosticsManager?.Register(GetDiagnostics);
      }

      private IEnumerable<DiagnosticResult> GetDiagnostics()
      {
        // If no timer is configured, we have a scheduled job user ID. When
        // that's the case, we queue jobs here to have them raised by availability
        // monitoring.

        if (!string.IsNullOrEmpty(scheduledJobUserId))
          client.QueueScheduledJobs(scheduledJobUserId);

        var diagnostics = client.GetDiagnostics();

        string message = $"Jobs posted: {diagnostics.JobsPosted}, cancellations posted: {diagnostics.CancellationsPosted}, updates received: {diagnostics.UpdatesReceived}, send queue backlog: {diagnostics.SendQueueBacklog}";
        if (diagnostics.UnhandledException != null)
          message += Environment.NewLine + $"Unhandled exception: {diagnostics.UnhandledException.Message} ({diagnostics.UnhandledException.GetType()})";

        if (diagnostics.UnhandledException == null)
          yield return new DiagnosticResult.Success("Job System Scheduler Client", message);
        else
          yield return new DiagnosticResult.Failure("Job System Scheduler Client", message, diagnostics.UnhandledException);

        if (diagnostics.NonTransientRetryException != null)
          yield return new DiagnosticResult.Failure("Job System Scheduler Client", $"A non transient retry exception occurred: {diagnostics.NonTransientRetryException.Message} ({diagnostics.NonTransientRetryException.GetType()})", diagnostics.NonTransientRetryException);
      }
    }

    private class AgentServiceDiagnostics
    {
      private readonly AgentService agent;

      public AgentServiceDiagnostics(AgentService agent, IDiagnosticsManager diagnosticsManager)
      {
        this.agent = agent;

        diagnosticsManager?.Register(GetDiagnostics);
      }

      private IEnumerable<DiagnosticResult> GetDiagnostics()
      {
        var diagnostics = agent.GetDiagnostics();

        var message = $"Running: {(diagnostics.IsRunning ? "yes" : "no")}, status: {diagnostics.GetStatus()}, jobs received: {diagnostics.JobsReceived}, cancellations received: {diagnostics.CancellationsReceived}, updates posted: {diagnostics.UpdatesPosted}, send queue backlog: {diagnostics.SendQueueBacklog}";
        var queueStatus = diagnostics.GetQueueStatus();

        if (queueStatus.Any())
          message += ", Active queues: ";

        message = queueStatus.Aggregate(message, (current, status) => current + $"{status.QueueName}: {status.ActiveQueueSize} of {status.QueueSize}; ");

        if (diagnostics.UnhandledException != null)
          message += Environment.NewLine + $"Unhandled exception: {diagnostics.UnhandledException.Message} ({diagnostics.UnhandledException.GetType()})";

        if (diagnostics.IsRunning && diagnostics.GetStatus() == AgentServiceStatus.Running && diagnostics.UnhandledException == null)
          yield return new DiagnosticResult.Success("Job System Agent", message);
        else
          yield return new DiagnosticResult.Failure("Job System Agent", message, diagnostics.UnhandledException);

        //if (diagnostics.IsRunning && diagnostics.GetStatus() == AgentServiceStatus.Running && diagnostics.NonTransientRetryException != null)
        //  yield return new DiagnosticResult.Failure("Job System Agent", $"A non transient retry exception occurred: {diagnostics.NonTransientRetryException.Message} ({diagnostics.NonTransientRetryException.GetType()})", diagnostics.NonTransientRetryException);
      }
    }

    private class ApplicationInsightsDiagnostics
    {
      private readonly string applicationInsightsInstrumentationKey;

      public ApplicationInsightsDiagnostics(string applicationInsightsInstrumentationKey, IDiagnosticsManager diagnosticsManager)
      {
        this.applicationInsightsInstrumentationKey = applicationInsightsInstrumentationKey;

        diagnosticsManager?.Register(GetDiagnostics);
      }

      private IEnumerable<DiagnosticResult> GetDiagnostics()
      {
        if (!string.IsNullOrWhiteSpace(applicationInsightsInstrumentationKey))
          yield return new DiagnosticResult.Success("Application Insights");
        else
          yield return new DiagnosticResult.Failure("Application Insights", "No Application Insights instrumentation key is set.");
      }
    }

    private class SynchronizationEventRegistrationService
    {
      public SynchronizationEventRegistrationService(IDataEventsBuilderService dataEventsBuilderService, IUserService userService)
      {
        dataEventsBuilderService.Add(new SynchronizationEvents(new SynchronisationRequestManager(userService)));
      }
    }

    private class EntityHistoryEventRegistrationService
    {
      public EntityHistoryEventRegistrationService(IDataEventsBuilderService dataEventsBuilderService)
      {
        dataEventsBuilderService.Add(new EntityHistoryDataEvents());
      }
    }
  }
}
