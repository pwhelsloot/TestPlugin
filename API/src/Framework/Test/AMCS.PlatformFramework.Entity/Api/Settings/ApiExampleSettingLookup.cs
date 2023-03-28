using System;
using System.Runtime.Serialization;
using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.Entity.Api.Settings
{
  /** 
   * Data Model for the Angular Form Editor 
   */
  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class ApiExampleSettingLookup : ApiSearchObject
  {
    [EntityMember]
    public int? ExampleSettingLookupId { get; set; }

    [EntityMember]
    public string Description { get; set; }
  }
}
