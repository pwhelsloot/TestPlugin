namespace AMCS.ApiService.Documentation.NETCore.Swagger
{
  using System.Text.RegularExpressions;
  using AMCS.ApiService.Documentation.Abstractions.Swagger.Documentation;
  using Microsoft.OpenApi.Models;
  using Swashbuckle.AspNetCore.SwaggerGen;

  internal class MarkdownOperationFilter : IOperationFilter
  {
    private static readonly Regex HeaderRe = new Regex(@"^\s*#(.*?)$", RegexOptions.Multiline | RegexOptions.Compiled);
    private readonly MarkdownDocumentationManager documentationManager;

    public MarkdownOperationFilter(MarkdownDocumentationManager documentationManager)
    {
      this.documentationManager = documentationManager;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      var relativePath = context.ApiDescription.RelativePath.Trim('/').Replace('/', '.');
      var document = documentationManager.GetDocumentation(relativePath, context.ApiDescription.HttpMethod);

      if (!string.IsNullOrEmpty(document))
        operation.Summary = GetHeader(document);

      operation.Description = document;
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