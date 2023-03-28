namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration
{
  using AzureServiceBusSupport;

  public class AzureAppConfigurationQueueNameBuilder
  {
    public string AzureAppConfigurationSubscriptionName { get; }
    public string AzureAppConfigurationTopicName { get; }

    public AzureAppConfigurationQueueNameBuilder(string instanceName, string topicName)
    {
      AzureAppConfigurationSubscriptionName = MessageQueueUtils.CleanSubscriptionName(MessageQueueUtils.ParseInstanceName(instanceName));
      AzureAppConfigurationTopicName = topicName;
    }
  }
}