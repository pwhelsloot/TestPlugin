using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.Entity.Api.Recipe
{
  public enum DifficultyLevel
  {
    EASY = 1,
    MEDIUM = 2,
    HARD = 3,
  }

  [EntityTable("DinnerCourse", "DinnerCourseId")]
  public class ApiDinnerPlanDifficultyLevelLookup : EntityObject
  {
    [EntityMember]
    public int? DinnerPlanDifficultyLevelId { get; set; }

    [EntityMember]
    public string Description { get; set; }
  }
}