using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets.Restrictions
{
  public class DataSetRangeRestriction : DataSetRestriction
  {
    public decimal Minimum { get; }
    public decimal Maximum { get; }

    public DataSetRangeRestriction(DataSetColumn column, decimal minimum, decimal maximum)
      : base(column)
    {
      Minimum = minimum;
      Maximum = maximum;
    }

    internal override void WriteJsonProperties(JsonWriter writer, JsonSerializer serializer)
    {
      writer.WritePropertyName("Minimum");
      writer.WriteValue(Minimum);

      writer.WritePropertyName("Maximum");
      writer.WriteValue(Maximum);
    }

    public override void Accept(IDataSetRestrictionVisitor visitor)
    {
      visitor.VisitRange(this);
    }
  }
}
