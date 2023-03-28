using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Support;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets
{
  [JsonConverter(typeof(DataSetNamedValueListConverter))]
  public class DataSetNamedValueList
  {
    private static readonly DataSetNamedValue[] EmptyItems = new DataSetNamedValue[0];

    public IList<DataSetNamedValue> Items { get; }

    public DataSetNamedValueList(IEnumerable<DataSetNamedValue> items)
    {
      if (items == null)
        Items = EmptyItems;
      else
        Items = new ReadOnlyCollection<DataSetNamedValue>(items.ToArray());
    }

    public DataSetNamedValue Find(object value)
    {
      foreach (var item in Items)
      {
        var itemValue = item.Value.Value;

        if (value == null && itemValue == null)
          return item;

        if (value != null && itemValue != null)
        {
          if (
            ValueCoercion.TryCoerce(value, itemValue.GetType(), out var coercedValue) &&
            Equals(itemValue, coercedValue)
          )
            return item;
        }
      }

      return null;
    }
  }
}
