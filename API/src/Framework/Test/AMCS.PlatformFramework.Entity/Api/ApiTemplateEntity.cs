using AMCS.Data.Entity;
using System;
using System.Runtime.Serialization;

namespace AMCS.PlatformFramework.Entity.Api
{
  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class ApiTemplateEntity : ApiSearchObject
  {
    [EntityMember]
    public string TemplateName { get; set; }

    [EntityMember]
    public DateTime? TemplateDate { get; set; }
  }
}