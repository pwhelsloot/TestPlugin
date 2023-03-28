using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.DataSets.FilterExpressions;

namespace AMCS.Data.Server.DataSets.Filters
{
  public class DataSetSimpleFilter : DataSetFilter
  {
    public DataSetSimpleFilter(DataSetColumn column)
      : base(column)
    {
    }

    public override bool IsMatch(DataSetFilterExpression expression)
    {
      return true;
    }
  }
}
