using AMCS.Data.Entity;
using System;
using System.Runtime.Serialization;

namespace AMCS.PlatformFramework.Entity.Api.Settings
{
  /** 
   * Data Model for the Angular Form Editor 
   */
  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class ApiExampleSetting : ApiSearchObject
  {
    [EntityMember]
    public int? ExampleSettingId { get; set; }

    [EntityMember]
    public string Description { get; set; }

    [EntityMember]
    public int? DropdownId { get; set; }
  }
}
