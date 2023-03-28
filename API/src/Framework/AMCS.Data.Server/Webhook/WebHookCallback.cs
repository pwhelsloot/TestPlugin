using System;

namespace AMCS.Data.Server.WebHook
{
  public class WebHookCallback
  {
    public string RegistrationKey { get; set; }

    public string Id { get; set; }

    public string Trigger { get; set; }

    public string TransactionId { get; set; }

    public string Body { get; set; }
  }
}