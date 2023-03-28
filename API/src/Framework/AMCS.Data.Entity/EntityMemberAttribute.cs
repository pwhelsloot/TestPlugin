using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  [AttributeUsage(AttributeTargets.Property)]
  public class EntityMemberAttribute : Attribute
  {
    public string Name { get; }

    public bool IsDynamic { get; set; }

    public bool IsOverridable { get; set; }

    public DateStorage DateStorage { get; set; }

    public string TimeZoneMember { get; set; }

    public EntityMemberAttribute()
    {
    }

    public EntityMemberAttribute(string name)
    {
      Name = name;
    }
  }
}
