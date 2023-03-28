namespace AMCS.Data.Server.Plugin
{
  using System.Collections.Generic;
  using AMCS.PluginData.Data.Metadata.WorkCenterIcons;

  public interface IWorkCenterIconMetadataProcessor
  {
    void Process(IList<WorkCenterIcon> workCenterIcons, string fullyQualifiedPluginName,
      ISessionToken sessionToken, IDataSession dataSession);
  }
}