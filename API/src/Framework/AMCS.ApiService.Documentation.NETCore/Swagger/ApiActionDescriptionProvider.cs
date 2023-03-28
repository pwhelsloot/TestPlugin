namespace AMCS.ApiService.Documentation.NETCore.Swagger
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Text.RegularExpressions;
  using AMCS.ApiService.Documentation.Abstractions.Swagger;
  using AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions;
  using Data.Entity;
  using Microsoft.AspNetCore.Mvc.ApiExplorer;
  using Microsoft.AspNetCore.Mvc.Controllers;
  using Microsoft.AspNetCore.Mvc.ModelBinding;
  using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
  using Microsoft.AspNetCore.Routing.Template;

  internal class ApiActionDescriptionProvider : IApiActionDescriptionProvider
  {
    private static readonly Regex ControllerPostfixRe = new Regex(@"Controller(_\d+)?$", RegexOptions.Compiled);
    private readonly IList<ApiExplorerAction> apiExplorerActions = new List<ApiExplorerAction>();

    public int Order => -900;

    public void RegisterApiAction(ApiExplorerAction action)
    {
      apiExplorerActions.Add(action);
    }

    public void OnProvidersExecuting(ApiDescriptionProviderContext context)
    {
      var actionDescriptors = context.Actions.OfType<ControllerActionDescriptor>().ToList();
      
      foreach (var action in apiExplorerActions)
      {
        var actionDescriptor = actionDescriptors.SingleOrDefault(
          a => a.ControllerTypeInfo.AsType() == action.ControllerType &&
            a.ActionName == action.ActionName);

        if (actionDescriptor == null)
          continue;

        actionDescriptor.ControllerName = GetFriendlyControllerName(actionDescriptor.ControllerName);

        var apiDescription = GetApiDescription(action, actionDescriptor);
        if (apiDescription == null)
          continue;

        if (action.HttpVerb is "PUT" or "POST")
        {
          apiDescription.ParameterDescriptions.Add(new ApiParameterDescription
          {
            Source = BindingSource.Body,
            IsRequired = true,
            ModelMetadata = new ApiModelMetadata(ModelMetadataIdentity.ForType(action.ResponseType)),
            Type = action.ResponseType,
            ParameterDescriptor = new ControllerParameterDescriptor
            {
              ParameterType = action.ResponseType,
              ParameterInfo = new EntityParameterInfo(action.ResponseType),
            }
          });
        }

        context.Results.Add(apiDescription);
      }
    }

    public void OnProvidersExecuted(ApiDescriptionProviderContext context)
    {
      // do nothing
    }

    private ApiDescription GetApiDescription(ApiExplorerAction action, ControllerActionDescriptor actionDescriptor)
    {
      var methodName = action.MethodName ?? action.ActionName;

      var attribute = action.ResponseType.GetCustomAttribute<ApiExplorerAttribute>();
      var controllerTypeAttribute = action.ControllerType.GetCustomAttribute<ApiExplorerAttribute>();
      ApiExplorerAttribute controllerGenericArgumentAttribute = null;
      var controllerTypeGenericArguments = action.ControllerType.GetGenericArguments();
      if (controllerTypeGenericArguments.Length > 0)
        controllerGenericArgumentAttribute = controllerTypeGenericArguments[0].GetCustomAttribute<ApiExplorerAttribute>();

      // The methods property on the attribute defaults to None. We
      // pick the first one that has a different value, or All if none are set.
      // Ideally we'd make Methods a ApiExplorerMethods?, but .NET doesn't
      // allow us to do that.

      ApiExplorerMethods methods;
      if (controllerGenericArgumentAttribute != null && controllerGenericArgumentAttribute.Methods != ApiExplorerMethods.None)
        methods = controllerGenericArgumentAttribute.Methods;
      else if (controllerTypeAttribute != null && controllerTypeAttribute.Methods != ApiExplorerMethods.None)
        methods = controllerTypeAttribute.Methods;
      else if (attribute != null && attribute.Methods != ApiExplorerMethods.None)
        methods = attribute.Methods;
      else
        methods = ApiExplorerMethods.All;

      // Do we have specific methods configured?
      if (methods != ApiExplorerMethods.All)
      {
        // Match this action's method name to the enum.
        if (!Enum.TryParse<ApiExplorerMethods>(methodName, out var method))
          method = ApiExplorerMethods.Other;

        // If our method isn't in the enum, skip this method.
        if ((methods & method) == 0)
          return null;
      }
      
      var apiDescription = new ApiDescription
      {
        HttpMethod = action.HttpVerb,
        RelativePath = action.Route,
        ActionDescriptor = actionDescriptor,
        SupportedRequestFormats = { new ApiRequestFormat { MediaType = "application/json" } },
        SupportedResponseTypes = {
          new ApiResponseType
          {
            ApiResponseFormats = { new ApiResponseFormat { MediaType = "application/json" } },
            ModelMetadata = new ApiModelMetadata(ModelMetadataIdentity.ForType(action.ResponseType)),
            StatusCode = 200,
            Type = action.ResponseType
          },
        }
      };

      var routeTemplate = TemplateParser.Parse(action.Route);

      foreach (var parameterDescriptor in actionDescriptor.Parameters)
      {
        var apiParameterDescription = new ApiParameterDescription
        {
          Name = parameterDescriptor.Name,
          Type = parameterDescriptor.ParameterType,
          ParameterDescriptor = parameterDescriptor
        };

        if (routeTemplate.Parameters.Any(p => p.Name == parameterDescriptor.Name))
        {
          apiParameterDescription.Source = BindingSource.Path;
          apiParameterDescription.IsRequired = true;
        }

        apiDescription.ParameterDescriptions.Add(apiParameterDescription);
      }

      return apiDescription;
    }
    
    private string GetFriendlyControllerName(string controllerName)
    {
      return ControllerPostfixRe.Replace(controllerName, "");
    }
  }
}