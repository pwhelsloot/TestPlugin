namespace AMCS.Data.Server.Plugin
{
  using System.Collections.Generic;
  using AMCS.PluginData.Data.Metadata.ReverseProxyRules;

  public interface IReverseProxyRuleMetadataProcessor
  {
    void Process(IList<ReverseProxyRule> reverseProxyRules, string fullyQualifiedPluginName,
      ISessionToken sessionToken, IDataSession dataSession);
  }
}