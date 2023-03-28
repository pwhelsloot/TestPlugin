using AMCS.Data.Entity;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AMCS.PlatformFramework.Entity.Api.Recipe
{
  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class ApiRecipe : ApiSearchObject
  {
    [EntityMember]
    public int? RecipeId { get; set; }

    [EntityMember]
    public int? DinnerCourseId { get; set; }

    [EntityMember]
    public string Name { get; set; }

    [EntityMember]
    public string Method { get; set; }

    [EntityMember]
    public IList<ApiIngredient> Ingredients { get; set; }

  }
}
