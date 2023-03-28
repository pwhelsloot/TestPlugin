namespace AMCS.Data.Server.Http
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public interface IHttpRetryAttemptTime
  {
    TimeSpan HttpRetryTime(int retryAttempt);
  }
}
