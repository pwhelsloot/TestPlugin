using AMCS.Data.Entity;
using System;

namespace AMCS.PlatformFramework.Entity.Api.Recipe
{
  [EntityTable("DinnerPlanRecipe", "DinnerPlanRecipeId")]
  public class ApiDinnerPlanRecipe : EntityObject
  {
    [EntityMember]
    public int? DinnerPlanRecipeId { get; set; }

    [EntityMember]
    public int? DinnerPlanId { get; set; }

    [EntityMember]
    public int? DinnerCourseId { get; set; }

    [EntityMember]
    public int? RecipeId { get; set; }

    [EntityMember]
    public DateTime? PreparationDate { get; set; }
  }
}