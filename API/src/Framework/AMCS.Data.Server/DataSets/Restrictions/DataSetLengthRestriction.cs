using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets.Restrictions
{
  public class DataSetLengthRestriction : DataSetRestriction
  {
    public int? Minimum { get; }
    public int? Maximum { get; }

    public DataSetLengthRestriction(DataSetColumn column, int? minimum, int? maximum)
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
      visitor.VisitLength(this);
    }
  }
}
