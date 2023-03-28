using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets
{
  [JsonConverter(typeof(DataSetNamedValueConverter))]
  public class DataSetNamedValue
  {
    public string Name { get; }

    public DataSetValue Value { get; }

    public DataSetNamedValue(string name, DataSetValue value)
    {
      Name = name;
      Value = value;
    }
  }
}
