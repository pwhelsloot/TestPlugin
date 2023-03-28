using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AMCS.Data;
using Newtonsoft.Json;

namespace AMCS.ApiService.Elemos.Batch
{
  internal interface IBatchRequest
  {
    void Perform(JsonWriter json, ISessionToken user, IApiContext context);
  }
}
