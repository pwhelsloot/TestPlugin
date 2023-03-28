using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Moq;

namespace AMCS.ApiService.Tests.Fixtures
{
  internal class HttpMessageHandlerFixture : BaseFixture<TestHttpMessageHandler>
  {
    public HttpMessageHandlerFixture()
      : base(true)
    {
    }

    public HttpMessageHandlerFixture SetupOkPost(string path, string content, Action<HttpRequestMessage> callback = null)
    {
      return SetupCall(path, HttpMethod.Post, HttpStatusCode.OK, content, callback);
    }

    public HttpMessageHandlerFixture SetupBadRequestPost(string path, string content = null)
    {
      return SetupCall(path, HttpMethod.Post, HttpStatusCode.BadRequest, content);
    }

    private HttpMessageHandlerFixture SetupCall(
      string path,
      HttpMethod method,
      HttpStatusCode statusCode,
      string content = null,
      Action<HttpRequestMessage> callback = null)
    {
      Mock.Setup(p => p.TestSendAsync(It.Is<HttpRequestMessage>(
          x => x.RequestUri.AbsolutePath == path
               && x.Method.Method == method.Method)))
        .Callback<HttpRequestMessage>(r => callback?.Invoke(r))
        .ReturnsAsync(new HttpResponseMessage(statusCode)
        {
          Content = !string.IsNullOrEmpty(content)
            ? new StringContent(content, Encoding.UTF8, "application/json")
            : null
        });

      return this;
    }
  }
}
