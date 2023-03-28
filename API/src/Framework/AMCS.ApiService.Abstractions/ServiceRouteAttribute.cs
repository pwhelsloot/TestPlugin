using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Abstractions
{
  [AttributeUsage(AttributeTargets.Class)]
  public class ServiceRouteAttribute : Attribute
  {
    public string Route { get; }

    public ServiceRouteAttribute(string route)
    {
      Route = route;
    }
  }
}
