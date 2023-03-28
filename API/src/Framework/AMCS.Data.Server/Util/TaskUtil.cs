using System;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Util
{
  public static class TaskUtil
  {
    /// <summary>
    /// Start a fire and forget task.
    /// </summary>
    public static void RunBackground(Func<Task> action)
    {
      Task.Run(action).ConfigureAwait(false);
    }
  }
}
