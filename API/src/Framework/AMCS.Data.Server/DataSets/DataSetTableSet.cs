using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets
{
  [JsonConverter(typeof(DataSetTableSetConverter))]
  public class DataSetTableSet
  {
    public IList<DataSetTable> Tables { get; } = new List<DataSetTable>();
  }
}
