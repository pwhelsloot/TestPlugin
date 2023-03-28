using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets.Restrictions
{
  public class DataSetListRestriction : DataSetRestriction
  {
    public DataSetNamedValueList List { get; }

    public DataSetListRestriction(DataSetColumn column, DataSetNamedValueList list)
      : base(column)
    {
      List = list;
    }

    internal override void WriteJsonProperties(JsonWriter writer, JsonSerializer serializer)
    {
      writer.WritePropertyName("List");
      serializer.Serialize(writer, List);
    }

    public override void Accept(IDataSetRestrictionVisitor visitor)
    {
      visitor.VisitList(this);
    }
  }
}
