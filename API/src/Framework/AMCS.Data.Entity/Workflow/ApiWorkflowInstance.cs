using System;
using System.Collections.Generic;
using System.Text.Json;

namespace AMCS.Data.Entity.Workflow
{
  [EntityTable("WorkflowProcessInstance", "Id")]
  [ApiExplorer(Mode = ApiMode.External)]
  public class ApiWorkflowInstance : WorkflowEntity
  {
    [EntityMember]
    public Guid WorkflowInstanceId { get; set; }

    [EntityMember]
    public Guid WorkflowDefinitionId { get; set; }

    [EntityMember]
    public string DefinitionName { get; set; }

    [EntityMember]
    public string State { get; set; }

    [EntityMember]
    public DateTime Started { get; set; }

    [EntityMember]
    public string UserContext { get; set; }

    [EntityMember]
    public int StatusId { get; set; }

    [EntityMember]
    public string Status { get; set; }

    private static readonly string[] ValidatedProperties =
    {
      nameof(TenantId),
      nameof(UserContext)
    };

    public override int? GetId()
    {
      return null;
    }

    public override string[] GetValidatedProperties() => ValidatedProperties;

    protected override string GetValidationError(string propertyName)
    {
      switch (propertyName)
      {
        case nameof(TenantId):
          if (string.IsNullOrWhiteSpace(TenantId))
            return "Tenant Id cannot be empty";
          break;
        case nameof(UserContext):
          if (string.IsNullOrWhiteSpace(UserContext))
            return "User cannot be empty";
          break;
      }

      return null;
    }
  }
}