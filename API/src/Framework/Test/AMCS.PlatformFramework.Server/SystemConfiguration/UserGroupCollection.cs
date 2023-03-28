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
  public class UserGroupCollection : ConfigurationCollection<UserGroup>
  {
    [XmlElement(nameof(UserGroup))]
    public override List<UserGroup> Items { get; set; }
  }
}
