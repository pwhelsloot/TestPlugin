namespace AMCS.Data.Server.Http
{
  using System;
  using System.Net;
  using System.Net.Http;
  using System.Threading.Tasks;
  using Polly;
  using Polly.Retry;

  public class HttpRetryService : IHttpRetryService
  {
    public const int MaxRetryAttempts = 8;
    private static IHttpRetryAttemptTime HttpRetryAttemptTime;

    public HttpRetryService(IHttpRetryAttemptTime httpRetryAttemptTime)
    {
      HttpRetryAttemptTime = httpRetryAttemptTime;
    }

    private readonly AsyncRetryPolicy<HttpResponseMessage> retryPolicy = Policy
      .Handle<TaskCanceledException>()
      .Or<HttpRequestException>()
      .OrResult<HttpResponseMessage>(IsTransientHttpError)
      .WaitAndRetryAsync(MaxRetryAttempts, retryAttempt => HttpRetryAttemptTime.HttpRetryTime(retryAttempt));

    public Task<HttpResponseMessage> ExecuteHttpWithRetry(Func<Task<HttpResponseMessage>> callback)
    {
      return retryPolicy.ExecuteAsync(callback);
    }

    private static bool IsTransientHttpError(HttpResponseMessage response)
    {
      return response.StatusCode == HttpStatusCode.RequestTimeout ||
             response.StatusCode == HttpStatusCode.BadGateway ||
             response.StatusCode == HttpStatusCode.GatewayTimeout ||
             response.StatusCode == HttpStatusCode.ServiceUnavailable;
    }
  }
}