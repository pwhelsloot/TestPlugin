using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService
{
  internal interface IApiContext
  {
    string BaseUrl { get; }

    Exception CreateHttpException(HttpStatusCode statusCode, string status = null);
  }
}
