namespace AMCS.Data.Server.Api
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Net;
  using System.Net.Http;
  using System.Text;
  using System.Threading.Tasks;
  using AMCS.Data.Configuration;
  using AMCS.Data.Server.PlatformCredentials;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  
  public class RestApiService : IRestApiService
  {
    private readonly IPlatformCredentialsTokenManager platformCredentialsTokenManager;

    public RestApiService(IPlatformCredentialsTokenManager platformCredentialsTokenManager)
    {
      this.platformCredentialsTokenManager = platformCredentialsTokenManager;
    }

    public ApiResult<IList<T>> GetCollection<T>(GetCollectionParams @params)
    {
      return Task.Run(() => GetCollectionAsync<T>(@params)).Result;
    }

    public async Task<ApiResult<IList<T>>> GetCollectionAsync<T>(GetCollectionParams @params)
    {
      var parameters = new Dictionary<string, string>();
      if (@params.Filter != null)
        parameters.Add("filter", @params.Filter);
      if (@params.Search != null)
        parameters.Add("search", @params.Search);
      if (@params.Order != null)
        parameters.Add("order", @params.Order);
      if (@params.Max.HasValue)
        parameters.Add("max", @params.Max.Value.ToString());
      if (@params.Page.HasValue)
        parameters.Add("page", @params.Page.Value.ToString());
      if (@params.IncludeCount.HasValue)
        parameters.Add("includeCount", @params.IncludeCount.Value ? "true" : "false");

      return await DoApiRequest<IList<T>>(@params.Url, @params.Username, @params.UserRoles, @params.TenantId, parameters, HttpMethod.Get, null);
    }

    private async Task<JToken> DoRequest(string url, string username, List<string> userRoles, string tenantId, HttpMethod method, JToken json)
    {
      return await DoRequest(url, username, userRoles, tenantId, null, method, json);
    }

    private async Task<JToken> DoRequest(string url, string username, List<string> userRoles, string tenantId, Dictionary<string, string> parameters,
      HttpMethod method, JToken json)
    {
      Action<Stream> requestWriter = null;

      if (json != null)
      {
        requestWriter = p =>
        {
          using (var writer = new StreamWriter(p))
          using (var jsonWriter = new JsonTextWriter(writer))
          {
            json.WriteTo(jsonWriter);
          }
        };
      }

      return await DoRequest(
        url,
        username,
        userRoles,
        tenantId,
        parameters,
        method,
        requestWriter,
        p =>
        {
          using (var reader = new StreamReader(p))
          using (var jsonReader = new JsonTextReader(reader))
          {
            return JToken.ReadFrom(jsonReader);
          }
        }
      );
    }

    private async Task<ApiResult<T>> DoApiRequest<T>(string url, string username, List<string> userRoles, string tenantId, Dictionary<string, string> parameters,
      HttpMethod method, JToken json)
    {
      Action<Stream> requestWriter = null;

      if (json != null)
      {
        requestWriter = p =>
        {
          using (var writer = new StreamWriter(p))
          using (var jsonWriter = new JsonTextWriter(writer))
          {
            json.WriteTo(jsonWriter);
          }
        };
      }

      return await DoRequest(
        url,
        username,
        userRoles,
        tenantId,
        parameters,
        method,
        requestWriter,
        stream =>
        {
          using (var reader = new StreamReader(stream))
          {
            return JsonConvert.DeserializeObject<ApiResult<T>>(reader.ReadToEnd());
          }
        }
      );
    }

    private async Task<T> DoRequest<T>(string url, string username, List<string> userRoles, string tenantId, Dictionary<string, string> parameters, HttpMethod method,
      Action<Stream> writer, Func<Stream, T> reader)
    {
      if (parameters != null)
      {
        var stringBuilder = new StringBuilder();

        foreach (var entry in parameters)
        {
          stringBuilder
            .Append(stringBuilder.Length > 0 ? '&' : '?')
            .Append(Uri.EscapeDataString(entry.Key))
            .Append('=')
            .Append(Uri.EscapeDataString(entry.Value));
        }

        url += stringBuilder.ToString();
      }

      var request = CreateWebRequest(url, username, userRoles, tenantId, method);

      if (writer != null)
      {
        request.ContentType = "application/json";

        using (var stream = await request.GetRequestStreamAsync())
        {
          writer(stream);
        }
      }

      using (var response = await request.GetResponseAsync())
      using (var stream = response.GetResponseStream())
      {
        return stream == null
          ? default
          : reader(stream);
      }
    }

    private HttpWebRequest CreateWebRequest(string url, string username, List<string> userRoles, string tenantId, HttpMethod method)
    {
#pragma warning disable SYSLIB0014
      var request = (HttpWebRequest)WebRequest.Create(url);
#pragma warning restore SYSLIB0014
      request.Method = method.ToString().ToUpper();

      if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(tenantId))
      {
        request.CookieContainer = new CookieContainer();

        var platformCredentials = new PlatformCredentials(username, tenantId);
        if (userRoles?.Count > 0)
          platformCredentials.Roles.AddRange(userRoles);

        var token = platformCredentialsTokenManager.Serialize(platformCredentials);

        var domain = new Uri(DataServices.Resolve<IServiceRootResolver>().ServiceRoot).GetBaseDomain();
        request.CookieContainer.Add(new Cookie("Platform.Credentials", token, "/", domain));
      }

      return request;
    }

    public class GetCollectionParams
    {
      public string Username { get; set; }
      public List<string> UserRoles { get; } = new List<string>();
      public string TenantId { get; set; }
      public string Url { get; set; }
      public string Filter { get; set; }
      public string Search { get; set; }
      public string Order { get; set; }
      public int? Max { get; set; }
      public int? Page { get; set; }
      public bool? IncludeCount { get; set; }
    }
  }
}