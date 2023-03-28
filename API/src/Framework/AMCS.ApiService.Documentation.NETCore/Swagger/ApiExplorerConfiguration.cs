namespace AMCS.ApiService.Documentation.NETCore.Swagger
{
  using System;
  using System.Reflection;
  using System.Text.RegularExpressions;
  using AMCS.ApiService.Abstractions.MvcSetup;
  using AMCS.ApiService.Documentation.Abstractions.Swagger;
  using AMCS.ApiService.Documentation.Abstractions.Swagger.Descriptions;
  using AMCS.ApiService.Documentation.Abstractions.Swagger.Documentation;
  using AMCS.Data.Configuration;
  using AMCS.Data.Support;
  using Data.Entity;
  using Data.Server.Services;
  using MarkdownSharp;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Mvc.ApiExplorer;
  using Microsoft.AspNetCore.Mvc.Controllers;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.OpenApi.Any;
  using Microsoft.OpenApi.Models;
  using Newtonsoft.Json.Serialization;
  using NodaTime;
  using Swashbuckle.AspNetCore.SwaggerGen;

  internal class ApiExplorerConfiguration : IApiExplorerConfiguration
  {
    private static readonly Regex ErrorHeaderRe = new Regex("#([^\n]*)(.*)", RegexOptions.Singleline);
    private static readonly Regex ErrorTemplateRe = new Regex(@"\{\{=(title|body)\}\}");
    
    private readonly IApiActionDescriptionProvider apiActionDescriptionProvider;
    private readonly object syncRoot = new object();
    private readonly ApiDocumentationConfiguration documentationConfiguration;
    private readonly string serviceRoot;

    private bool setup;
    private MarkdownDocumentationManager markdownDocumentationManager;
    private string errorCodeTemplate;

    public ApiExplorerConfiguration(IMvcSetupService mvcSetupService, IAppSetupService appSetupService, ApiDocumentationConfiguration documentationConfiguration, IServiceRootResolver serviceRootResolver)
    {
      this.documentationConfiguration = documentationConfiguration;

      if (!string.IsNullOrEmpty(documentationConfiguration.ServiceRoot))
        serviceRoot = serviceRootResolver.GetServiceRoot(documentationConfiguration.ServiceRoot);

      if (string.IsNullOrEmpty(serviceRoot))
        serviceRoot = serviceRootResolver.ServiceRoot;

      if (!string.IsNullOrEmpty(serviceRoot) && serviceRoot.LastIndexOf('/') == serviceRoot.Length - 1)
      {
        serviceRoot = serviceRoot.TrimEnd('/');
      }

      apiActionDescriptionProvider = new ApiActionDescriptionProvider();

      mvcSetupService.RegisterSetup(mvcBuilder =>
      {
        mvcBuilder.AddNewtonsoftJson(options =>
        {
          options.SerializerSettings.ContractResolver = new ContractResolver();
        });
      });

      appSetupService.RegisterConfigureServices(ConfigureServices, 1000);
      appSetupService.RegisterConfigure(Configure, 1000);
    }

    public void RegisterApiAction(ApiExplorerAction action)
    {
      EnsureNotSetup();

      apiActionDescriptionProvider.RegisterApiAction(action);
    }

    public void RegisterMessageService(Type controllerType, Type responseType, string route, string controllerName)
    {
      EnsureNotSetup();

      var action = new ApiExplorerAction
      {
        ControllerType = controllerType,
        ResponseType = responseType,
        ActionName = "Perform",
        HttpVerb = "POST",
        Route = route,
        ControllerName = controllerName
      };

      apiActionDescriptionProvider.RegisterApiAction(action);
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

    private void ConfigureServices(IServiceCollection services)
    {
      services.AddSwaggerGenNewtonsoftSupport();
      services.AddSwaggerGen(options =>
      {
        var defaultVersion = documentationConfiguration.Versions[0];

        foreach (var version in documentationConfiguration.Versions)
        {
          options.SwaggerDoc(version.Version, new OpenApiInfo { Title = version.Title, Version = version.Version });
        }

        options.TagActionsBy(api => new[] { ((ControllerActionDescriptor)api.ActionDescriptor).ControllerName });

        options.DocInclusionPredicate((version, desc) =>
        {
          if (!desc.TryGetMethodInfo(out var methodInfo))
            return false;

          var descriptionVersion = defaultVersion.Version;

          var typeArguments = methodInfo.DeclaringType?.GenericTypeArguments;

          if (typeArguments != null && typeArguments.Length > 0)
          {
            descriptionVersion = typeArguments[0].GetCustomAttribute<ApiExplorerAttribute>(true)?.Version ?? descriptionVersion;
          }

          return string.Equals(version, descriptionVersion, StringComparison.OrdinalIgnoreCase);
        });

        options.CustomSchemaIds(SchemaIdSelector);

        if (documentationConfiguration.MarkdownDocumentationLocation != null)
        {
          markdownDocumentationManager = new MarkdownDocumentationManager(documentationConfiguration.MarkdownDocumentationLocation);
          if (!string.IsNullOrEmpty(documentationConfiguration.ErrorCodeTemplate))
            errorCodeTemplate = markdownDocumentationManager.GetDocument(documentationConfiguration.ErrorCodeTemplate);

          options.OperationFilter<MarkdownOperationFilter>(markdownDocumentationManager);
        }

        MapSwaggerTypes(options);
      });
      
      services.AddSingleton<IApiDescriptionProvider>(apiActionDescriptionProvider);
    }

    private void Configure(IApplicationBuilder app)
    {
      lock (syncRoot)
      {
        EnsureNotSetup();

        this.setup = true;
      }

      app.UseSwagger(options =>
      {
        options.RouteTemplate = "swagger/{documentName}/swagger.json";
      });

      // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
      // specifying the Swagger JSON endpoint.
      app.UseSwaggerUI(options =>
      {
        options.DefaultModelsExpandDepth(-1);

        foreach (var version in documentationConfiguration.Versions)
        {
          options.SwaggerEndpoint($"/swagger/{ version.Version }/swagger.json", $"{version.Title}");
        }
      });
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

      return ApiResourceUtils.FriendlyId(type, true);
    }

    private void MapSwaggerTypes(SwaggerGenOptions options)
    {
      var localDate = JsonUtil.Print(new LocalDate(2000, 1, 1)).ToString();
      options.MapType<LocalDate>(() => new OpenApiSchema { Type = "string", Format = "localDate", Example = new OpenApiString(localDate) });
      options.MapType<LocalDate?>(() => new OpenApiSchema { Type = "string", Format = "localDate", Example = new OpenApiString(localDate) });

      var localTime = JsonUtil.Print(new LocalTime(0, 0, 0, 0)).ToString();
      options.MapType<LocalTime>(() => new OpenApiSchema { Type = "string", Format = "localTime", Example = new OpenApiString(localTime) });
      options.MapType<LocalTime?>(() => new OpenApiSchema { Type = "string", Format = "localTime", Example = new OpenApiString(localTime) });

      var localDateTime = new LocalDateTime(2000, 1, 1, 0, 0, 0, 0);
      var printedLocalDateTime = JsonUtil.Print(localDateTime).ToString();
      options.MapType<LocalDateTime>(() => new OpenApiSchema { Type = "string", Format = "localDateTime", Example = new OpenApiString(printedLocalDateTime) });
      options.MapType<LocalDateTime?>(() => new OpenApiSchema { Type = "string", Format = "localDateTime", Example = new OpenApiString(printedLocalDateTime) });

      var offsetDateTime = JsonUtil.Print(new OffsetDateTime(localDateTime, Offset.FromHours(1))).ToString();
      options.MapType<OffsetDateTime>(() => new OpenApiSchema { Type = "string", Format = "offsetDateTime", Example = new OpenApiString(offsetDateTime) });
      options.MapType<OffsetDateTime?>(() => new OpenApiSchema { Type = "string", Format = "offsetDateTime", Example = new OpenApiString(offsetDateTime) });

      var zonedDateTime = JsonUtil.Print(new ZonedDateTime(localDateTime, DateTimeZoneProviders.Tzdb["Europe/London"], Offset.FromHours(0))).ToString();
      options.MapType<ZonedDateTime>(() => new OpenApiSchema { Type = "string", Format = "zonedDateTime", Example = new OpenApiString(zonedDateTime) });
      options.MapType<ZonedDateTime?>(() => new OpenApiSchema { Type = "string", Format = "zonedDateTime", Example = new OpenApiString(zonedDateTime) });

      var dateTime = JsonUtil.Print(new DateTime(2000, 1, 1, 0, 0, 0, 0)).ToString();
      options.MapType<DateTime>(() => new OpenApiSchema { Type = "string", Format = "dateTime", Example = new OpenApiString(dateTime) });
      options.MapType<DateTime?>(() => new OpenApiSchema { Type = "string", Format = "dateTime", Example = new OpenApiString(dateTime) });

      var dateTimeOffset = JsonUtil.Print(new DateTimeOffset(2000, 1, 1, 0, 0, 0, 0, TimeSpan.FromHours(1))).ToString();
      options.MapType<DateTimeOffset>(() => new OpenApiSchema { Type = "string", Format = "dateTimeOffset", Example = new OpenApiString(dateTimeOffset) });
      options.MapType<DateTimeOffset?>(() => new OpenApiSchema { Type = "string", Format = "dateTimeOffset", Example = new OpenApiString(dateTimeOffset) });
    }

    private class ContractResolver : CamelCasePropertyNamesContractResolver
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
  }
}
