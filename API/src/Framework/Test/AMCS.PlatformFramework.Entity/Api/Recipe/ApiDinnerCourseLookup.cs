using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.Entity.Api.Recipe
{
  public enum Course
  {
    STARTER = 1,
    MAIN = 2,
    DESSERT = 3,
    FAKE = 3,
  }

  [EntityTable("DinnerCourse", "DinnerCourseId")]
  public class ApiDinnerCourseLookup : EntityObject
  {
    [EntityMember]
    public int? DinnerCourseId { get; set; }

    [EntityMember]
    public string Description { get; set; }
  }
}
