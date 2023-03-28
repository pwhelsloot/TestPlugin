using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace AMCS.Data.Support
{
  public static class TaskUtils
  {
    public static T RunSynchronously<T>(Func<Task<T>> callback)
    {
      try
      {
        return Task.Run(callback).Result;
      }
      catch (AggregateException ex)
      {
        ThrowException(ex);
        throw; // This would never be invoked as long as Throw() is called above.
      }
    }

    public static void RunSynchronously(Func<Task> callback)
    {
      try
      {
        Task.Run(callback).Wait();
      }
      catch (AggregateException ex)
      {
        ThrowException(ex);
        throw; // This would never be invoked as long as Throw() is called above.
      }
    }

    // See https://stackoverflow.com/questions/5613951 for more information.
    public static void RunBackground(Func<Task> action)
    {
      try
      {
        Task.Run(action).ConfigureAwait(false);
      }
      catch (AggregateException ex)
      {
        ThrowException(ex);
        throw; // This would never be invoked as long as Throw() is called above.
      }
    }

    private static void ThrowException(AggregateException ex)
    {
      var exception = ex.Flatten().InnerExceptions.FirstOrDefault() ?? ex;
      ExceptionDispatchInfo.Capture(exception).Throw();
    }
  }
}
