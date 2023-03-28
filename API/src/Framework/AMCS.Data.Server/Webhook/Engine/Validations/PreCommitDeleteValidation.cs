namespace AMCS.Data.Server.Webhook.Engine.Validations
{
  using System.Linq;
  using System.Net;
  using System.Net.Http;
  using System.Text.Json;
  using System.Threading.Tasks;
  using Exceptions;
  using AMCS.Data.Entity.WebHook;

  internal class PreCommitDeleteValidation : IWebHookValidation
  {
    public async Task Validate((WebHookEntity WebHook, HttpResponseMessage Response)[] objectsToBeValidated)
    {
      foreach (var objectToValidate in objectsToBeValidated)
      {
        var contentType = objectToValidate.Response.Content.Headers.ContentType?.MediaType;
        var message = await objectToValidate.Response.Content.ReadAsStringAsync();

        if (objectToValidate.Response.IsSuccessStatusCode && contentType == "application/json" &&
            !string.IsNullOrWhiteSpace(message))
        {
          throw new WebHookExecuteException(
            $"{objectToValidate.WebHook.SystemCategory}:{objectToValidate.WebHook.Name}: Web hook pre-commit delete response content must be empty");
        }

        if (objectToValidate.Response.IsSuccessStatusCode)
          continue;
        
        var errorResponse =
          $"{objectToValidate.WebHook.SystemCategory}:{objectToValidate.WebHook.Name} web hook returned a {objectToValidate.Response.StatusCode} status";

        if (!TryDeserialize<ApiError>(message, out var jsonResult))
        {
          if (contentType == "application/json" && !string.IsNullOrWhiteSpace(message))
            throw new WebHookExecuteException($"{errorResponse}: {message}");

          throw new WebHookExecuteException(errorResponse);
        }

        switch (objectToValidate.Response.StatusCode)
        {
          case HttpStatusCode.BadRequest:
            throw BslUserExceptionFactory<BslUserException>.CreateException($"{jsonResult.Message}");

          default:
            throw new WebHookExecuteException($"{errorResponse}: {jsonResult.Message}");
        }
      }
    }

    private static bool TryDeserialize<TType>(string message, out TType output)
      where TType : class
    {
      try
      {
        var result = string.IsNullOrWhiteSpace(message)
          ? default
          : JsonSerializer.Deserialize<TType>(message);

        output = result;

        return default != result;
      }
      catch
      {
        output = default;
        return false;
      }
    }
  }
}