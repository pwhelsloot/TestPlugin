using AMCS.Data.Entity;
using System;
using System.Runtime.Serialization;

namespace AMCS.PlatformFramework.Entity.Api.Recipe
{
  public enum IngredientType
  {
    SPICES = 1,
    PANTRY = 2,
    REFRIGERATOR = 3,
    CONDIMENTS = 4,
    VEGETABLES = 5,
    CANNEDGOODS = 6
  }

  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class ApiIngredientType : ApiSearchObject
  {
    [EntityMember]
    public int? ApiIngredientTypeId { get; set; }

    [EntityMember]
    public string Description { get; set; }
  }
}
