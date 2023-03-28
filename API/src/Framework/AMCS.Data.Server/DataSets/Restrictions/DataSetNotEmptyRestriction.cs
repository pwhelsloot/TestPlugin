using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Restrictions
{
  public class DataSetNotEmptyRestriction : DataSetRestriction
  {
    public DataSetNotEmptyRestriction(DataSetColumn column)
      : base(column)
    {
    }

    public override void Accept(IDataSetRestrictionVisitor visitor)
    {
      visitor.VisitNotEmpty(this);
    }
  }
}
