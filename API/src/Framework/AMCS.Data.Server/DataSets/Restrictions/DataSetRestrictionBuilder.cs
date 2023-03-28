using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace AMCS.Data.Server.DataSets.Restrictions
{
  public class DataSetRestrictionBuilder
  {
    private readonly DataSet dataSet;

    public List<DataSetRestriction> Restrictions { get; } = new List<DataSetRestriction>();

    public DataSetRestrictionBuilder(DataSet dataSet)
    {
      this.dataSet = dataSet;
    }

    public DataSetRestrictionBuilder Add(DataSetRestriction restriction)
    {
      Restrictions.Add(restriction);
      return this;
    }

    public DataSetRestrictionBuilder AddDateMaximum(string column, ZonedDateTime value)
    {
      return AddDateMaximum(dataSet.GetColumn(column), value);
    }

    public DataSetRestrictionBuilder AddDateMaximum(DataSetColumn column, ZonedDateTime value)
    {
      return Add(new DataSetDateMaximumRestriction(column, value));
    }

    public DataSetRestrictionBuilder AddDateMinimum(string column, ZonedDateTime value)
    {
      return AddDateMinimum(dataSet.GetColumn(column), value);
    }

    public DataSetRestrictionBuilder AddDateMinimum(DataSetColumn column, ZonedDateTime value)
    {
      return Add(new DataSetDateMinimumRestriction(column, value));
    }

    public DataSetRestrictionBuilder AddDateRange(string column, ZonedDateTime minimum, ZonedDateTime maximum)
    {
      return AddDateRange(dataSet.GetColumn(column), minimum, maximum);
    }

    public DataSetRestrictionBuilder AddDateRange(DataSetColumn column, ZonedDateTime minimum, ZonedDateTime maximum)
    {
      return Add(new DataSetDateRangeRestriction(column, minimum, maximum));
    }

    public DataSetRestrictionBuilder AddDigits(string column, int? precision = null, int? scale = null)
    {
      return AddDigits(dataSet.GetColumn(column), precision, scale);
    }

    public DataSetRestrictionBuilder AddDigits(DataSetColumn column, int? precision = null, int? scale = null)
    {
      return Add(new DataSetDigitsRestriction(column, precision, scale));
    }

    public DataSetRestrictionBuilder AddFuture(string column)
    {
      return AddFuture(dataSet.GetColumn(column));
    }

    public DataSetRestrictionBuilder AddFuture(DataSetColumn column)
    {
      return Add(new DataSetFutureRestriction(column));
    }

    public DataSetRestrictionBuilder AddLength(string column, int? minimum = null, int? maximum = null)
    {
      return AddLength(dataSet.GetColumn(column), minimum, maximum);
    }

    public DataSetRestrictionBuilder AddLength(DataSetColumn column, int? minimum = null, int? maximum = null)
    {
      return Add(new DataSetLengthRestriction(column, minimum, maximum));
    }

    public DataSetRestrictionBuilder AddMaximum(string column, decimal value)
    {
      return AddMaximum(dataSet.GetColumn(column), value);
    }

    public DataSetRestrictionBuilder AddMaximum(DataSetColumn column, decimal value)
    {
      return Add(new DataSetMaximumRestriction(column, value));
    }

    public DataSetRestrictionBuilder AddMinimum(string column, decimal value)
    {
      return AddMinimum(dataSet.GetColumn(column), value);
    }

    public DataSetRestrictionBuilder AddMinimum(DataSetColumn column, decimal value)
    {
      return Add(new DataSetMinimumRestriction(column, value));
    }

    public DataSetRestrictionBuilder AddNotEmpty(string column)
    {
      return AddNotEmpty(dataSet.GetColumn(column));
    }

    public DataSetRestrictionBuilder AddNotEmpty(DataSetColumn column)
    {
      return Add(new DataSetNotEmptyRestriction(column));
    }

    public DataSetRestrictionBuilder AddPast(string column)
    {
      return AddPast(dataSet.GetColumn(column));
    }

    public DataSetRestrictionBuilder AddPast(DataSetColumn column)
    {
      return Add(new DataSetPastRestriction(column));
    }

    public DataSetRestrictionBuilder AddRange(string column, decimal minimum, decimal maximum)
    {
      return AddRange(dataSet.GetColumn(column), minimum, maximum);
    }

    public DataSetRestrictionBuilder AddRange(DataSetColumn column, decimal minimum, decimal maximum)
    {
      return Add(new DataSetRangeRestriction(column, minimum, maximum));
    }

    public DataSetRestrictionBuilder AddList(string column, Func<DataSetNamedValueListBuilder, DataSetNamedValueListBuilder> builder)
    {
      return AddList(dataSet.GetColumn(column), builder);
    }

    public DataSetRestrictionBuilder AddList(DataSetColumn column, Func<DataSetNamedValueListBuilder, DataSetNamedValueListBuilder> builder)
    {
      return AddList(column, builder(new DataSetNamedValueListBuilder()).Build());
    }

    public DataSetRestrictionBuilder AddList(string column, DataSetNamedValueList list)
    {
      return AddList(dataSet.GetColumn(column), list);
    }

    public DataSetRestrictionBuilder AddList(DataSetColumn column, DataSetNamedValueList list)
    {
      return Add(new DataSetListRestriction(column, list));
    }

    public DataSetRestrictionBuilder AddReference(string column, DataSet referenced)
    {
      return AddReference(dataSet.GetColumn(column), referenced);
    }

    public DataSetRestrictionBuilder AddReference(DataSetColumn column, DataSet referenced)
    {
      return Add(new DataSetReferenceRestriction(column, referenced));
    }
  }
}
