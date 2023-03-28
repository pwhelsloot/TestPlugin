using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Restrictions
{
  public class DataSetPastRestriction : DataSetRestriction
  {
    public DataSetPastRestriction(DataSetColumn column)
      : base(column)
    {
    }

    public override void Accept(IDataSetRestrictionVisitor visitor)
    {
      visitor.VisitPast(this);
    }
  }
}
