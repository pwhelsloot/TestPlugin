namespace AMCS.Data.Entity.Workflow
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.Json;
  using System.Text.Json.Serialization;
  using PluginData.Utils;

  public class ShowScreenActivityRequest
  {
    public string CallbackUrl { get; set; }

    public JsonElement Parameters { get; set; }

    public string Name { get; set; }

    public string UserContext { get; set; }

    public List<string> AllowedActions { get; set; }

    [JsonIgnore]
    public WorkflowUserContext TypedUserContext
    {
      get
      {
        return string.IsNullOrEmpty(UserContext) ? null : JsonSerializer.Deserialize<WorkflowUserContext>(UserContext);
      }
    }

    public (bool isValid, List<(string Name, string Message)> validationErrors) GetValidationErrors()
    {
      var result = ValidateName();
      if (!result.isValid)
        return result;

      result = ValidateUserContext();
      if (!result.isValid)
        return result;

      result = ValidateAllowedActions();
      if (!result.isValid)
        return result;

      return ValidateCallbackUrl();
    }

    private (bool isValid, List<(string Name, string Message)> validationErrors) ValidateName()
    {
      if (string.IsNullOrWhiteSpace(Name))
      {
        return (false, new List<(string, string)>
                {
                    ("Name", "Name cannot be empty")
                });
      }

      // Expecting something like amcs/scale:CreateWeighTicket
      if (!PluginMetadataUtility.IsValidPluginFullyQualifiedObjectName(Name))
      {
        return (false, new List<(string, string)>
                {
                    ("Name", $"'{Name}' is invalid. It must be in the format of 'vendorid/pluginid:componentid'")
                });
      }

      var indexOfColon = Name.LastIndexOf(":");
      var plugin = Name.Substring(0, indexOfColon);
      var componentName = Name.Substring(Name.LastIndexOf(":") + 1);

      if (!PluginMetadataUtility.IsValidPluginFullyQualifiedName(plugin) || string.IsNullOrWhiteSpace(componentName))
      {
        return (false, new List<(string, string)>
                {
                    ("Name", $"'{Name}' is invalid. It must be in the format of 'vendorid/pluginid:componentid'")
                });
      }

      return (true, new List<(string, string)>());
    }

    private (bool isValid, List<(string Name, string Message)> validationErrors) ValidateUserContext()
    {
      if (TypedUserContext == null)
      {
        return (false, new List<(string, string)>
                {
                    ("UserContext", "UserContext cannot be null")
                });
      }

      if (string.IsNullOrWhiteSpace(TypedUserContext.UserConnectionId) || 
          string.IsNullOrWhiteSpace(TypedUserContext.Email) || 
          string.IsNullOrWhiteSpace(TypedUserContext.TenantId))
      {
        return (false, new List<(string, string)>
                {
                    ("UserContext", "UserContext must have a tenant, identity and connectionId")
                });
      }

      return (true, new List<(string, string)>());
    }

    private (bool isValid, List<(string Name, string Message)> validationErrors) ValidateAllowedActions()
    {
      if (AllowedActions?.GroupBy(action => action).Count() != AllowedActions?.Count)
      {
        return (false, new List<(string, string)>
                {
                    ("AllowedActions", "AllowedActions may not contain duplicates")
                });
      }

      return (true, new List<(string, string)>());
    }

    private (bool isValid, List<(string Name, string Message)> validationErrors) ValidateCallbackUrl()
    {
      if (string.IsNullOrWhiteSpace(CallbackUrl))
      {
        return (false, new List<(string, string)>
                {
                    ("CallbackUrl", "CallbackUrl cannot be empty")
                });
      }

      var validUrl = Uri.TryCreate(CallbackUrl, UriKind.Absolute, out var uriResult) && 
                     (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
      if (!validUrl)
      {
        return (false, new List<(string, string)>
                {
                    ("CallbackUrl", $"'{CallbackUrl}' is not a valid URL.")
                });
      }

      return (true, new List<(string, string)>());
    }
  }
}