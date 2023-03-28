namespace AMCS.Data.Server.Webhook.Engine.HttpCallbacks
{
  using System;
  using System.Linq;
  using System.Net.Http;
  using System.Net.Http.Headers;
  using System.Text;
  using AMCS.Data.Entity.Plugin;
  using AMCS.Data.Entity.WebHook;
  using AMCS.Data.Server.PlatformCredentials;
  using AMCS.Data.Support.Security;
  using AMCS.PluginData.Data.WebHook;

  internal class HttpCallbackService : IHttpCallbackService
  {
    public HttpRequestMessage GenerateRequestMessage(WebHookEntity entity, Uri requestUri)
    {
      var requestMessage = new HttpRequestMessage
      {
        Method = HttpMethod.Post,
        RequestUri = requestUri
      };

      // Full can either be POST or PUT
      if (entity.Format == (int)WebHookFormat.Full)
        requestMessage.Method = ParseWebHookHttpMethod(entity.HttpMethod);

      // We're making the assumption here that the format is already in "username:password"
      if (!string.IsNullOrEmpty(entity.BasicCredentials))
      {
        var basicCredentials = StringEncryptor.DefaultEncryptor.Decrypt(entity.BasicCredentials);
        var encodedCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(basicCredentials));
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);
      }

      var tenantId = entity.TenantId.ToString();

      var platformCredentials = new PlatformCredentials(
        $"app:{DataServices.Resolve<IPluginSystem>().FullyQualifiedName}",
        tenantId);

      var token = DataServices.Resolve<IPlatformCredentialsTokenManager>().Serialize(platformCredentials);
      requestMessage.Headers.Add("Cookie", $"{PlatformCredentials.CookieName}={token}");

      if (!string.IsNullOrEmpty(entity.Headers))
      {
        string[] separators = { Environment.NewLine };
        var headers = entity.Headers.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        foreach (var header in headers.Where(content => content.Split(new[] { ':' }, 2).Length == 2))
        {
          var split = header.Split(new[] { ':' }, 2);
          requestMessage.Headers.Add(split[0].Trim(), split[1].Trim());
        }
      }

      return requestMessage;
    }

    private static HttpMethod ParseWebHookHttpMethod(string method)
    {
      if (string.Equals(HttpMethod.Post.Method, method, StringComparison.InvariantCultureIgnoreCase))
        return HttpMethod.Post;

      if (string.Equals(HttpMethod.Put.Method, method, StringComparison.InvariantCultureIgnoreCase))
        return HttpMethod.Put;

      throw new ArgumentOutOfRangeException(nameof(method));
    }
  }
}
