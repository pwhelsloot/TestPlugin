namespace AMCS.PlatformFramework.UnitTests.StepDefinitions.Plugin;

using System.Xml.Linq;
using Data.Server.Plugin;
using NUnit.Framework;
using PluginData.Data.Configuration;
using TechTalk.SpecFlow;

[Binding]
public class MexProcessingSteps
{
  private readonly Version version811 = new Version("8.11.0.0");

  private PluginConfiguration pluginConfiguration;
  private string resultXml;
  private string originalXml;
  private Version pluginVersion;

  [Given(@"the core version is (.*)")]
  public void GivenTheCoreVersionIs(string version)
  {
    pluginConfiguration = new PluginConfiguration
    {
      PluginDependencies = new List<PluginDependency>
      {
        new PluginDependency
        {
          Name = PluginHelper.GetCorePluginFullName(),
          Version = version
        }
      }
    };
  }

  [Given(@"the plugin version is (.*)")]
  public void GivenThePluginVersionIs(string input)
  {
    pluginVersion = new Version(input);
    
    originalXml = pluginVersion.CompareTo(version811) < 0
      ? File.ReadAllText("StepDefinitions\\Plugin\\OldPluginMetadata.xml")
      : File.ReadAllText("StepDefinitions\\Plugin\\NewPluginMetadata.xml");
  }

  [When(@"executing the get metadata mex post processing")]
  public void WhenExecutingTheGetMetadataMexPostProcessing()
  {
    var sut = new MexPostProcessingService();
    sut.Start();
    
    resultXml = sut.ExecuteGetMetadataPostProcessing(pluginConfiguration, originalXml);
  }

  [Then(@"then the reverse proxy rules have AcceptsOAuthToken added")]
  public void ThenThenTheReverseProxyRulesHaveAcceptsOAuthTokenAdded()
  {
    var coreVersion = new Version(pluginConfiguration.PluginDependencies[0].Version);

    var expected = coreVersion.Minor == 11 && pluginVersion.Minor == 11
      ? XDocument.Parse(File.ReadAllText("StepDefinitions\\Plugin\\NewPluginMetadata.xml"))
      : XDocument.Parse(File.ReadAllText("StepDefinitions\\Plugin\\OldPluginMetadata.xml"));

    var actual = XDocument.Parse(resultXml);

    Assert.AreEqual(expected.ToString(), actual.ToString());
  }
}