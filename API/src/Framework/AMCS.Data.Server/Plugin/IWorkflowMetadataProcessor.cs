namespace AMCS.Data.Server.Plugin
{
  using System.Collections.Generic;
  using PluginData.Data.Metadata.Workflows;

  public interface IWorkflowMetadataProcessor
  {
    void Process(IList<Workflow> workflows, string fullyQualifiedPluginName,
      ISessionToken sessionToken, IDataSession dataSession);
  }
}