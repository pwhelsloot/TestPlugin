using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Documentation.Abstractions.Swagger
{
  internal interface IApiExplorerConfiguration
  {
    void RegisterApiAction(ApiExplorerAction action);

    void RegisterMessageService(Type controllerType, Type responseType, string route, string controllerName);

    string GetErrorCodeUrl(int errorCode);

    string RenderMarkdownDocumentation(string path);
  }
}