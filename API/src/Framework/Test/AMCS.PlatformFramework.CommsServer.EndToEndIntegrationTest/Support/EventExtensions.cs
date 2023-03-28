using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;

namespace AMCS.PlatformFramework.CommsServer.EndToEndIntegrationTest.Support
{
  internal static class EventExtensions
  {
    public static void WaitForCompletion(this ManualResetEventSlim self, TimeSpan timeout)
    {
      WaitForCompletion(self, timeout, false);
    }

    public static void WaitForCompletion(this ManualResetEventSlim self, TimeSpan timeout, bool throwOnTimeout)
    {
      if (Debugger.IsAttached)
        self.Wait();
      else
      {
        if (throwOnTimeout)
        {
          if (!self.Wait(timeout))
            throw new TimeoutException("Timed out waiting for event to complete");
        }
        else
        {
          Assert.IsTrue(self.Wait(timeout));
        }
      }
    }
  }
}