using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  [AttributeUsage(AttributeTargets.Property)]
  public class EntityChildAttribute : Attribute
  {
    public string ForeignKeyColumn { get; }
    public bool Sparse { get; }

    /// <summary>
    /// Identify this property as being a collection of children to this object
    /// </summary>
    /// <param name="foreignKeyColumn">The name of the column on the child object that has the foreign key to this object</param>
    /// <param name="sparse">Set to true if this object is expected to have only very few child entities. This tells the framework it can join on this table without fearing cartesian explosion.</param>
    public EntityChildAttribute(string foreignKeyColumn, bool sparse = false)
    {
      ForeignKeyColumn = foreignKeyColumn;
      Sparse = sparse;
    }
  }
}
