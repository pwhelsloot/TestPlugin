using AMCS.Data.Entity;
using System;
using System.Collections.Generic;

namespace AMCS.PlatformFramework.Entity.Api.Recipe
{
  [EntityTable("DinnerPlan", "DinnerPlanId")]
  public class ApiDinnerPlan : EntityObject
  {
    [EntityMember]
    public int? DinnerPlanId { get; set; }

    [EntityMember]
    public string Name { get; set; }

    [EntityMember]
    public DateTime? EstimatedTime { get; set; }

    [EntityMember]
    public int? DifficultyLevelId { get; set; }

    [EntityMember]
    public IList<int?> CourseIds { get; set; }
  }
}