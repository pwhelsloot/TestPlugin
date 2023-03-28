using AMCS.Data.Entity;
using System;
using System.Runtime.Serialization;

namespace AMCS.PlatformFramework.Entity.Api.Recipe
{
  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class ApiIngredient : ApiSearchObject
  {
    [EntityMember]
    public int IngredientId { get; set; }

    [EntityMember]
    public string Name { get; set; }

    [EntityMember]
    public double? Amount { get; set; }

    [EntityMember]
    public int? MeasurementId { get; set; }

    [EntityMember]
    public string Measurement { get; set; }

    [EntityMember]
    public string Type { get; set; }

    [EntityMember]
    public int TypeId { get; set; }

    [EntityMember]
    public bool Optional { get; set; }

    public ApiMeasurement MeasurementLookup { get; set; }

    public ApiIngredientType IngredientTypeLookup { get; set; }

  }
}
