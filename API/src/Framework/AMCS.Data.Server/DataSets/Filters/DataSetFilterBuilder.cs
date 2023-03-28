using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Filters
{
  public class DataSetFilterBuilder
  {
    private readonly DataSet dataSet;

    public List<DataSetFilter> Filters { get; } = new List<DataSetFilter>();

    public DataSetFilterBuilder(DataSet dataSet)
    {
      this.dataSet = dataSet;
    }

    public DataSetFilterBuilder Add(DataSetFilter filter)
    {
      Filters.Add(filter);
      return this;
    }

    public DataSetFilterBuilder AddList(string column, Func<DataSetNamedValueListBuilder, DataSetNamedValueListBuilder> builder)
    {
      return AddList(dataSet.GetColumn(column), builder);
    }

    public DataSetFilterBuilder AddList(DataSetColumn column, Func<DataSetNamedValueListBuilder, DataSetNamedValueListBuilder> builder)
    {
      return AddList(column, builder(new DataSetNamedValueListBuilder()).Build());
    }

    public DataSetFilterBuilder AddList(string column, DataSetNamedValueList items)
    {
      return AddList(dataSet.GetColumn(column), items);
    }

    public DataSetFilterBuilder AddList(DataSetColumn column, DataSetNamedValueList items)
    {
      return Add(new DataSetListFilter(column, items));
    }

    public DataSetFilterBuilder AddSimple(string column)
    {
      return AddSimple(dataSet.GetColumn(column));
    }

    public DataSetFilterBuilder AddSimple(DataSetColumn column)
    {
      return Add(new DataSetSimpleFilter(column));
    }
  }
}
