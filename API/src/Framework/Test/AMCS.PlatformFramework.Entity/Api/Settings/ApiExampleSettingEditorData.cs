using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.Entity.Api.Settings
{
  /** 
   * Editor Data Model for the Angular Form Editor (contains all Lookups + Data Model)
   */
  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class ApiExampleSettingEditorData : ApiSearchObject
  {
    [EntityMember]
    public int? ExampleSettingId { get; set; }

    [EntityMember]
    public ApiExampleSetting DataModel { get; set; }

    [EntityMember]
    public IList<ApiExampleSettingLookup> LookupItems { get; set; }
  }
}