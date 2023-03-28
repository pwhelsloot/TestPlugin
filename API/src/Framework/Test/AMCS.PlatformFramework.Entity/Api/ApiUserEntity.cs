using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.Entity.Api
{
  [EntityTable("User", "UserId")]
  [ApiExplorer(Mode = ApiMode.External, Version = "external")]
  public class ApiUserEntity : EntityObject
  {
    [EntityMember]
    public int? UserId { get; set; }

    [EntityMember]
    public string User { get; set; }

    [EntityMember]
    public string Password { get; set; }

    public override int? GetId() => UserId;
  }
}
