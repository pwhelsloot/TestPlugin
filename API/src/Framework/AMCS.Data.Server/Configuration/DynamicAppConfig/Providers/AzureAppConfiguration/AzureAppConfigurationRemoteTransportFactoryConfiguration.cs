namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration
{
  using System;
  using System.Net;

  public class AzureAppConfigurationRemoteTransportFactoryConfiguration
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
    /// Gets or sets the topic name.
    /// </summary>
    public string TopicName { get; set; }

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