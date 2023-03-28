using AMCS.Data.Server.Azure.Helpers;
using Microsoft.Azure.Amqp.Framing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace AMCS.PlatformFrameWork.UnitTests
{
  [Binding]
  public class AzureHelperSteps
  {
    private const int RandomNameLength = 30;
    private readonly string originalHostName;
    private readonly string originalSiteName;
    private string EnvironmentVariableName;
    private string EnvironmentUrl;
    private string OtherEnvironmentVariableName;
    private string OtherEnvironmentUrl;
    private List<string> GeneratedStrings = new List<string>();
    public AzureHelperSteps()
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

    [Given(@"no app service running")]
    public void GivenNoAppServiceRunning()
    {
      Assert.IsFalse(AzureHelpers.IsRunningOnAppService());
    }

    [Given(@"environment variable (.*) environment value (.*)")]
    public void GivenEnvironmentVariableEnvironmentValue(string environmentVariableName, string environmentUrl)
    {
      switch (environmentVariableName.ToUpperInvariant())
      {
        case AzureHelpers.WEBSITE_HOSTNAME:
          if (string.IsNullOrEmpty(EnvironmentVariableName))
            EnvironmentVariableName = AzureHelpers.WEBSITE_HOSTNAME;
          else
            OtherEnvironmentVariableName = AzureHelpers.WEBSITE_HOSTNAME;
          break;
        case AzureHelpers.WEBSITE_SITE_NAME:
          if (string.IsNullOrEmpty(EnvironmentVariableName))
            EnvironmentVariableName = AzureHelpers.WEBSITE_SITE_NAME;
          else
            OtherEnvironmentVariableName = AzureHelpers.WEBSITE_SITE_NAME;
          break;
      }

      if (string.IsNullOrEmpty(environmentUrl))
      {
        environmentUrl = "";
      }
      if (environmentUrl.Equals("<null>"))
      {
        environmentUrl = null;
      }

      if (string.IsNullOrEmpty(EnvironmentUrl))
        EnvironmentUrl = environmentUrl;
      else
        OtherEnvironmentUrl = environmentUrl;
    }

    [When(@"environment variable is set to environment value")]
    public void WhenAEnvironmentVariableValueIsSet()
    {
      Environment.SetEnvironmentVariable(EnvironmentVariableName, EnvironmentUrl);
      if (!string.IsNullOrEmpty(OtherEnvironmentVariableName))
        Environment.SetEnvironmentVariable(OtherEnvironmentVariableName, OtherEnvironmentUrl);
    }

    [When(@"random instances generated for (.*) times")]
    public void WhenRandomInstancesGeneratedForTimes(int randomInstanceCount)
    {
      for (int i = 0; i < randomInstanceCount; i++)
      {
        GeneratedStrings.Add(AzureHelpers.GenerateInstanceName());
      }
    }

    [Then(@"generated instances will all be distinct")]
    public void ThenGeneratedInstancesWillAllBeDistinct()
    {
      Assert.That(GeneratedStrings.Distinct().Count(), Is.EqualTo(GeneratedStrings.Count()));
    }


    [Then(@"app service starts running")]
    public void ThenAppServiceStartsRunning()
    {
      Assert.IsTrue(AzureHelpers.IsRunningOnAppService());
    }

    [Then(@"(.*) equals (.*) expected value")]
    public void ThenSiteNameEqualsA_MExpectedValue(string propertyName, string expectedResult)
    {
      if (string.IsNullOrEmpty(expectedResult))
        expectedResult = null;
      switch (propertyName.ToUpperInvariant())
      {
        case "SITE NAME":
          Assert.AreEqual(expectedResult, AzureHelpers.GetSiteName());
          break;
        case "SLOT NAME":
          Assert.AreEqual(expectedResult, AzureHelpers.GetSlotName());
          break;
        case "DATABASE NAME":
          Assert.AreEqual(expectedResult, AzureHelpers.GetExpectedDatabaseName());
          break;
      }
    }

    [Then(@"generated instance name length equals expected length")]
    public void ThenGeneratedInstanceNameLengthEqualsExpectedLength()
    {
      var generatedInstanceName = AzureHelpers.GenerateInstanceName();
      if (!string.IsNullOrWhiteSpace(EnvironmentUrl))
      {
        Assert.That(generatedInstanceName.StartsWith($"{EnvironmentUrl}-"));
        var expectedLength = EnvironmentUrl.Length + 1 + RandomNameLength;
        Assert.That(generatedInstanceName.Length, Is.EqualTo(expectedLength));
      }
      else
      {
        Assert.That(generatedInstanceName.Length, Is.EqualTo(RandomNameLength));
      }
    }

    [Then(@"generated instances will all be of length less than (.*)")]
    public void ThenGeneratedInstancesWillAllBeOfLengthLessThan(int siteNameLength)
    {
      Assert.That(GeneratedStrings[0].Length, Is.LessThanOrEqualTo(siteNameLength));
    }
  }
}