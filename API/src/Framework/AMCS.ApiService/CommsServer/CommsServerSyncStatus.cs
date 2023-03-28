namespace AMCS.ApiService.CommsServer
{
  using System;

  public class CommsServerSyncStatus
  {
    public DateTime? SyncLastCompleted { get; }

    public CommsServerSyncStatus(DateTime? syncLastCompleted)
    {
      SyncLastCompleted = syncLastCompleted;
    }
  }
}
