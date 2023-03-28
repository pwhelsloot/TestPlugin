using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.JobSystem;
using AMCS.JobSystem.Scheduler.Api;

namespace AMCS.TestPlugin.Server
{
  public static class Jobs
  {
    public const string PriorityQueue = "priority";
    public const string SlowQueue = "slow";

    public static SchedulerClient Client => DataServices.Resolve<SchedulerClient>();

    /// <summary>
    /// Post a new job on the job system.
    /// </summary>
    /// <param name="userId">User ID sent to the job handler.</param>
    /// <param name="request">Description of the job.</param>
    /// <param name="update">Callback called when a job update is received.</param>
    /// <returns>The ID of the posted job.</returns>
    public static Guid Post(ISessionToken userId, IJobRequest request, Action<IJobUpdate> update = null)
    {
      return Client.Post(JobHandler.GetJobUserId(userId), request, update);
    }

    /// <summary>
    /// Post a new job on the job system.
    /// </summary>
    /// <param name="userId">User ID sent to the job handler.</param>
    /// <param name="queue">Queue on which to post the job.</param>
    /// <param name="handler">Type of the job handler.</param>
    /// <param name="parameters">Parameters sent to the job handler.</param>
    /// <param name="friendlyName">Friendly name of the job used for logging.</param>
    /// <param name="deadline">Deadline within which the job must start execution.</param>
    /// <param name="persistenceMode">Whether the job is transient or persisted.</param>
    /// <param name="duplicateMode">Duplicate mode of the job.</param>
    /// <param name="update">Callback called when a job update is received.</param>
    /// <returns>The ID of the posted job.</returns>
    public static Guid Post(ISessionToken userId, string queue, Type handler, object parameters, string friendlyName = null, TimeSpan? deadline = null, PersistenceMode persistenceMode = PersistenceMode.Transient, DuplicateMode? duplicateMode = null, Action<IJobUpdate> update = null)
    {
      var builder = new JobBuilder()
        .Queue(queue)
        .Handler(handler)
        .Parameters(parameters)
        .PersistenceMode(persistenceMode);

      if (friendlyName != null)
        builder.FriendlyName(friendlyName);
      if (deadline.HasValue)
        builder.Deadline(deadline.Value);
      if (duplicateMode.HasValue)
        builder.DuplicateMode(duplicateMode.Value);

      return Post(userId, builder.Build(), update);
    }

    /// <summary>
    /// Post a new job on the job system on the priority queue.
    /// </summary>
    /// <param name="userId">User ID sent to the job handler.</param>
    /// <param name="handler">Type of the job handler.</param>
    /// <param name="parameters">Parameters sent to the job handler.</param>
    /// <param name="friendlyName">Friendly name of the job used for logging.</param>
    /// <param name="deadline">Deadline within which the job must start execution.</param>
    /// <param name="persistenceMode">Whether the job is transient or persisted.</param>
    /// <param name="duplicateMode">Duplicate mode of the job.</param>
    /// <param name="update">Callback called when a job update is received.</param>
    /// <returns>The ID of the posted job.</returns>
    public static Guid PostPriority(ISessionToken userId, Type handler, object parameters, string friendlyName = null, TimeSpan? deadline = null, PersistenceMode persistenceMode = PersistenceMode.Transient, DuplicateMode? duplicateMode = null, Action<IJobUpdate> update = null)
    {
      return Post(userId, PriorityQueue, handler, parameters, friendlyName, deadline, persistenceMode, duplicateMode, update);
    }

    /// <summary>
    /// Post a new job on the job system on the slow queue.
    /// </summary>
    /// <param name="userId">User ID sent to the job handler.</param>
    /// <param name="handler">Type of the job handler.</param>
    /// <param name="parameters">Parameters sent to the job handler.</param>
    /// <param name="friendlyName">Friendly name of the job used for logging.</param>
    /// <param name="deadline">Deadline within which the job must start execution.</param>
    /// <param name="persistenceMode">Whether the job is transient or persisted.</param>
    /// <param name="duplicateMode">Duplicate mode of the job.</param>
    /// <param name="update">Callback called when a job update is received.</param>
    /// <returns>The ID of the posted job.</returns>
    public static Guid PostSlow(ISessionToken userId, Type handler, object parameters, string friendlyName = null, TimeSpan? deadline = null, PersistenceMode persistenceMode = PersistenceMode.Persist, DuplicateMode? duplicateMode = null, Action<IJobUpdate> update = null)
    {
      return Post(userId, SlowQueue, handler, parameters, friendlyName, deadline, persistenceMode, duplicateMode, update);
    }
  }
}
