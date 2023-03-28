using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Restrictions
{
  public interface IDataSetRestrictionVisitor
  {
    void VisitDateMaximum(DataSetDateMaximumRestriction restriction);

    void VisitDateMinimum(DataSetDateMinimumRestriction restriction);

    void VisitDateRange(DataSetDateRangeRestriction restriction);

    void VisitDigits(DataSetDigitsRestriction restriction);

    void VisitFuture(DataSetFutureRestriction restriction);

    void VisitLength(DataSetLengthRestriction restriction);

    void VisitMaximum(DataSetMaximumRestriction restriction);

    void VisitMinimum(DataSetMinimumRestriction restriction);

    void VisitNotEmpty(DataSetNotEmptyRestriction restriction);

    void VisitPast(DataSetPastRestriction restriction);

    void VisitRange(DataSetRangeRestriction restriction);

    void VisitList(DataSetListRestriction restriction);

    void VisitReference(DataSetReferenceRestriction restriction);
  }
}
