namespace AMCS.PlatformFramework.IntegrationTests.Plugin.MetadataRegistry
{
  using System.Linq;
  using System.Xml.Linq;
  using AMCS.Data;
  using AMCS.PluginData.Data.Metadata.MetadataRegistries;
  using AMCS.PluginData.Services;
  using Data.Server.Plugin;
  using Data.Server.Plugin.MetadataRegistry.WorkflowActivity;
  using NUnit.Framework;
  using PluginData.Data.Configuration;

  [TestFixture]
  public class WorkflowActivityRegistryFixture : TestBase
  {
    [Test]
    public void WhenCreatingWorkflowMetadataRegistry_ThenEntityIsReturned()
    {
      const string expected =
        "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
         "<WorkflowActivityRegistry xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"https://schemas.amcsgroup.io/platform/metadata/workflow-activities/2021-12\">" +
            "<WorkflowActivity xsi:type=\"RestWorkflowActivity\" Name=\"amcs/platformframework:CoreRESTAuthentication\">" +
              "<Description>" +
                  "<Value Language=\"fr-fr\">S'authentifie auprès de l'application principale.</Value>" +
                  "<Value Language=\"en-gb\">Authenticates against the core app.</Value>" +
              "</Description>" +
              "<Inputs>" +
                "<Input Name=\"Username\" Type=\"string\" Required=\"true\">" +
                   "<Description>" +
                      "<Value Language=\"es-mx\">Su nombre de usuario o dirección de correo electrónico.</Value>" +
                      "<Value Language=\"de-de\">Je gebruikersnaam of e-mailadres.</Value>" +
                      "<Value Language=\"fr-fr\">Votre nom d'utilisateur ou votre adresse e-mail.</Value>" +
                      "<Value Language=\"en-gb\">Your username or email address.</Value>" +
                    "</Description>" +
                "</Input>" +
                "<Input Name=\"Password\" Type=\"string\" Required=\"true\">" +
                   "<Description>" +
                      "<Value Language=\"fr-fr\">Votre mot de passe de domaine AMCS.</Value>" +
                      "<Value Language=\"en-gb\">Your AMCS domain password.</Value>" +
                   "</Description>" +
                "</Input>" +
              "</Inputs>" +
              "<Outputs>" +
                "<Output Name=\"IsAuthenticated\" Type=\"bool\" Required=\"false\">" +
                   "<Description>" +
                      "<Value Language=\"fr-fr\">Si l'authentification a réussi.</Value>" +
                      "<Value Language=\"en-gb\">If authentication was successful.</Value>" +
                   "</Description>" +
                "</Output>" +
              "</Outputs>" +
              "<Endpoint HttpMethod=\"POST\" Url=\"http://localhost:26519/authTokens\" RequestMimeType=\"application/json\">" +
                 "<RequestTemplate>{\"username\": \"admin\",\"password\": \"aB1?abc\"}</RequestTemplate>" +
              "</Endpoint>" +
           "</WorkflowActivity>" +
         "<WorkflowActivity xsi:type=\"RestWorkflowActivity\" Name=\"amcs/platformframework:RestName2\"/>" +
        "</WorkflowActivityRegistry>";

      var workflowActivityRegistryService = DataServices.Resolve<IWorkflowActivityMetadataRegistryService>();
      var workflowActivityRegistry = workflowActivityRegistryService.CreateWorkflowMetadataRegistry();

      // as other tests may register now, we only want to take registrations from our service to test
      var names = new[] {"amcs/platformframework:CoreRESTAuthentication", "amcs/platformframework:RestName2"};
      workflowActivityRegistry.WorkflowActivities = workflowActivityRegistry.WorkflowActivities
        .Where(registry => names.Contains(registry.Name))
        .ToList();
      
      var actual = DataServices.Resolve<IPluginSerializationService>().Serialize(workflowActivityRegistry);
      
      // Ignore white-space
      var actualDoc = XDocument.Parse(actual);
      var expectedDoc = XDocument.Parse(expected);
      
      Assert.AreEqual(expectedDoc.ToString(), actualDoc.ToString());
    }

    [Test]
    public void GivenPluginMetadata_WhenDoingMex_ThenWorkflowActivityAdded()
    {
      var workflowActivityRegistryService = DataServices.Resolve<IPluginMetadataService>();
      var pluginMetadata = workflowActivityRegistryService
        .GetMetadata("4C0FFDF1-CC0A-4C1F-A023-7B2C0354AE98", new PluginConfiguration());

      var actual = pluginMetadata.MetadataRegistries.First(registry => registry.Type == MetadataRegistryType.WorkflowActivities);

      Assert.AreEqual(4, pluginMetadata.MetadataRegistries.Count);
      Assert.AreEqual(MetadataRegistryType.WorkflowActivities, actual.Type);
      Assert.AreEqual(TestWorkflowActivityMetadataService.Url, actual.Url);
    }
  }
}
