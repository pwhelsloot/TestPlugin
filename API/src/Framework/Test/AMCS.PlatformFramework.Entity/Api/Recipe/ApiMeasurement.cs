using AMCS.Data.Entity;
using System;
using System.Runtime.Serialization;

namespace AMCS.PlatformFramework.Entity.Api.Recipe
{
  public enum Measurement
  {
    CUP = 1,
    TEASPOON = 2,
    TABLESPOON = 3,
    POUND = 4,
    EACH = 5,
    LITER = 6,
    GRAMS = 7,
    HANDFULL = 8
  }

  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class ApiMeasurement : ApiSearchObject
  {
    [EntityMember]
    public int ApiMeasurementId { get; set; }

    [EntityMember]
    public string Description { get; set; }
  }
}
