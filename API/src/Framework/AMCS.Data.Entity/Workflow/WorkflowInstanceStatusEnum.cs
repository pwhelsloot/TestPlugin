namespace AMCS.Data.Entity.Workflow
{
  public enum WorkflowInstanceStatusEnum
  {
    Initialized,
    Running,
    Idled,
    Finalized,
    Terminated,
    Error,
    Suspended,
    Canceled,
    NotFound = 255,
    Unknown = 254,
  }
}
