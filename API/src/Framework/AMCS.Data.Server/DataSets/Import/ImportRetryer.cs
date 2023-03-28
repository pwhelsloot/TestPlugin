using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import
{
  internal class ImportRetryer
  {
    public static ImportRetryer CreateDefault()
    {
#if DEBUG
      return new ImportRetryer(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
#else
      return new ImportRetryer(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
#endif
    }

    private readonly int maxTries;
    private readonly TimeSpan initialTime;
    private readonly TimeSpan maxTime;

    public ImportRetryer(int maxTries, TimeSpan initialTime, TimeSpan maxTime)
    {
      this.maxTries = maxTries;
      this.initialTime = initialTime;
      this.maxTime = maxTime;
    }

    public void Retry(Func<bool, bool> action)
    {
      var delay = initialTime;

      for (int i = 0; i < maxTries; i++)
      {
        bool last = i == maxTries - 1;

        if (action(last))
          return;

        // Sleep for the delay.

        Thread.Sleep(delay);

        // Double the delay until maxTime is reached.

        delay = new TimeSpan(Math.Min(delay.Ticks * 2, maxTime.Ticks));
      }
    }
  }
}
