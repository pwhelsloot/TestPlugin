using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService
{
  public interface IControllerProvider
  {
    void RegisterRoutes(IRouteCollectionManager routes);

    Type GetControllerType(string controllerName);

    List<string> GetControllerNames();
  }
}
