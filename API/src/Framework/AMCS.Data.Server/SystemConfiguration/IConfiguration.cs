using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AMCS.Data.Server.SystemConfiguration
{
  public interface IConfiguration
  {
    IEnumerable<ConfigurationCollection> GetCollections();

    string ProfileName { get; set; }

    void Merge(IConfiguration configuration);
  }
}
