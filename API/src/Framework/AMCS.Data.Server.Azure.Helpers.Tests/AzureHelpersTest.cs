namespace AMCS.Data.Server.Azure.Helpers.Tests
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using NUnit.Framework;

  public class AzureHelpersTest
  {
    private const int RandonNameLength = 30;
    private readonly string originalHostName;
    private readonly string originalSiteName;

    public AzureHelpersTest()
    {
      originalHostName = Environment.GetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME);
      originalSiteName = Environment.GetEnvironmentVariable(AzureHelpers.WEBSITE_SITE_NAME);
    }

    [TearDown]
    public void ResetVariables()
    {
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME, originalHostName);
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_SITE_NAME, originalSiteName);
    }

    [Test]
    public void IsRunningOnAppServiceTest()
    {
      try
      {
        Assert.IsFalse(AzureHelpers.IsRunningOnAppService());
        Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME, "a1-m76-svc-elemos.azurewebsites.net");
        Assert.IsTrue(AzureHelpers.IsRunningOnAppService());
      }
      finally
      {
        Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME, originalHostName);
      }
    }

    [Test]
    public void GetSiteNameTest()
    {
      try
      {
        Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME, "a1-m76-svc-elemos.azurewebsites.net");
        Assert.AreEqual("a1-m76", AzureHelpers.GetSiteName());

        Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME, "a1-m76-svc-elemos-staging.azurewebsites.net");
        Assert.AreEqual("a1-m76", AzureHelpers.GetSiteName());
      }
      finally
      {
        Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME, originalHostName);
      }
    }

    [Test]
    public void GetSlotNameTest()
    {
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME, "a1-m76-svc-elemos-staging.azurewebsites.net");
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_SITE_NAME, "a1-m76-svc-elemos");
      Assert.AreEqual("staging", AzureHelpers.GetSlotName());

      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME, "a1-m76-svc-elemos.azurewebsites.net");
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_SITE_NAME, "a1-m76-svc-elemos");
      Assert.IsNull(AzureHelpers.GetSlotName());

      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME, "a1-m76-svc-vehicle-staging.azurewebsites.net");
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_SITE_NAME, "a1-m76-svc-vehicle");
      Assert.AreEqual("staging", AzureHelpers.GetSlotName());

      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME, "a1-m76-svc-integration-staging.azurewebsites.net");
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_SITE_NAME, "a1-m76-svc-integration");
      Assert.AreEqual("staging", AzureHelpers.GetSlotName());
    }

    [Test]
    public void GetExpectedDatabaseNameTest()
    {
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME, "a1-m76-svc-elemos-staging.azurewebsites.net");
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_SITE_NAME, "a1-m76-svc-elemos");
      Assert.AreEqual("a1-m76-sqldb-elemos-staging", AzureHelpers.GetExpectedDatabaseName());

      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME, "a1-m76-svc-elemos.azurewebsites.net");
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_SITE_NAME, "a1-m76-svc-elemos");
      Assert.AreEqual("a1-m76-sqldb-elemos", AzureHelpers.GetExpectedDatabaseName());

      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME, "a1-m76-svc-vehicle-staging.azurewebsites.net");
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_SITE_NAME, "a1-m76-svc-vehicle");
      Assert.AreEqual("a1-m76-sqldb-elemos-staging", AzureHelpers.GetExpectedDatabaseName());

      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME, "a1-m76-svc-integration-staging.azurewebsites.net");
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_SITE_NAME, "a1-m76-svc-integration");
      Assert.AreEqual("a1-m76-sqldb-elemos-staging", AzureHelpers.GetExpectedDatabaseName());
    }

    [Test]
    public void GetFullSiteNameTest()
    {
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME, "a1-m76-svc-elemos-staging.azurewebsites.net");
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_SITE_NAME, "a1-m76-svc-elemos");
      Assert.AreEqual("a1-m76-staging", AzureHelpers.GetFullSiteName());

      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME, "a1-m76-svc-elemos.azurewebsites.net");
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_SITE_NAME, "a1-m76-svc-elemos");
      Assert.AreEqual("a1-m76", AzureHelpers.GetFullSiteName());

      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME, "a1-m76-svc-vehicle-staging.azurewebsites.net");
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_SITE_NAME, "a1-m76-svc-vehicle");
      Assert.AreEqual("a1-m76-staging", AzureHelpers.GetFullSiteName());

      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_HOSTNAME, "a1-m76-svc-integration-staging.azurewebsites.net");
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_SITE_NAME, "a1-m76-svc-integration");
      Assert.AreEqual("a1-m76-staging", AzureHelpers.GetFullSiteName());
    }

    [TestCase("a1-m76-svc-elemos")]
    [TestCase("b1-dev-svc-elemos")]
    public void GenerateRandomInstance_WhenWebsiteNamePresent_Will_Return_Correct_Result(string siteName)
    {
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_SITE_NAME, siteName);
      var generatedInstanceName = AzureHelpers.GenerateInstanceName();
      Assert.That(generatedInstanceName.StartsWith($"{siteName}-"));
      var expectedLength = siteName.Length + 1 + RandonNameLength;
      Assert.That(generatedInstanceName.Length, Is.EqualTo(expectedLength));
    }

    [TestCase(null)]
    [TestCase("")]
    public void GenerateRandomInstance_WhenNoWebsiteNamePresent_Will_Return_Correct_Result(string siteName)
    {
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_SITE_NAME, siteName);
      var generatedInstanceName = AzureHelpers.GenerateInstanceName();
      Assert.That(generatedInstanceName.Length, Is.EqualTo(RandonNameLength));
    }

    [TestCase("a1-m76-svc-elemos", 20)]
    [TestCase("b1-dev-svc-elemos", 60)]
    [TestCase(null, 50)]
    [TestCase("", 35)]
    public void GenerateRandomInstance_Will_Not_Return_Duplicates(string siteName, int numberToGenerate)
    {
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_SITE_NAME, siteName);
      var generatedStrings = new List<string>();
      for (int i = 0; i < numberToGenerate; i++)
      {
        generatedStrings.Add(AzureHelpers.GenerateInstanceName());
      }

      Assert.That(generatedStrings.Distinct().Count(), Is.EqualTo(generatedStrings.Count()));
    }

    [TestCase("a1-m76-svc-elemos")]
    [TestCase("b1-dev-svc-elemos")]
    [TestCase("eu1-workflowsrvr-dev-app")]
    [TestCase(null)]
    [TestCase("")]
    public void GenerateRandomInstance_Will_Not_Return_GreaterThan50Charatcters(string siteName)
    {
      Environment.SetEnvironmentVariable(AzureHelpers.WEBSITE_SITE_NAME, siteName);
      var generatedString = AzureHelpers.GenerateInstanceName();
      Assert.That(generatedString.Length, Is.LessThanOrEqualTo(50));
    }
  }
}
