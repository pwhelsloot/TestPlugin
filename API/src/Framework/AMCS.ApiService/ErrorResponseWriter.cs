using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Documentation.Abstractions.Swagger;
using AMCS.Data;
using AMCS.Data.Server;
using Newtonsoft.Json;

namespace AMCS.ApiService
{
  internal static class ErrorResponseWriter
  {
    public static void WriteError(JsonTextWriter json, Exception exception, bool isCustomErrorEnabled)
    {
      json.WriteStartObject();

      json.WritePropertyName("message");
      json.WriteValue(exception.Message);

      if (exception is BslUserException userException && userException.ErrorCode.HasValue)
      {
        json.WritePropertyName("code");
        json.WriteValue(userException.ErrorCode.Value);

        json.WritePropertyName("codeUrl");
        json.WriteValue(DataServices.Resolve<IApiExplorerConfiguration>().GetErrorCodeUrl(userException.ErrorCode.Value));
      }

      // Only output technical information if custom errors are disabled. Custom errors imply
      // that we don't want to send technical information because the API e.g. is opened up
      // on the internet.

      if (!isCustomErrorEnabled)
      {
        json.WritePropertyName("type");
        json.WriteValue(exception.GetType().FullName);

        if (exception.StackTrace != null)
        {
          json.WritePropertyName("stackTrace");
          json.WriteValue(exception.StackTrace);
        }

        if (exception.InnerException != null)
        {
          json.WritePropertyName("cause");
          WriteError(json, exception.InnerException, isCustomErrorEnabled);
        }
      }

      json.WriteEndObject();
    }
  }
}
