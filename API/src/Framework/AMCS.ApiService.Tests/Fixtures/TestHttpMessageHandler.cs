using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AMCS.ApiService.Tests.Fixtures
{
  public class TestHttpMessageHandler : HttpMessageHandler
  {
    public virtual Task<HttpResponseMessage> TestSendAsync(HttpRequestMessage request)
    {
      throw new NotSupportedException("For testing purposes only, this should be mocked. Getting this exception means that at least one of the responses has not been mocked");
    }

    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      return TestSendAsync(request);
    }
  }
}
