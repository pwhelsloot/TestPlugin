using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.DataSets.FilterExpressions;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets
{
  [JsonConverter(typeof(DataSetQueryConverter))]
  public class DataSetQuery
  {
    public DataSet DataSet { get; }

    public IList<DataSetColumn> Columns { get; }

    public IList<DataSetFilterExpression> Expressions { get; }

    public DataSetQuery(DataSet dataSet, IList<DataSetColumn> columns, params DataSetFilterExpression[] expressions)
      : this(dataSet, columns, (IList<DataSetFilterExpression>)expressions)
    {
    }

    public DataSetQuery(DataSet dataSet, IList<DataSetColumn> columns, IList<DataSetFilterExpression> expressions)
    {
      DataSet = dataSet;
      Columns = columns;
      Expressions = expressions;
    }
  }
}
