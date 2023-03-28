namespace AMCS.Data.Server.Plugin
{
  using System;

  public class UdfMetadataDeleteJobRequest
  {
    public int? MaximumDeleteAmount { get; set; }

    public TimeSpan? MaximumRuntimeMinutes { get; set; }
  }
}