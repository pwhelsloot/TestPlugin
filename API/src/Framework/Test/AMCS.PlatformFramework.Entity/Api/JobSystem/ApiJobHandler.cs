using System.Collections.Generic;
using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.Entity.Api.JobSystem
{
  [EntityTable("NA", "NA")]
  public class ApiJobHandler : EntityObject
  {
    [EntityMember]
    public string Type { get; set; }

    [EntityMember]
    public string DisplayName { get; set; }

    [EntityMember]
    public bool AllowScheduling { get; set; }

    [EntityMember]
    public string DuplicateMode { get; set; }

    [EntityMember]
    public ApiJobHandlerParameter RequestParameterInfo { get; set; }

    [EntityMember]
    public List<string> CompatibleQueues { get; set; }

    public override int? GetId() => null;
  }
}
