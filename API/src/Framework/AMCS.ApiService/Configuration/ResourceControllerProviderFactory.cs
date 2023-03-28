using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Elemos;

namespace AMCS.ApiService.Configuration
{
  public class ResourceControllerProviderFactory : IControllerProviderFactory
  {
    private readonly Assembly assembly;
    private readonly string resourceName;

    public ResourceControllerProviderFactory(Assembly assembly, string resourceName)
    {
      this.assembly = assembly;
      this.resourceName = resourceName;
    }

    public IControllerProvider Create()
    {
      using (var stream = assembly.GetManifestResourceStream(resourceName))
      {
        return XmlControllerProvider.Load(stream);
      }
    }
  }
}
