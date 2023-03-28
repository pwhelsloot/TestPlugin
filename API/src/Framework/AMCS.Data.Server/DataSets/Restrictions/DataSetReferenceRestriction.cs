using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets.Restrictions
{
  public class DataSetReferenceRestriction : DataSetRestriction
  {
    public DataSet Referenced { get; }

    public DataSetReferenceRestriction(DataSetColumn column, DataSet referenced)
      : base(column)
    {
      Referenced = referenced;
    }

    internal override void WriteJsonProperties(JsonWriter writer, JsonSerializer serializer)
    {
      writer.WritePropertyName("Referenced");
      writer.WriteValue(Referenced.Name);
    }

    public override void Accept(IDataSetRestrictionVisitor visitor)
    {
      visitor.VisitReference(this);
    }
  }
}
