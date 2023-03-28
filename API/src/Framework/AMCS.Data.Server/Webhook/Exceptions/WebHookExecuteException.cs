namespace AMCS.Data.Server.Webhook.Exceptions
{
  using System;
  using System.Runtime.Serialization;

  [Serializable]
  public class WebHookExecuteException : Exception
  {
    public WebHookExecuteException()
    {
    }

    public WebHookExecuteException(string message) : base(message)
    {
    }

    public WebHookExecuteException(string message, Exception inner) : base(message, inner)
    {
    }

    protected WebHookExecuteException(
      SerializationInfo info,
      StreamingContext context) : base(info, context)
    {
    }
  }
}