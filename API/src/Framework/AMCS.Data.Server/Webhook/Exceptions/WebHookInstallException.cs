namespace AMCS.Data.Server.Webhook.Exceptions
{
  using System;
  using System.Runtime.Serialization;

  [Serializable]
  public class WebHookInstallException : Exception
  {
    public WebHookInstallException()
    {
    }

    public WebHookInstallException(string message) : base(message)
    {
    }

    public WebHookInstallException(string message, Exception inner) : base(message, inner)
    {
    }

    protected WebHookInstallException(
      SerializationInfo info,
      StreamingContext context) : base(info, context)
    {
    }
  }
}