namespace AMCS.Data.Entity.Workflow
{
  using System.Text.Json.Serialization;

  public class WorkflowUserContext
  {
    public const string UserIdentityToken = "sub_uid";
    public const string UserConnectionIdToken = "sub_cid";
    public const string TenantIdToken = "sub_tid";
    public const string WorkflowUserContextToken = "__UserContext";
    
    [JsonPropertyName(UserIdentityToken)]
    public string Email { get; set; }
    
    [JsonPropertyName(TenantIdToken)]
    public string TenantId { get; set; }
    
    [JsonPropertyName(UserConnectionIdToken)]
    public string UserConnectionId { get; set; }
  }
}