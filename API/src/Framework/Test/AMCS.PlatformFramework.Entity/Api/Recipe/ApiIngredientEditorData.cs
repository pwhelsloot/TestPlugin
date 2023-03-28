using AMCS.Data.Entity;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AMCS.PlatformFramework.Entity.Api.Recipe
{
  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class ApiIngredientEditorData : ApiSearchObject
  {
    [EntityMember]
    public int? IngredientId { get; set; }

    [EntityMember]
    public ApiIngredient DataModel { get; set; }

    [EntityMember]
    public ApiMeasurement SelectedMeasurement{ get; set; }

    [EntityMember]
    public IList<ApiIngredientType> IngredientTypes { get; set; }
  }
}
