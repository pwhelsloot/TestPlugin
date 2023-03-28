using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  [AttributeUsage(AttributeTargets.Property)]
  public class EntityParentAttribute : Attribute
  {
    public string ForeignKeyColumn { get; }

    public EntityParentAttribute(string foreignKeyColumn)
    {
      ForeignKeyColumn = foreignKeyColumn;
    }
  }
}
