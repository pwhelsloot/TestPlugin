namespace AMCS.TestPlugin.Server.Plugin.Workflows
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Text;
  using AMCS.Data.Server.Plugin;
  using AMCS.PluginData.Data.Configuration;
  using AMCS.PluginData.Data.Metadata;
  using AMCS.PluginData.Data.Metadata.Workflows;
  using AMCS.PluginData.Data.MetadataRegistry.Shared;

  public class PluginWorkflowMetadataService
  {
    private readonly string tenantId;

    public PluginWorkflowMetadataService(string tenantId, IPluginMetadataService pluginMetadataService)
    {
      this.tenantId = tenantId;

      pluginMetadataService.Register(AddWorkflows);
    }

    /// <summary>
    /// This registration allows us to inject the workflow metadata objects into the MEX process
    /// </summary>
    /// <param name="tenantId">The tenant id from the core app that started mex.</param>
    /// <param name="pluginMetadata">The plugin metadata object that will be returned to core.</param>
    /// <param name="pluginDependencies">The registered dependencies that have been installed in the core app.</param>
    private void AddWorkflows(
      string tenantId,
      PluginMetadata pluginMetadata,
      IList<PluginDependency> pluginDependencies)
    {
      // the tenant id and pluginDependencies can be used to alter the list of workflows to expose
      // eg a workflow is only exposed for tenant 1, or if erp is in the list of dependencies

      // this id should be generated via SQLGuidComb.Generate() the first time and then saved as below.
      pluginMetadata.Workflows ??= new List<Workflow>();

      pluginMetadata.Workflows.Add(new Workflow
      {
        Id = Guid.Parse("ef949aaf-5928-454d-ba28-c1836c2d6e2a"),
        WorkflowProvider = "amcs/workflow-server:workflow-server",
        Data = GetWorkflowBase64("GetTestMessage.xml"),
        Description = new Description
        {
          Value = new List<LocalisationValue>
          {
            new LocalisationValue()
            {
              Language = "en",
              Text = "Get Test Message"
            }
          }
        }
      });
    }

    /// <summary>
    /// Reads the workflow xml embedded into the AMCS.ExtensibilityGuides_WorkflowDefinitionMex.Server.Plugin.Workflows folder
    /// </summary>
    /// <param name="workflowName">The name of the file without the extension.</param>
    /// <returns>The base64 encoded string for workflow server.</returns>
    private string GetWorkflowBase64(string workflowName)
    {
      var path = $"{GetType().Namespace}.{workflowName}";

      using (var stream = GetType().Assembly.GetManifestResourceStream(path))
      {
        if (stream == null)
        {
          throw new InvalidOperationException(
            $"Could not find the workflow xml file located at: {path}");
        }

        using (var streamReader = new StreamReader(stream))
        {
          // read workflow and add tenant id
          // this is required as the workflow server app is multi tenant
          var workflow = streamReader.ReadToEnd();
          workflow = workflow.Replace("{{tenantId}}", tenantId);

          // the plugin system expects the workflow definition to be base64 encoded
          var bytes = Encoding.UTF8.GetBytes(workflow);
          return Convert.ToBase64String(bytes);
        }
      }
    }
  }
}