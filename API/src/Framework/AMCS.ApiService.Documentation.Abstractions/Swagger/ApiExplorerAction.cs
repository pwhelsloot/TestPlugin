using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger
{
  internal class ApiExplorerAction
  {
    public Type ControllerType { get; set; }

    public Type ResponseType { get; set; }

    public string ActionName { get; set; }

    public string MethodName { get; set; }

    public string HttpVerb { get; set; }

    public string Route { get; set; }

    public string ControllerName { get; set; }
  }
}
