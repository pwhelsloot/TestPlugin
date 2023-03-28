using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Broadcast
{
  public class BroadcastRemoteTransportFactoryConfiguration
  {
    /// <summary>
    /// Gets or sets the Azure Service Bus connection string.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the instance name.
    /// </summary>
    /// <remarks>
    /// This defaults to <see cref="Dns.GetHostName()"/>
    /// </remarks>
    public string InstanceName { get; set; }

    /// <summary>
    /// Gets or sets the optional prefix to message queues.
    /// </summary>
    public string QueuePrefix { get; set; }

    /// <summary>
    /// Gets or sets the auto delete on idle value for auto deleting
    /// topic subscriptions.
    /// </summary>
    public TimeSpan? AutoDeleteOnIdle { get; set; }

    /// <summary>
    /// Gets or sets the maximum delivery count
    /// </summary>
    public int? MaxDeliveryCount { get; set; }
  }
}
