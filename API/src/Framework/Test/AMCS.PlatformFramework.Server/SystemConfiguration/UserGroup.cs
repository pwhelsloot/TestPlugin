using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AMCS.Data.Server.SystemConfiguration;

namespace AMCS.PlatformFramework.Server.SystemConfiguration
{
  [XmlType(Namespace = Configuration.Ns)]
  [ConfigurationElement(typeof(UserGroupSynchronizer))]
  public class UserGroup : ConfigurationElement
  {
    [XmlAttribute]
    public string Name { get; set; }

    [XmlAttribute]
    public bool IsAdministrator { get; set; }
  }
}
