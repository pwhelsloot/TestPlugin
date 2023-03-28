namespace AMCS.Data.Server.WebHook
{
  using System;
  using System.Collections.Generic;
  using System.Net.Http;
  using System.Security.Cryptography;
  using System.Text;
  using AMCS.Data.Configuration;
  using AMCS.Data.Entity.WebHook;
  using AMCS.Data.Server.Util;
  using AMCS.PluginData.Data.WebHook;

  public class WebHookRegistrationDto
  {
    public string RegistrationKey { get; }

    public Action<WebHookCallback> Callback { get; }

    public PluginData.Data.Metadata.WebHooks.WebHook WebHook { get; }

    public WebHookRegistrationDto(string name, WebHookFormat format, WebHookTrigger trigger, Action<WebHookCallback> callback, string filter = "")
    {
      RegistrationKey = Hash(new StringBuilder()
        .Append(name)
        .Append(format)
        .Append(trigger)
        .Append(callback)
        .ToString());

      var serviceRoot = DataServices.Resolve<IServiceRootResolver>()
        .GetServiceRoot(DataServices.Resolve<IProjectConfiguration>().ServiceRootName)
        .TrimEnd('/');

      WebHook = new PluginData.Data.Metadata.WebHooks.WebHook
      {
        Url = QueryHelper.AddQueryString(
          $"{serviceRoot}/services/api/webhook/callback", 
          new Dictionary<string, string> { { "registrationKey", RegistrationKey } }),
        Format = format,
        Name = name,
        Trigger = trigger,
        Filter = filter,
        HttpMethod = HttpMethod.Post.ToString()
      };

      Callback = callback;
    }

    public void UnRegister()
    {
      DataServices.Resolve<IWebHookService>().UnRegister(this);
    }

    private string Hash(string input)
    {
      using (var sha1 = SHA1.Create())
      {
        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(hash);
      }
    }
  }
}