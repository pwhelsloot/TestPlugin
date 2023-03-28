using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets.Restrictions
{
  public class DataSetDigitsRestriction : DataSetRestriction
  {
    public int? Precision { get; }
    public int? Scale { get; }

    public DataSetDigitsRestriction(DataSetColumn column, int? precision, int? scale)
      : base(column)
    {
      Precision = precision;
      Scale = scale;
    }

    internal override void WriteJsonProperties(JsonWriter writer, JsonSerializer serializer)
    {
      writer.WritePropertyName("Precision");
      writer.WriteValue(Precision);

      writer.WritePropertyName("Scale");
      writer.WriteValue(Scale);
    }

    public override void Accept(IDataSetRestrictionVisitor visitor)
    {
      visitor.VisitDigits(this);
    }
  }
}
