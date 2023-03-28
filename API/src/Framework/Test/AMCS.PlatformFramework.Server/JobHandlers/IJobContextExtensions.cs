using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.JobSystem;

namespace AMCS.PlatformFramework.Server.JobHandlers
{
  internal static class IJobContextExtensions
  {
    public static void SetProgress(this IJobContext self, int current, int total, string status)
    {
      self.SetProgress((double)current / total, status);
    }
  }
}
