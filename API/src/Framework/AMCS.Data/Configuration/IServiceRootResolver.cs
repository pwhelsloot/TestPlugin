using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Configuration
{
  public interface IServiceRootResolver
  {
    string ServiceRoot { get; }

    string GetServiceRoot(string name);

    string GetProjectServiceRoot();

#if NETFRAMEWORK
    EndpointAddress ResolveEndpointAddress(string name);
#endif
  }
}
