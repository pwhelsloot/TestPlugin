using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SystemConfiguration
{
  [AttributeUsage(AttributeTargets.Class)]
  public class ConfigurationElementAttribute : Attribute
  {
    public Type Synchronizer { get; }

    public ConfigurationElementAttribute(Type synchronizer)
    {
      Synchronizer = synchronizer;
    }
  }
}
