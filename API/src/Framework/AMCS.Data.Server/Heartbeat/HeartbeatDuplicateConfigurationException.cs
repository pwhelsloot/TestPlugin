namespace AMCS.Data.Server.Heartbeat
{
  using System;
  using System.Runtime.Serialization;

  [Serializable]
  public class HeartbeatDuplicateConfigurationException : Exception
  {
    public HeartbeatDuplicateConfigurationException()
    {
    }

    public HeartbeatDuplicateConfigurationException(string message) : base(message)
    {
    }

    public HeartbeatDuplicateConfigurationException(string message, Exception inner) : base(message, inner)
    {
    }

    protected HeartbeatDuplicateConfigurationException(
      SerializationInfo info,
      StreamingContext context) : base(info, context)
    {
    }
  }
}