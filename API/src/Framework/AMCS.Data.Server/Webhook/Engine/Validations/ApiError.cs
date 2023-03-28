namespace AMCS.Data.Server.Webhook.Engine.Validations
{
  using System.Collections.Generic;
  using System.Text.Json.Serialization;

  internal class ApiError
  {
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("stackTrace")]
    public string StackTrace { get; set; }
  }
}