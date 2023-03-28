using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets
{
  public class DataSetNamedValueListBuilder
  {
    public List<DataSetNamedValue> Items { get; } = new List<DataSetNamedValue>();

    public DataSetNamedValueListBuilder Add(DataSetNamedValue item)
    {
      Items.Add(item);
      return this;
    }

    public DataSetNamedValueListBuilder Add(object name, object value)
    {
      return Add(new DataSetNamedValue(
        DataSetUtil.GetLocalizedString(GetType(), name),
        DataSetValue.Create(value)));
    }

    public DataSetNamedValueListBuilder AddFromEnum<T>()
    {
      return AddFromEnum(typeof(T));
    }

    public DataSetNamedValueListBuilder AddFromEnum(Type enumType)
    {
      foreach (var value in Enum.GetValues(enumType))
      {
        Add(DataSetUtil.GetLocalizedString(GetType(), value), (int)value);
      }

      return this;
    }

    public DataSetNamedValueList Build()
    {
      return new DataSetNamedValueList(Items);
    }
  }
}
