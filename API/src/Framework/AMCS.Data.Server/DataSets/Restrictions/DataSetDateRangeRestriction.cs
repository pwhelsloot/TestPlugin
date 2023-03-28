using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Support;
using Newtonsoft.Json;
using NodaTime;

namespace AMCS.Data.Server.DataSets.Restrictions
{
  public class DataSetDateRangeRestriction : DataSetRestriction
  {
    public ZonedDateTime Minimum { get; }
    public ZonedDateTime Maximum { get; }

    public DataSetDateRangeRestriction(DataSetColumn column, ZonedDateTime minimum, ZonedDateTime maximum)
      : base(column)
    {
      Minimum = minimum;
      Maximum = maximum;
    }

    internal override void WriteJsonProperties(JsonWriter writer, JsonSerializer serializer)
    {
      writer.WritePropertyName("Minimum");
      writer.WriteValue(JsonUtil.Print(Minimum));

      writer.WritePropertyName("Maximum");
      writer.WriteValue(JsonUtil.Print(Maximum));
    }

    public override void Accept(IDataSetRestrictionVisitor visitor)
    {
      visitor.VisitDateRange(this);
    }
  }
}
