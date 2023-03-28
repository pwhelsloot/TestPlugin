using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets.Restrictions
{
  public class DataSetMinimumRestriction : DataSetRestriction
  {
    public decimal Value { get; }

    public DataSetMinimumRestriction(DataSetColumn column, decimal value)
      : base(column)
    {
      Value = value;
    }

    internal override void WriteJsonProperties(JsonWriter writer, JsonSerializer serializer)
    {
      writer.WritePropertyName("Value");
      writer.WriteValue(Value);
    }

    public override void Accept(IDataSetRestrictionVisitor visitor)
    {
      visitor.VisitMinimum(this);
    }
  }
}
