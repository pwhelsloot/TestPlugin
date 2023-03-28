using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AMCS.Data.Server.DataSets
{
  public class DataSetImport
  {
    [JsonConverter(typeof(StringEnumConverter))]
    public DataSetImportMode Mode { get; }

    public DataSetTableSet TableSet { get; }

    public DataSetImport(DataSetImportMode mode)
      : this(mode, null)
    {
    }

    [JsonConstructor]
    public DataSetImport(DataSetImportMode mode, DataSetTableSet tableSet)
    {
      Mode = mode;
      TableSet = tableSet ?? new DataSetTableSet();
    }
  }
}
