using System;
using System.Runtime.Serialization;
using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.Entity.Api.Settings
{
  /** 
   * Model for the Angular Browser
   */
  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class ApiExampleSettingBrowser : ApiSearchObject
  {
    [EntityMember]
    public int? ExampleSettingId { get; set; }

    [EntityMember]
    public string Description { get; set; }

    [EntityMember]
    public string Dropdown { get; set; }
  }
}
