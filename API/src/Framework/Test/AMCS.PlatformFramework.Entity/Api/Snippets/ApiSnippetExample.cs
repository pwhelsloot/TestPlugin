using AMCS.Data.Entity;
using System;
using System.Runtime.Serialization;

namespace AMCS.PlatformFramework.Entity.Api.Snippets
{
  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class ApiSnippetExample : ApiSearchObject
  {
    [EntityMember]
    public int? SnippetExampleId { get; set; }
  }
}
