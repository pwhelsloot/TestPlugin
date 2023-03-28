namespace AMCS.ApiService.Documentation.NETCore.Swagger
{
  using AMCS.ApiService.Documentation.Abstractions.Swagger;
  using Microsoft.AspNetCore.Mvc.ApiExplorer;

  internal interface IApiActionDescriptionProvider : IApiDescriptionProvider
  {
    void RegisterApiAction(ApiExplorerAction action);
  }
}