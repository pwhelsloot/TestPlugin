namespace AMCS.Data.Server.Http
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public class HttpRetryAttemptTime : IHttpRetryAttemptTime
  {
    public TimeSpan HttpRetryTime(int retryAttempt)
    {
      return TimeSpan.FromSeconds(Math.Min(Math.Pow(2, retryAttempt), 60));
    }
  }
}
