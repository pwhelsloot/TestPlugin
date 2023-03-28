namespace AMCS.Data.Server.WebHook
{
  using AMCS.Data.Server.WebHook.BslTrigger;
  using AMCS.Data.Configuration;
  using AMCS.Data.Server.Plugin;
  using Autofac;
  using AMCS.PluginData.Data.WebHook;
  using AMCS.Data.Server.Configuration;
  using Microsoft.ApplicationInsights;
  using Microsoft.ApplicationInsights.Extensibility;
  using Plugin.MetadataRegistry;
  using AMCS.Data.Server.Webhook.Engine;
  using Plugin.MetadataRegistry.WebHook;
  using Services;
  using Webhook.Engine.Actions;
  using Webhook.Engine.HttpCallbacks;
  using Webhook.Engine.Validations;
  using AMCS.Data.Server.Webhook;

  public static class DataConfigurationExtensions
  {
    public static void ConfigureWebHooks(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<WebHookService>()
        .SingleInstance()
        .AsSelf()
        .AutoActivate()
        .As<IWebHookService>();
      
      self.ContainerBuilder
        .RegisterType<WebHookCacheService>()
        .SingleInstance()
        .AsSelf()
        .AutoActivate()
        .As<IWebHookCacheService>();

      self.ContainerBuilder
        .RegisterType<WebHookBslTriggerService>()
        .SingleInstance()
        .AsSelf()
        .As<IWebHookBslTriggerService>();

      self.ContainerBuilder
        .RegisterType<WebHookBslTriggerExecuteService>()
        .SingleInstance()
        .As<IWebHookBslTriggerExecuteService>();

      self.ContainerBuilder
        .RegisterType<WebHookManager>()
        .SingleInstance()
        .As<IWebHookInternalManager>()
        .As<IWebHookManager>();

      self.ContainerBuilder
        .RegisterType<PostCommitWebHookExecutor>()
        .SingleInstance()
        .Keyed<IWebHookExecutor>(WebHookConstants.WebHookPostCommitExecutorKey);

      self.ContainerBuilder
        .RegisterType<PreCommitWebHookExecutor>()
        .SingleInstance()
        .Keyed<IWebHookExecutor>(WebHookConstants.WebHookPreCommitExecutorKey);

      self.ContainerBuilder
        .RegisterType<PreCommitDeleteValidation>()
        .Keyed<IWebHookValidation>(WebHookTrigger.PreDelete.ToString());

      self.ContainerBuilder
        .RegisterType<PreCommitSaveValidation>()
        .Keyed<IWebHookValidation>(WebHookTrigger.PreUpdate.ToString());

      self.ContainerBuilder
        .RegisterType<PreCommitSaveValidation>()
        .Keyed<IWebHookValidation>(WebHookTrigger.PreSave.ToString());

      self.ContainerBuilder
        .RegisterType<PreCommitSaveValidation>()
        .Keyed<IWebHookValidation>(WebHookTrigger.PreInsert.ToString());

      self.ContainerBuilder
        .RegisterType<WebHookUdfDataAction>()
        .As<IWebHookDataAction>();

      self.ContainerBuilder
        .Register(context =>
        {
          var aiKey = context.Resolve<IServerConfiguration>().ApplicationInsightsInstrumentationKey;
          var config = new TelemetryConfiguration(aiKey);
          return new TelemetryClient(config);
        })
        .Keyed<TelemetryClient>(WebHookConstants.WebHookPostCommitExecutorKey)
        .SingleInstance();

      self.ContainerBuilder
        .Register(context =>
        {
          var aiKey = context.Resolve<IServerConfiguration>().ApplicationInsightsInstrumentationKey;
          var config = new TelemetryConfiguration(aiKey);
          return new TelemetryClient(config);
        })
        .Keyed<TelemetryClient>(WebHookConstants.WebHookPreCommitExecutorKey)
        .SingleInstance();

      self.ContainerBuilder
        .RegisterType<HttpCallbackService>()
        .SingleInstance()
        .As<IHttpCallbackService>();

      self.ContainerBuilder
        .RegisterType<HttpPostCommitCallback>()
        .Keyed<IHttpCallback>(WebHookConstants.WebHookPostCommitExecutorKey);

      self.ContainerBuilder
        .RegisterType<HttpPreCommitCallback>()
        .Keyed<IHttpCallback>(WebHookConstants.WebHookPreCommitExecutorKey);

      self.ContainerBuilder
        .RegisterType<WebHookSimpleService>()
        .Keyed<IWebHookTriggerService>(WebHookFormat.Simple)
        .SingleInstance();

      self.ContainerBuilder
        .RegisterType<WebHookFullService>()
        .Keyed<IWebHookTriggerService>(WebHookFormat.Full)
        .SingleInstance();

      self.ContainerBuilder
        .RegisterType<WebHookCoalesceService>()
        .Keyed<IWebHookTriggerService>(WebHookFormat.Coalesce)
        .SingleInstance();

      self.AddWebHookMetadataRegistry<WebHookMetadataRegistryService>();
      self.AddWebHookMetadataProcessor<WebHookMetadataProcessor>();
    }
  }
}
