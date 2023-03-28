using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Mvc;
using System.Web.Routing;
using AMCS.ApiService.Abstractions.MvcSetup;
using AMCS.ApiService.Documentation.Abstractions.Swagger;
using AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions;
using AMCS.ApiService.Documentation.Abstractions.Swagger.Documentation;
using AMCS.ApiService.Documentation.NETFramework.Swagger.Descriptions;
using AMCS.Data.Configuration;
using AMCS.Data.Entity;
using AMCS.Data.Support;
using MarkdownSharp;
using Newtonsoft.Json.Serialization;
using NodaTime;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using Swashbuckle.Swagger.Annotations;

namespace AMCS.ApiService.Documentation.NETFramework.Swagger
{
  [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1008:OpeningParenthesisMustBeSpacedCorrectly", Justification = "C# 7 language construct")]
  [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:ClosingParenthesisMustBeSpacedCorrectly", Justification = "C# 7 language construct")]
  internal class ApiExplorerConfiguration : IApiExplorerConfiguration
  {
    private static readonly JsonMediaTypeFormatter Formatter = CreateFormatter();
    private static readonly Regex ControllerPostfixRe = new Regex(@"Controller(_\d+)?$", RegexOptions.Compiled);
    private static readonly Regex ErrorHeaderRe = new Regex("#([^\n]*)(.*)", RegexOptions.Singleline);
    private static readonly Regex ErrorTemplateRe = new Regex(@"\{\{=(title|body)\}\}");
    private static readonly Regex HeaderRe = new Regex(@"^\s*#(.*?)$", RegexOptions.Multiline | RegexOptions.Compiled);

    private static JsonMediaTypeFormatter CreateFormatter()
    {
      var formatter = new JsonMediaTypeFormatter();

      foreach (var mediaType in formatter.SupportedMediaTypes.ToList())
      {
        if (mediaType.MediaType != "application/json")
          formatter.SupportedMediaTypes.Remove(mediaType);
      }

      return formatter;
    }

    private readonly object syncRoot = new object();
    private bool setup;
    private readonly HttpConfiguration configuration;
    private readonly IApiExplorer apiExplorer;
    private readonly string serviceRoot;
    private MarkdownDocumentationManager markdownDocumentationManager;
    private readonly ApiDocumentationConfiguration documentationConfiguration;
    private string errorCodeTemplate;

    public ApiExplorerConfiguration(IMvcSetupService mvcSetup, ApiDocumentationConfiguration documentationConfiguration, IServiceRootResolver serviceRootResolver)
    {
      this.documentationConfiguration = documentationConfiguration;
      configuration = GlobalConfiguration.Configuration;
      apiExplorer = configuration.Services.GetApiExplorer();

      if (!string.IsNullOrEmpty(documentationConfiguration.ServiceRoot))
        serviceRoot = serviceRootResolver.GetServiceRoot(documentationConfiguration.ServiceRoot);

      if (string.IsNullOrEmpty(serviceRoot))
        serviceRoot = serviceRootResolver.ServiceRoot;

      if (!string.IsNullOrEmpty(serviceRoot) && serviceRoot.LastIndexOf('/') == serviceRoot.Length - 1)
      {
        serviceRoot = serviceRoot.TrimEnd('/');
      }

      mvcSetup.Register(SetupMvc, 1000);
    }

    private void SetupMvc(IMvcSetup setup)
    {
      lock (syncRoot)
      {
        EnsureNotSetup();

        this.setup = true;
      }

      // If all controllers are migrated to web api this method is not needed
      // as web api is supported by swagger out of the box.

      PopulateRoutes(setup.Routes);

      configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new ContractResolver();

      configuration
        .EnableSwagger(p =>
        {
          p.RootUrl(req => serviceRoot);

          var defaultVersion = documentationConfiguration.Versions[0];

          p.MultipleApiVersions(
            (p1, p2) => VersionSupportResolver(p1, p2, defaultVersion.Version),
            p1 => BuildVersions(p1, documentationConfiguration.Versions));

          p.SchemaId(SchemaIdSelector);

          if (documentationConfiguration.MarkdownDocumentationLocation != null)
          {
            markdownDocumentationManager = new MarkdownDocumentationManager(documentationConfiguration.MarkdownDocumentationLocation);
            if (!string.IsNullOrEmpty(documentationConfiguration.ErrorCodeTemplate))
              errorCodeTemplate = markdownDocumentationManager.GetDocument(documentationConfiguration.ErrorCodeTemplate);

            p.OperationFilter(() => new MarkdownOperationFilter(markdownDocumentationManager));
          }

          MapSwaggerTypes(p);
        })
        .EnableSwaggerUi();
    }

    private void BuildVersions(VersionInfoBuilder versionInfoBuilder, IList<ApiDocumentationVersion> versions)
    {
      foreach (var version in versions)
      {
        if (!version.IsHidden)
          versionInfoBuilder.Version(version.Version, version.Title);
      }
    }

    private bool VersionSupportResolver(ApiDescription description, string version, string defaultVersion)
    {
      var descriptionVersion = ((CustomHttpActionDescriptor)description.ActionDescriptor).Version ?? defaultVersion;

      return string.Equals(version, descriptionVersion, StringComparison.OrdinalIgnoreCase);
    }

    private void EnsureNotSetup()
    {
      lock (syncRoot)
      {
        if (setup)
          throw new InvalidOperationException("Setup of the API explorer has already been completed");
      }
    }

    private string SchemaIdSelector(Type type)
    {
      var attributes = type.GetCustomAttributes(typeof(ApiExplorerAttribute), true);
      if (attributes.Length > 0)
      {
        var attribute = (ApiExplorerAttribute)attributes[0];

        string name = attribute.Name;
        if (name != null)
          return name;

        if (attribute.ContractProvider != null)
        {
          var contractProvider = (IApiExplorerContractProvider)Activator.CreateInstance(attribute.ContractProvider);
          name = contractProvider.GetTypeName(type);
          if (name != null)
            return name;
        }
      }

      return type.FriendlyId();
    }

    private void MapSwaggerTypes(SwaggerDocsConfig config)
    {
      var localDate = JsonUtil.Print(new LocalDate(2000, 1, 1));
      config.MapType<LocalDate>(() => new Schema { type = "string", format = "localDate", example = localDate });
      config.MapType<LocalDate?>(() => new Schema { type = "string", format = "localDate", example = localDate });

      var localTime = JsonUtil.Print(new LocalTime(0, 0, 0, 0));
      config.MapType<LocalTime>(() => new Schema { type = "string", format = "localTime", example = localTime });
      config.MapType<LocalTime?>(() => new Schema { type = "string", format = "localTime", example = localTime });

      var localDateTime = new LocalDateTime(2000, 1, 1, 0, 0, 0, 0);
      var printedLocalDateTime = JsonUtil.Print(localDateTime);
      config.MapType<LocalDateTime>(() => new Schema { type = "string", format = "localDateTime", example = printedLocalDateTime });
      config.MapType<LocalDateTime?>(() => new Schema { type = "string", format = "localDateTime", example = printedLocalDateTime });

      var offsetDateTime = JsonUtil.Print(new OffsetDateTime(localDateTime, Offset.FromHours(1)));
      config.MapType<OffsetDateTime>(() => new Schema { type = "string", format = "offsetDateTime", example = offsetDateTime });
      config.MapType<OffsetDateTime?>(() => new Schema { type = "string", format = "offsetDateTime", example = offsetDateTime });

      var zonedDateTime = JsonUtil.Print(new ZonedDateTime(localDateTime, DateTimeZoneProviders.Tzdb["Europe/London"], Offset.FromHours(0)));
      config.MapType<ZonedDateTime>(() => new Schema { type = "string", format = "zonedDateTime", example = zonedDateTime });
      config.MapType<ZonedDateTime?>(() => new Schema { type = "string", format = "zonedDateTime", example = zonedDateTime });

      var dateTime = JsonUtil.Print(new DateTime(2000, 1, 1, 0, 0, 0, 0));
      config.MapType<DateTime>(() => new Schema { type = "string", format = "dateTime", example = dateTime });
      config.MapType<DateTime?>(() => new Schema { type = "string", format = "dateTime", example = dateTime });

      var dateTimeOffset = JsonUtil.Print(new DateTimeOffset(2000, 1, 1, 0, 0, 0, 0, TimeSpan.FromHours(1)));
      config.MapType<DateTimeOffset>(() => new Schema { type = "string", format = "dateTimeOffset", example = dateTimeOffset });
      config.MapType<DateTimeOffset?>(() => new Schema { type = "string", format = "dateTimeOffset", example = dateTimeOffset });
    }

    private void PopulateRoutes(RouteCollection routes)
    {
      // This method is needed only for ASP.NET MVC 5, if controllers are switched to ASP.NET WEB API this method can be removed.

      if (apiExplorer.ApiDescriptions.Any())
        return;

      foreach (var route in routes.OfType<Route>())
      {
        if (!(route.DataTokens?["MS_DirectRouteActions"] is ActionDescriptor[] actionDescriptors))
          continue;

        var actionDescriptor = actionDescriptors[0];

        var methods = actionDescriptor.ControllerDescriptor.ControllerType
          .GetMethods(BindingFlags.Public | BindingFlags.Instance)
          .Where(p => p.Name.Equals(actionDescriptor.ActionName, StringComparison.OrdinalIgnoreCase));

        var controllerDescriptor = new HttpControllerDescriptor(
          configuration,
          actionDescriptor.ControllerDescriptor.ControllerName,
          actionDescriptor.ControllerDescriptor.ControllerType);

        foreach (var method in methods)
        {
          var returnType = method.GetCustomAttribute<SwaggerResponseAttribute>()?.Type;
          string version = method.GetCustomAttribute<ApiExplorerAttribute>()?.Version;

          var result = new ApiDescription
          {
            HttpMethod = MapToHttpVerb(method),
            RelativePath = route.Url,
            ActionDescriptor = new CustomHttpActionDescriptor(controllerDescriptor, method, returnType, version)
          };

          result.SupportedRequestBodyFormatters.Add(Formatter);
          result.SupportedResponseFormatters.Add(Formatter);

          foreach (var parameter in method.GetParameters().Where(p => !p.IsRetval))
          {
            result.ParameterDescriptions.Add(new ApiParameterDescription
            {
              Name = parameter.Name,
              ParameterDescriptor = new ReflectedOptionalHttpParameterDescriptor
              {
                ActionDescriptor = result.ActionDescriptor,
                Configuration = configuration,
                ParameterInfo = parameter,
              },
              Source = ApiParameterSource.FromBody
            });
          }

          apiExplorer.ApiDescriptions.Add(result);
        }
      }
    }

    private static HttpMethod MapToHttpVerb(MethodInfo method)
    {
      if (method.GetCustomAttribute<System.Web.Mvc.HttpGetAttribute>() != null)
        return HttpMethod.Get;
      if (method.GetCustomAttribute<System.Web.Mvc.HttpPostAttribute>() != null)
        return HttpMethod.Post;
      if (method.GetCustomAttribute<System.Web.Mvc.HttpPutAttribute>() != null)
        return HttpMethod.Put;
      if (method.GetCustomAttribute<System.Web.Mvc.HttpDeleteAttribute>() != null)
        return HttpMethod.Delete;

      if (method.Name.StartsWith("get", StringComparison.OrdinalIgnoreCase))
        return HttpMethod.Get;
      if (method.Name.StartsWith("post", StringComparison.OrdinalIgnoreCase))
        return HttpMethod.Post;
      if (method.Name.StartsWith("put", StringComparison.OrdinalIgnoreCase))
        return HttpMethod.Put;
      if (method.Name.StartsWith("delete", StringComparison.OrdinalIgnoreCase))
        return HttpMethod.Delete;

      return null;
    }

    public void RegisterApiAction(ApiExplorerAction action)
    {
      EnsureNotSetup();

      var (apiDescription, httpDescriptor) = GetApiDescription(action, configuration);
      if (apiDescription == null)
        return;

      if (action.HttpVerb == "PUT" || action.HttpVerb == "POST")
      {
        apiDescription.ParameterDescriptions.Add(new ApiParameterDescription
        {
          Source = ApiParameterSource.FromBody,
          ParameterDescriptor = new ReflectedHttpParameterDescriptor
          {
            ActionDescriptor = httpDescriptor,
            Configuration = configuration,
            ParameterInfo = new EntityParameterInfo(action.ResponseType)
          }
        });
      }

      apiExplorer.ApiDescriptions.Add(apiDescription);
    }

    public void RegisterMessageService(Type controllerType, Type responseType, string route, string controllerName)
    {
      EnsureNotSetup();

      var configuration = GlobalConfiguration.Configuration;
      var apiExplorer = configuration.Services.GetApiExplorer();
      var controllerDescriptor = new ReflectedControllerDescriptor(controllerType);

      foreach (var actionDescriptor in controllerDescriptor.GetCanonicalActions())
      {
        var action = new ApiExplorerAction
        {
          ControllerType = controllerType,
          ResponseType = responseType,
          ActionName = actionDescriptor.ActionName,
          HttpVerb = "POST",
          Route = route,
          ControllerName = controllerName
        };

        var (apiDescription, _) = GetApiDescription(action, configuration);
        if (apiDescription == null)
          continue;

        foreach (var parameter in apiDescription.ParameterDescriptions)
        {
          parameter.Source = ApiParameterSource.FromBody;
          parameter.Name = null;
        }

        apiExplorer.ApiDescriptions.Add(apiDescription);
      }
    }

    public string GetErrorCodeUrl(int errorCode)
    {
      if (serviceRoot == null)
        return null;

      return serviceRoot + "/api/documentation/error/" + errorCode;
    }

    public string RenderMarkdownDocumentation(string path)
    {
      string content = markdownDocumentationManager.GetDocument(path, ".md");
      if (content == null)
        return null;

      string title = string.Empty;

      if (errorCodeTemplate != null)
      {
        var match = ErrorHeaderRe.Match(content);
        if (match.Success)
        {
          title = match.Groups[1].Value.Trim();
          content = match.Groups[2].Value.Trim();
        }
      }

      string renderedContent = new Markdown().Transform(content);

      if (errorCodeTemplate != null)
      {
        renderedContent = ErrorTemplateRe.Replace(errorCodeTemplate, p =>
        {
          switch (p.Groups[1].Value)
          {
            case "title":
              return title;
            case "body":
              return renderedContent;
            default:
              throw new InvalidOperationException();
          }
        });
      }

      return renderedContent;
    }

    private (ApiDescription ApiDescription, HttpActionDescriptor HttpDescriptor) GetApiDescription(ApiExplorerAction action, HttpConfiguration configuration)
    {
      var methodName = action.MethodName ?? action.ActionName;

      // We try to be a bit flexible in where we look for the ApiExplorer
      // attribute. Specifically we look at the response type and the
      // controller type. However, the controller type may be a generic
      // controller, e.g. for EntityObject's. In that case, we try the first
      // generic argument of the controller type as well.

      var attribute = action.ResponseType.GetCustomAttribute<ApiExplorerAttribute>();
      var controllerTypeAttribute = action.ControllerType.GetCustomAttribute<ApiExplorerAttribute>();
      ApiExplorerAttribute controllerGenericArgumentAttribute = null;
      var controllerTypeGenericArguments = action.ControllerType.GetGenericArguments();
      if (controllerTypeGenericArguments.Length > 0)
        controllerGenericArgumentAttribute = controllerTypeGenericArguments[0].GetCustomAttribute<ApiExplorerAttribute>();

      // Pick the first attribute that has a version set.

      string version =
        controllerGenericArgumentAttribute?.Version ??
        controllerTypeAttribute?.Version ??
        attribute?.Version;

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
          return (null, null);
      }

      var methodInfo = action.ControllerType.GetMethod(methodName);
      var httpControllerDescriptor = new HttpControllerDescriptor(configuration, GetFriendlyControllerName(action.ControllerName), action.ControllerType);
      var httpActionDescriptor = new CustomHttpActionDescriptor(httpControllerDescriptor, methodInfo, action.ResponseType, version);

      var result = new ApiDescription
      {
        HttpMethod = new HttpMethod(action.HttpVerb),
        RelativePath = action.Route,
        ActionDescriptor = httpActionDescriptor,
        SupportedRequestBodyFormatters =
        {
          Formatter
        },
        SupportedResponseFormatters =
        {
          Formatter
        }
      };

      foreach (var parameter in methodInfo.GetParameters())
      {
        result.ParameterDescriptions.Add(new ApiParameterDescription
        {
          Name = parameter.Name,
          ParameterDescriptor = new ReflectedOptionalHttpParameterDescriptor
          {
            ActionDescriptor = httpActionDescriptor,
            Configuration = configuration,
            ParameterInfo = parameter,
          }
        });
      }

      return (result, httpActionDescriptor);
    }

    private string GetFriendlyControllerName(string controllerName)
    {
      return ControllerPostfixRe.Replace(controllerName, "");
    }

    private class ContractResolver : DefaultContractResolver
    {
      protected override JsonContract CreateContract(Type objectType)
      {
        var attribute = objectType.GetCustomAttribute<ApiExplorerAttribute>();
        if (attribute?.ContractProvider != null)
        {
          var contractProvider = (IApiExplorerContractProvider)Activator.CreateInstance(attribute.ContractProvider);
          var contract = contractProvider.CreateContract(objectType, this);
          if (contract != null)
            return contract;
        }

        return base.CreateContract(objectType);
      }
    }

    private class MarkdownOperationFilter : IOperationFilter
    {
      private readonly MarkdownDocumentationManager documentationManager;

      public MarkdownOperationFilter(MarkdownDocumentationManager documentationManager)
      {
        this.documentationManager = documentationManager;
      }

      public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
      {
        var relativePath = apiDescription.RelativePath.Trim('/').Replace('/', '.');

        var document = documentationManager.GetDocumentation(relativePath, apiDescription.HttpMethod.Method);

        if (!string.IsNullOrEmpty(document))
          operation.summary = GetHeader(document);

        operation.description = document;
      }

      private string GetHeader(string document)
      {
        var match = HeaderRe.Match(document);
        if (match.Success)
          return match.Groups[1].Value.Trim();
        return null;
      }
    }
  }
}
