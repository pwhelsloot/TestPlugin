namespace AMCS.PlatformFramework.IntegrationTests.Plugin.MetadataRegistry
{
  using System.Linq;
  using System.Xml.Linq;
  using System.Threading.Tasks;
  using Data;
  using AMCS.PluginData.Data.Metadata;
  using AMCS.PluginData.Data.Metadata.MetadataRegistries;
  using PluginData.Services;
  using NUnit.Framework;
  using AMCS.Data.Server.Plugin.MetadataRegistry.UiComponent;

  [TestFixture]
  public class UiComponentsRegistryFixture : TestBase
  {
    [Test]
    public async Task WhenCreatingUiComponentsMetadataRegistry_ThenEntityIsReturned()
    {
      const string expected =
        "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
         "<UiComponentRegistry xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"https://schemas.amcsgroup.io/platform/metadata/ui-components/2021-12\">" +
            "<UiComponent xsi:type=\"ShowScreenUiComponent\" Name=\"amcs/platform:LoginScreen\">" +
              "<Description>" +
                "<Value Language=\"de-DE\">Dein Benutzername oder deine Email Adresse</Value>" +
                "<Value Language=\"en\">Your username or email address</Value>" +
            "</Description>" +
            "<Inputs>" +
                "<Input Name=\"Username\" Type=\"string\" Required=\"true\">" +
                    "<Description>" +
                        "<Value Language=\"de-DE\">Ihr Passwort</Value>" +
                        "<Value Language=\"en\">Your password</Value>" +
                    "</Description>" +
                "</Input>" +
            "</Inputs>" +
            "<Outputs>" +
                "<Output Name=\"IsAuthenticated\" Type=\"bool\" Required=\"false\">" +
                    "<Documentation>" +
                        "<Value Language=\"en\">Is will be true if the user authenticated successfully</Value>" +
                        "<Value Language=\"nl\">Dit is waar als de gebruiker met succes is geverifieerd</Value>" +
                    "</Documentation>" +
                "</Output>" +
            "</Outputs>" +
            "</UiComponent>" +
        "</UiComponentRegistry>";

      var uiComponentsRegistryService = DataServices.Resolve<IUiComponentsMetadataRegistryService>();
      var uiComponentRegistry = await uiComponentsRegistryService.CreateUiComponentRegistry();

      var actual = DataServices.Resolve<IPluginSerializationService>().Serialize(uiComponentRegistry);
      
      // Ignore white-space
      var actualDoc = XDocument.Parse(actual);
      var expectedDoc = XDocument.Parse(expected);
      
      Assert.AreEqual(expectedDoc.ToString(), actualDoc.ToString());
    }

    [Test]
    public void GivenPluginMetadata_WhenDoingMex_ThenUiComponentsAdded()
    {
      var uiComponentsRegistryService = (TestUiComponentsMetadataRegistryService)DataServices.Resolve<IUiComponentsMetadataRegistryService>();
      var pluginMetadata = new PluginMetadata();
      uiComponentsRegistryService.AddMetadataRegistry(pluginMetadata);

      Assert.AreEqual(1, pluginMetadata.MetadataRegistries.Count);
      var actual = pluginMetadata.MetadataRegistries.First();
      Assert.AreEqual(MetadataRegistryType.UiComponents, actual.Type);
      Assert.AreEqual(TestWorkflowActivityMetadataService.Url, actual.Url);
    }
  }
}
