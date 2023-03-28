using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Abstractions.CommsServer
{
  [AttributeUsage(AttributeTargets.Class)]
  public class CommsServerProtocolAttribute : Attribute
  {
    public string Protocol { get; }

    public CommsServerProtocolAttribute(string protocol)
    {
      Protocol = protocol;
    }
  }
}
