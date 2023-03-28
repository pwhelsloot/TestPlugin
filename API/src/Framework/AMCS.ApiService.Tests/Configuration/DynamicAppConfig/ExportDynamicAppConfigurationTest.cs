namespace AMCS.ApiService.Tests.Configuration.DynamicAppConfig
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.Json;
  using Data;
  using Data.Server.Configuration.DynamicAppConfig.Tools;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class ExportDynamicAppConfigurationTest : BaseTest
  {
    [TestMethod]
    public void ExportDynamicAppConfiguration_SuccessfulExport()
    {
      var dynamicAppConfigurations = DataServices.Resolve<AzureAppConfigurationExporter>().ExportDynamicAppConfigurations();

      var json = JsonSerializer.Serialize(dynamicAppConfigurations);

      const string expected = "{\"FeatureFlags\":[{\"Name\":\"%TENANT_ID%|%PLUGIN_ID%|MyTenantFeatureFlag\"},{\"Name\":\"%TENANT_ID%|%PLUGIN_ID%|MyTenantFeatureFlag2\"},{\"Name\":\"%PLUGIN_ID%|MyGlobalFeatureFlag\"},{\"Name\":\"%ENVIRONMENT%|MyGlobalFeatureFlag\"}],\"ConfigurationKeys\":[{\"Name\":\"%TENANT_ID%|%PLUGIN_ID%|MyTenantConfiguration\"},{\"Name\":\"%PLUGIN_ID%|MyGlobalConfiguration\"},{\"Name\":\"%ENVIRONMENT%|MyGlobalConfiguration\"}]}";

      Assert.AreEqual(expected, json);
    }
  }
}