namespace AMCS.PlatformFramework.CommsServer.EndToEndIntegrationTest.Support
{
  using Azure.Messaging.ServiceBus.Administration;
  using Data.Support;

  internal static class CommsServerQueueUtils
  {
    internal static void CleanUpAzureServiceBus(string connectionString, string queuePrefix)
    {
      TaskUtils.RunSynchronously(async () =>
      {
        var managementClient = new ServiceBusAdministrationClient(connectionString);
        await foreach (var queue in managementClient.GetQueuesAsync())
        {
          if (queue.Name.StartsWith(queuePrefix))
            await managementClient.DeleteQueueAsync(queue.Name);
        }

        await foreach (var topic in managementClient.GetTopicsAsync())
        {
          if (topic.Name.StartsWith(queuePrefix))
            await managementClient.DeleteTopicAsync(topic.Name);
        }
      });
    }
  }
}
