using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.DataSets.Import;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AMCS.Data.Server.DataSets
{
  public class DataSetImportResult
  {
    [JsonConverter(typeof(StringEnumConverter))]
    public DataSetImportMode Mode { get; set; }

    public DataSetTableSet ImportedSet { get; } = new DataSetTableSet();

    public DataSetTableSet FailedSet { get; } = new DataSetTableSet();

    public MessageCollection Messages { get; } = new MessageCollection();
  }
}
