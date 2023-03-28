namespace AMCS.Data.Entity.Workflow
{
  using System;

  [EntityTable("WorkflowDefinition", "WorkflowDefinitionId")]
  [ApiExplorer(Mode = ApiMode.External)]
  public class ApiWorkflowDefinition : WorkflowEntity
  {
    [EntityMember]
    public Guid WorkflowDefinitionId { get; set; }

    [EntityMember]
    public string Name { get; set; }

    [EntityMember]
    public string Definition { get; set; }
    
    [EntityMember]
    public string DesignerUrl { get; set; }
    
    [EntityMember]
    public string Version { get; set; }
    
    [EntityMember]
    public DateTime LastModifiedDate { get; set; }

    private static readonly string[] ValidatedProperties =
    {
      nameof(Name),
      nameof(Definition),
      nameof(Version),
      nameof(TenantId)
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
        case nameof(Definition):
          if (string.IsNullOrWhiteSpace(Definition))
            return "Definition cannot be empty";
          break;
        case nameof(Version):
          if (string.IsNullOrWhiteSpace(Version))
            return "Version cannot be empty";
          break;
        case nameof(TenantId):
          if (string.IsNullOrWhiteSpace(TenantId))
            return "Tenant Id cannot be empty";
          break;
      }
    
      return null;
    }
  }
}
