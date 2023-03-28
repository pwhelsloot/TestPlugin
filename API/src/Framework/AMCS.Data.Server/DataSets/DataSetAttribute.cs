using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets
{
  [AttributeUsage(AttributeTargets.Class)]
  public class DataSetAttribute : Attribute
  {
    public string Name { get; }

    public object Label { get; }

    public Type EntityType { get; set; }

    public DataSetAttribute(string name, object label)
    {
      Name = name;
      Label = label;
    }
  }
}
