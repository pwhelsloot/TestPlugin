using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.AzureServiceBusSupport;

namespace AMCS.Data.Server.AzureServiceBus
{
  internal class MessageQueueNameBuilder
  {
    private readonly string prefix;
    private readonly string instanceName;

    public MessageQueueNameBuilder(string instanceName, string queuePrefix)
    {
      if (!String.IsNullOrEmpty(queuePrefix))
        this.prefix = queuePrefix.Trim('-') + "-";

      this.instanceName = MessageQueueUtils.ParseInstanceName(instanceName);
    }

    public string GetBroadcastTopicName()
    {
      return MessageQueueUtils.CleanQueueOrTopicName(prefix + "broadcast");
    }

    public string GetBroadcastSubscriptionName()
    {
      return MessageQueueUtils.CleanSubscriptionName(instanceName);
    }
  }
}
