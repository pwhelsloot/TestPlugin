using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets
{
  [JsonConverter(typeof(DataSetValueListConverter))]
  public class DataSetValueList
  {
    private static readonly DataSetValue[] EmptyItems = new DataSetValue[0];

    public IList<DataSetValue> Items { get; }

    public DataSetValueList(IEnumerable<DataSetValue> items)
    {
      if (items == null)
        Items = EmptyItems;
      else
        Items = new ReadOnlyCollection<DataSetValue>(items.ToArray());
    }
  }
}