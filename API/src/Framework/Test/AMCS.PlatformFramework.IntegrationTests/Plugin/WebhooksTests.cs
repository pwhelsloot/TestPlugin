namespace AMCS.PlatformFramework.IntegrationTests.Plugin
{
  using System.Collections.Generic;
  using AMCS.Data;
  using AMCS.Data.Entity.Plugin;
  using AMCS.Data.Entity.WebHook;
  using AMCS.Data.Server;
  using AMCS.Data.Server.Plugin;
  using AMCS.PluginData.Data.Metadata;
  using AMCS.PluginData.Data.Metadata.WebHooks;
  using AMCS.PluginData.Data.WebHook;
  using NUnit.Framework;

  [TestFixture]
  public class WebhooksTests : TestBase
  {
    [TearDown]
    public void Teardown()
    {
      var pluginSystem = DataServices.Resolve<IPluginSystem>();

      WithSession(dataSession =>
      {
        var webhooks = dataSession.GetAllByTemplate(
          AdminUserId,
          new WebHookEntity { SystemCategory = pluginSystem.FullyQualifiedName },
          true);

        foreach (var webhook in webhooks)
        {
          dataSession.Delete(AdminUserId, webhook, false);
        }
      });
    }

    [Test]
    public void GivenPluginMetadataAndNoExistingData_WhenAddingWebhooks_ThenSavedInDb()
    {
      var metadataProcessor = DataServices.Resolve<IMetadataProcessor>();
      var pluginSystem = DataServices.Resolve<IPluginSystem>();

      var pluginMetadata = new PluginMetadata
      {
        Plugin = pluginSystem.FullyQualifiedName,
        Version = "1.0.0.0",
        WebHooks = new List<WebHook>
        {
          new WebHook
          {
            Url = "http://localhost:60047/services/api/webhook/callback",
            Format = WebHookFormat.Simple,
            Name = $"{pluginSystem.FullyQualifiedName}:Webhook1",
            Trigger = WebHookTrigger.Save | WebHookTrigger.Delete,
            BasicCredentials = new BasicCredentials { Username = "test", Password = "test" },
            HttpMethod = System.Net.Http.HttpMethod.Post.Method,
            Headers = new List<string>
            {
              "Content-Type: application/xml",
              "Custom Header: Custom Value"
            }
          },
          new WebHook
          {
            Url = "http://localhost:60047/services/api/webhook/callback",
            Format = WebHookFormat.Simple,
            Name = $"{pluginSystem.FullyQualifiedName}:Webhook2",
            Trigger = WebHookTrigger.Save | WebHookTrigger.Delete,
            HttpMethod = System.Net.Http.HttpMethod.Post.Method
          }
        }
      };

      WithSession(dataSession =>
      {
        metadataProcessor.ProcessMetadata(pluginMetadata, AdminUserId, dataSession);

        var reverseProxyRules = dataSession.GetAllByTemplate(
          AdminUserId,
          new WebHookEntity { SystemCategory = pluginSystem.FullyQualifiedName },
          true);

        Assert.IsNotNull(reverseProxyRules);
        Assert.AreEqual(2, reverseProxyRules.Count);
      });
    }

    [Test]
    public void GivenPluginMetadataWithExistingData_WhenAddingWebhooks_ThenSavedInDb()
    {
      var metadataProcessor = DataServices.Resolve<IMetadataProcessor>();
      var pluginSystem = DataServices.Resolve<IPluginSystem>();

      WithSession(dataSession =>
      {
        dataSession.BulkSave(AdminUserId, new List<WebHookEntity>
        {
          // 'old' record, it should be deleted
          new WebHookEntity
          {
            Url = "http://localhost:60047/services/api/webhook/callback",
            SystemCategory = pluginSystem.FullyQualifiedName,
            Format = 1,
            Name = $"{pluginSystem.FullyQualifiedName}:Webhook99-old",
            Trigger = (WebHookTrigger.Save | WebHookTrigger.Delete).ToString(),
            BasicCredentials = "un=test,pw=test",
            HttpMethod = System.Net.Http.HttpMethod.Post.Method,
            Headers = "Content-Type: application/xml \nCustom Header: Custom Value"
          },
          // current record, it should be kept
          new WebHookEntity
          {
            Url = "http://localhost:60047/services/api/webhook/callback",
            SystemCategory = pluginSystem.FullyQualifiedName,
            Format = 1,
            Name = $"{pluginSystem.FullyQualifiedName}:Webhook1",
            Trigger = (WebHookTrigger.Save | WebHookTrigger.Delete).ToString(),
            BasicCredentials = "un=test,pw=test",
            HttpMethod = System.Net.Http.HttpMethod.Post.Method,
            Headers = "Content-Type: application/xml \nCustom Header: Custom Value"
          }
        });
      });

      var pluginMetadata = new PluginMetadata
      {
        Plugin = pluginSystem.FullyQualifiedName,
        Version = "1.0.0.0",
        WebHooks = new List<WebHook>
        {
          new WebHook
          {
            Url = "http://localhost:60047/services/api/webhook/callback",
            Format = WebHookFormat.Simple,
            Name = $"{pluginSystem.FullyQualifiedName}:Webhook1",
            Trigger = WebHookTrigger.Save | WebHookTrigger.Delete,
            BasicCredentials = new BasicCredentials { Username = "test", Password = "test" },
            HttpMethod = System.Net.Http.HttpMethod.Post.Method,
            Headers = new List<string>
            {
              "Content-Type: application/xml",
              "Custom Header: Custom Value"
            }
          },
          new WebHook
          {
            Url = "http://localhost:60047/services/api/webhook/callback",
            Format = WebHookFormat.Simple,
            Name = $"{pluginSystem.FullyQualifiedName}:Webhook2",
            Trigger = WebHookTrigger.Save | WebHookTrigger.Delete,
            HttpMethod = System.Net.Http.HttpMethod.Post.Method
          }
        }
      };

      WithSession(dataSession =>
      {
        metadataProcessor.ProcessMetadata(pluginMetadata, AdminUserId, dataSession);

        var reverseProxyRules = dataSession
          .GetAllByTemplate(AdminUserId,
            new WebHookEntity { SystemCategory = pluginSystem.FullyQualifiedName }, true);

        // should still equal 2 as that is the amount in the plugin request
        Assert.IsNotNull(reverseProxyRules);
        Assert.AreEqual(2, reverseProxyRules.Count);
      });
    }

    [Test]
    public void GivenEmptyPluginMetadataWithExistingData_WhenAddingWebhooks_ThenDataDeleted()
    {
      var metadataProcessor = DataServices.Resolve<IMetadataProcessor>();
      var pluginSystem = DataServices.Resolve<IPluginSystem>();

      WithSession(dataSession =>
      {
        dataSession.BulkSave(AdminUserId, new List<WebHookEntity>
        {
          // 'old' record, it should be deleted
          new WebHookEntity
          {
            Url = "http://localhost:60047/services/api/webhook/callback",
            SystemCategory = pluginSystem.FullyQualifiedName,
            Format = 1,
            Name = "Webhook99-old",
            Trigger = (WebHookTrigger.Save | WebHookTrigger.Delete).ToString(),
            BasicCredentials = "un=test,pw=test",
            HttpMethod = System.Net.Http.HttpMethod.Post.Method,
            Headers = "Content-Type: application/xml \nCustom Header: Custom Value"
          },
          // current record, it should be kept
          new WebHookEntity
          {
            Url = "http://localhost:60047/services/api/webhook/callback",
            SystemCategory = pluginSystem.FullyQualifiedName,
            Format = 1,
            Name = "Webhook1",
            Trigger = (WebHookTrigger.Save | WebHookTrigger.Delete).ToString(),
            BasicCredentials = "un=test,pw=test",
            HttpMethod = System.Net.Http.HttpMethod.Post.Method,
            Headers = "Content-Type: application/xml \nCustom Header: Custom Value"
          }
        });
      });

      var pluginMetadata = new PluginMetadata
      {
        Plugin = pluginSystem.FullyQualifiedName,
        Version = "1.0.0.0",
        WebHooks = new List<WebHook>()
      };

      WithSession(dataSession =>
      {
        metadataProcessor.ProcessMetadata(pluginMetadata, AdminUserId, dataSession);

        var reverseProxyRules = dataSession.GetAllByTemplate(
          AdminUserId,
          new WebHookEntity { SystemCategory = pluginSystem.FullyQualifiedName },
          true);

        Assert.IsNotNull(reverseProxyRules);
        Assert.AreEqual(0, reverseProxyRules.Count);
      });
    }
  }
}