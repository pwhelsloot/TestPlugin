namespace AMCS.Data.Server.Broadcast
{
  using System;
  using AMCS.Data.Configuration;
  using AMCS.Data.Server.Azure.Helpers;
  using AMCS.Data.Server.Configuration;
  using Autofac;

  public static class DataConfigurationExtensions
  {
    /// <summary>
    /// Sets up the broadcast service with default heartbeat timer/latency of 2 minutes.
    /// </summary>
    /// <param name="self">The data configuration.</param>
    /// <param name="configuration">The broadcast service configuration i.e. instance/queue prefix settings.</param>
    /// <param name="messageQueueConnectionString">The optional message bus connection string.</param>
    public static void ConfigureBroadcastService(
      this DataConfiguration self,
      IBroadcastConfiguration configuration,
      IConnectionString messageQueueConnectionString)
    {
      self.ConfigureBroadcastService(configuration, messageQueueConnectionString, TimeSpan.FromMinutes(2),
        TimeSpan.FromMinutes(2));
    }

    /// <summary>
    /// Sets up the broadcast service with all settings manually configured.
    /// </summary>
    /// <param name="self">The data configuration.</param>
    /// <param name="configuration">The broadcast service configuration i.e. instance/queue prefix settings.</param>
    /// <param name="messageQueueConnectionString">The optional message bus connection string.</param>
    /// <param name="heartbeatTimerTimeout">The interval on which the heartbeat timer will run.</param>
    /// <param name="maxHeartbeatLatency">The maximum latency threshold for sending/receiving a broadcasted message.</param>
    public static void ConfigureBroadcastService(
      this DataConfiguration self,
      IBroadcastConfiguration configuration,
      IConnectionString messageQueueConnectionString,
      TimeSpan heartbeatTimerTimeout,
      TimeSpan maxHeartbeatLatency)
    {
      IBroadcastTransportFactory broadcastTransportFactory;

      if (messageQueueConnectionString != null)
      {
        broadcastTransportFactory = new BroadcastRemoteTransportFactory(new BroadcastRemoteTransportFactoryConfiguration
        {
          ConnectionString = messageQueueConnectionString.GetConnectionString(),
          AutoDeleteOnIdle = TimeSpan.FromHours(1),
          InstanceName = AzureHelpers.GenerateInstanceName(),
          QueuePrefix = configuration.QueuePrefix
        });
      }
      else
      {
        broadcastTransportFactory = new BroadcastLocalTransportFactory();
      }

      self.ContainerBuilder
        .Register(context => new BroadcastService(
          broadcastTransportFactory,
          context.Resolve<ISetupService>()))
        .SingleInstance()
        .AutoActivate()
        .As<IBroadcastService>();

      self.ContainerBuilder
        .Register(context => new BroadcastHeartbeatService(
          context.Resolve<IBroadcastService>(),
          heartbeatTimerTimeout,
          maxHeartbeatLatency))
        .SingleInstance()
        .AutoActivate()
        .AsSelf()
        .As<IBroadcastHeartbeatService>();
    }
  }
}
