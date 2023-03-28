using AMCS.Data.Entity;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AMCS.PlatformFramework.Entity.Api.Recipe
{
  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class ApiRecipeStepperSave : ApiSearchObject
  {
    [EntityMember]
    public ApiDinnerPlan DinnerPlan { get; set; }

    [EntityMember]
    public IList<ApiDinnerPlanRecipe> DinnerPlanRecipes { get; set; }
  }
}