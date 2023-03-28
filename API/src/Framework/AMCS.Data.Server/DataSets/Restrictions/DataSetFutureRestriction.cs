using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Restrictions
{
  public class DataSetFutureRestriction : DataSetRestriction
  {
    public DataSetFutureRestriction(DataSetColumn column)
      : base(column)
    {
    }

    public override void Accept(IDataSetRestrictionVisitor visitor)
    {
      visitor.VisitFuture(this);
    }
  }
}
