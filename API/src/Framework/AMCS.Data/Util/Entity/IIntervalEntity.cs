namespace AMCS.Data.Util.Entity
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public interface IIntervalEntity
  {
    int? GetId();
    int? NoOfDays { get; }
    int? NoOfMonths { get; }
    bool IsDaily { get; }
    bool IsMonthly { get; }
    bool IsWeekly { get; }
    string Description { get; }
  }
}
