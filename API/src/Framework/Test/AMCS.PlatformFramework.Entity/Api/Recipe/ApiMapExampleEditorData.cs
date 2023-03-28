using AMCS.Data.Entity;
using System;
using System.Runtime.Serialization;

namespace AMCS.PlatformFramework.Entity.Api.Recipe
{
  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class ApiMapExampleEditorData : ApiSearchObject
  {
    [EntityMember]
    public int? MapExampleId { get; set; }

    [EntityMember]
    public ApiMapExample MapExample { get; set; }
  }
}
