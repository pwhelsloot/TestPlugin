using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.AzureServiceBusSupport;
using Azure.Messaging.ServiceBus;

namespace AMCS.Data.Server.AzureServiceBus
{
  internal static class ServiceBusConstants
  {
    // Queue configuration.

    public const bool EnablePartitioning = false;
    public const long QueueSizeMb = 1024;
    public static readonly TimeSpan DefaultMessageTimeToLive = TimeSpan.MaxValue;

    // Sending and receiving configuration.

    public static readonly ServiceBusRetryOptions RetryOptions =
      new ServiceBusRetryOptions { CustomRetryPolicy = new RetryExponentialIndefinitely(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30)) };
    public const int MaxOutstandingAcks = 100;
#if DEBUG
    public static TimeSpan LockDuration = TimeSpan.FromSeconds(30);
#else
    public static TimeSpan LockDuration = TimeSpan.FromMinutes(4);
#endif
    public const int MaxDeliveryCount = 3;
  }
}
