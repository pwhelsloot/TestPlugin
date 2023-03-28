namespace AMCS.ApiService.Tests.Configuration.DynamicAppConfig
{
  using System;
  using System.Collections.Generic;
  using Data;
  using Data.Server.Configuration.DynamicAppConfig;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class ConfigurationServiceTests : BaseTest
  {
    [TestMethod]
    public void ConfigureConfigurationService_ValidSingleConfiguration_CanAccessConfiguration()
    {
      // Act
      var configService = DataServices.Resolve<IConfigurationService>();
      var validTestConfigurationResolver = DataServices.Resolve<ValidTestConfigurationResolver>();

      var expectedFeatureFlag1Value = ValidTestConfigurationKeys.FeatureFlag1Value;
      var expectedFeatureFlag2Value = ValidTestConfigurationKeys.FeatureFlag2Value;

      // Act
      var actualFeatureFlag1Value = validTestConfigurationResolver.FeatureFlag1;
      var actualFeatureFlag2Value = validTestConfigurationResolver.FeatureFlag2;

      // Assert
      Assert.IsNotNull(configService);
      Assert.AreEqual(expectedFeatureFlag1Value, actualFeatureFlag1Value);
      Assert.AreEqual(expectedFeatureFlag2Value, actualFeatureFlag2Value);
    }
  }
}
