namespace AMCS.Data.Entity.Workflow
{
  [EntityTable("WorkflowProvider", nameof(WorkflowProviderId))]
  public class WorkflowProviderEntity : CacheCoherentEntity
  {
    [EntityMember]
    public int? WorkflowProviderId { get; set; }

    [EntityMember]
    public string Name { get; set; }

    [EntityMember]
    public string SystemCategory { get; set; }

    [EntityMember]
    public string Description { get; set; }

    [EntityMember]
    public string Endpoint { get; set; }

    [EntityMember]
    public string ApiVersion { get; set; }

    public override int? GetId() => WorkflowProviderId;

    private static readonly string[] ValidatedProperties =
    {
      nameof(Name),
      nameof(Endpoint),
      nameof(ApiVersion),
      nameof(SystemCategory)
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
            return "Endpoint cannot be empty";
          break;
        case nameof(ApiVersion):
          if (string.IsNullOrWhiteSpace(ApiVersion))
            return "ApiVersion cannot be empty";
          break;
        case nameof(SystemCategory):
          if (string.IsNullOrWhiteSpace(SystemCategory))
            return "SystemCategory cannot be empty";
          break;
      }

      return null;
    }

    public ApiWorkflowProvider ToApiWorkflowProvider()
    {
      return new ApiWorkflowProvider
      {
        WorkflowProviderId = WorkflowProviderId,
        Name = Name,
        Description = Description,
        Endpoint = Endpoint,
        ApiVersion = ApiVersion,
        SystemCategory = SystemCategory
      };
    }
  }
}