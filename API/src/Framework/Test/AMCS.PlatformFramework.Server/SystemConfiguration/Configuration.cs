using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using AMCS.Data.Server.SystemConfiguration;

namespace AMCS.PlatformFramework.Server.SystemConfiguration
{
  [XmlRoot(Namespace = Ns)]
  public class Configuration : IConfiguration
  {
    public const string Ns = SystemConfigurationService.SystemConfigurationNamespace;
    
    [XmlElement(nameof(ProfileName))]
    public string ProfileName { get; set; }

    public SystemConfigurationCollection SystemConfigurations { get; set; } = new SystemConfigurationCollection();
    public UserGroupCollection UserGroups { get; set; } = new UserGroupCollection();

    public IEnumerable<ConfigurationCollection> GetCollections()
    {
      yield return SystemConfigurations;
      yield return UserGroups;
    }

    public void Merge(IConfiguration configuration)
    {
      throw new NotSupportedException();
    }
  }
}
