namespace AMCS.Data.Entity.Workflow
{
  [ApiExplorer(Mode = ApiMode.External, Methods = ApiExplorerMethods.GetCollection)]
  public class ApiWorkflowProvider : WorkflowProviderEntity
  {
    public override int? GetId() => WorkflowProviderId;

    private static readonly string[] ValidatedProperties =
    {
      nameof(Name),
      nameof(Endpoint),
      nameof(ApiVersion)
    };

    public override string[] GetValidatedProperties() => ValidatedProperties;

    protected override string GetValidationError(string propertyName)
    {
      switch (propertyName)
      {
        case nameof(Name):
          if (string.IsNullOrWhiteSpace(Name))
            return "Name cannot be empty";
          break;
        case nameof(Endpoint):
          if (string.IsNullOrWhiteSpace(Endpoint))
            return "EndPoint cannot be empty";
          break;
        case nameof(ApiVersion):
          if (string.IsNullOrWhiteSpace(ApiVersion))
            return "ApiVersion cannot be empty";
          break;
      }

      return null;
    }
  }
}
