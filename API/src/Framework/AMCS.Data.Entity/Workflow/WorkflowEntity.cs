namespace AMCS.Data.Entity.Workflow
{
  public abstract class WorkflowEntity : EntityObject
  {
    [EntityMember]
    public string ProviderName { get; set; }
    
    [EntityMember]
    public string TenantId { get; set; }
  }
}