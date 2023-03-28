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
  public class DataSetDateMaximumRestriction : DataSetRestriction
  {
    public ZonedDateTime Value { get; }

    public DataSetDateMaximumRestriction(DataSetColumn column, ZonedDateTime value)
      : base(column)
    {
      Value = value;
    }

    internal override void WriteJsonProperties(JsonWriter writer, JsonSerializer serializer)
    {
      writer.WritePropertyName("Value");
      writer.WriteValue(JsonUtil.Print(Value));
    }

    public override void Accept(IDataSetRestrictionVisitor visitor)
    {
      visitor.VisitDateMaximum(this);
    }
  }
}
