namespace AMCS.Data.Server.Webhook.Exceptions
{
  using System;
  using System.Runtime.Serialization;

  [Serializable]
  public class WebHookDeleteException : Exception
  {
    public WebHookDeleteException()
    {
    }

    public WebHookDeleteException(string message) : base(message)
    {
    }

    public WebHookDeleteException(string message, Exception inner) : base(message, inner)
    {
    }

    protected WebHookDeleteException(
      SerializationInfo info,
      StreamingContext context) : base(info, context)
    {
    }
  }
}