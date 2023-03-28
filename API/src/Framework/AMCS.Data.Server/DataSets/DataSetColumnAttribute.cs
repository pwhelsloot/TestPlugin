using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets
{
  [AttributeUsage(AttributeTargets.Property)]
  public class DataSetColumnAttribute : Attribute
  {
    public object Label { get; }

    public bool IsKeyColumn { get; set; }

    public bool IsDisplayColumn { get; set; }

    public bool IsReadOnly { get; set; }

    public bool IsDefault { get; set; }

    public bool IsMandatory { get; set; }

    public DataSetColumnAttribute(object label)
    {
      Label = label;
    }
  }
}
