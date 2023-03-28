using AMCS.Data.Entity;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AMCS.PlatformFramework.Entity.Api.Recipe
{
  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class ApiDinnerPlanEditorData : ApiSearchObject
  {
    [EntityMember]
    public int? DinnerPlanId { get; set; }

    [EntityMember]
    public ApiDinnerPlan DataModel { get; set; }

    [EntityMember]
    public IList<ApiDinnerCourseLookup> Courses { get; set; }

    [EntityMember]
    public IList<ApiDinnerPlanDifficultyLevelLookup> DifficultyLevels { get; set; }
  }
}