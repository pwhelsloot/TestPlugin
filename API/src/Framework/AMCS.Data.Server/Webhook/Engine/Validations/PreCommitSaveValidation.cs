namespace AMCS.Data.Server.Webhook.Engine.Validations
{
  using System;
  using System.Linq;
  using System.Net;
  using AMCS.Data.Entity.WebHook;
  using System.Net.Http;
  using System.Text.Json;
  using System.Threading.Tasks;
  using Entity;
  using Exceptions;

  internal class PreCommitSaveValidation : IWebHookValidation
  {
    public async Task Validate((WebHookEntity WebHook, HttpResponseMessage Response)[] objectsToBeValidated)
    {
      foreach (var objectToValidate in objectsToBeValidated)
      {
        var contentType = objectToValidate.Response.Content.Headers.ContentType?.MediaType;
        var message = await objectToValidate.Response.Content.ReadAsStringAsync();

        if (objectToValidate.Response.IsSuccessStatusCode)
        {
          if (string.IsNullOrWhiteSpace(message) || contentType != "application/json")
            continue;

          using (var document = JsonDocument.Parse(message))
          {
            if (document.RootElement.EnumerateObject().Any(property => property.Name != UdfConstants.UdfProperty))
              throw new WebHookExecuteException(
                $"Non-udf properties are not currently allowed in pre-commit web hook response data");
          }

          continue;
        }
        
        var result = string.IsNullOrWhiteSpace(message)
          ? null
          : JsonSerializer.Deserialize<ApiError>(message);

        var errorResponse =
          $"{objectToValidate.Response.StatusCode} response error return with web hook call back for {objectToValidate.WebHook.SystemCategory}:{objectToValidate.WebHook.Name}";

        if (contentType == "application/json" && !string.IsNullOrWhiteSpace(message) && result == null)
          throw new WebHookExecuteException($"{errorResponse}: {message}");

        switch (objectToValidate.Response.StatusCode)
        {
          case HttpStatusCode.BadRequest when (contentType == "application/json" && result != null):
            throw new BslUserException(result.Message);

          default:
            throw new WebHookExecuteException(result == null ? $"{errorResponse}" : $"{errorResponse}: {result.Message}");
        }
      }

    }
  }
}