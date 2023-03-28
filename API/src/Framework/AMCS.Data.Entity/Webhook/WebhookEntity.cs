namespace AMCS.Data.Entity.WebHook
{
  using System;
  using AMCS.ApiService.Filtering;
  using AMCS.PluginData.Data.WebHook;
  using Webhook;

  [Serializable]
  [EntityTable("WebHook", "WebHookId")]
  public class WebHookEntity : CacheCoherentEntity
  {
    [EntityMember]
    public int? WebHookId { get; set; }

    [EntityMember]
    public string Name { get; set; }
    
    [EntityMember]
    public string SystemCategory { get; set; }

    /// <summary>
    /// As a matter of history, this column ( Trigger ) maps to an enum but is being left as a string variable
    /// instead of an enum/int because the DB schema was originally deployed as a string. The effort to change
    /// an existing column is a bit ridiculous since our ORM doesn't really properly support migrations of any kind
    /// </summary>
    [EntityMember]
    public string Trigger { get; set; }

    [EntityMember]
    public string Url { get; set; }
   
    [EntityMember]
    public int Format { get; set; }
    
    [EntityMember]
    public string HttpMethod { get; set; }
    
    [EntityMember]
    public string BasicCredentials { get; set; }
    
    [EntityMember]
    public string Headers { get; set; }

    [EntityMember]
    public string Filter { get; set; }

    [EntityMember]
    public string Environment { get; set; }

    [EntityMember]
    public Guid? TenantId { get; set; }
    
    [EntityMember]
    public bool InstalledViaRest { get; set; }

    public override int? GetId() => WebHookId;

    private static readonly string[] ValidatedProperties =
    {
      nameof(Name),
      nameof(Format),
      nameof(HttpMethod),
      nameof(Url),
      nameof(Filter),
      nameof(TenantId)
    };

    public override string[] GetValidatedProperties() => ValidatedProperties;

    protected override string GetValidationError(string propertyName)
    {
      switch (propertyName)
      {
        case nameof(Name):
          if (string.IsNullOrWhiteSpace(Name))
            return "Name cannot be empty";
          break;
        case nameof(Format):
          if (!Enum.TryParse<WebHookFormat>(Format.ToString(), out var webHookFormat))
            return "Format has to be Simple (0), Full (1) or Coalesce (2)";
          break;
        case nameof(HttpMethod):
          if (string.IsNullOrWhiteSpace(HttpMethod))
            return "Http Method be empty";

          if (Format == (int)WebHookFormat.Simple && HttpMethod != System.Net.Http.HttpMethod.Post.Method)
            return "If Format is Simple (0) the Http Method has to be Post";

          if (Format == (int)WebHookFormat.Full && (HttpMethod != System.Net.Http.HttpMethod.Post.Method &&
                                               HttpMethod != System.Net.Http.HttpMethod.Put.Method))
            return "If Format is Full (1) the Http Method has to be Post";

          if (Format == (int)WebHookFormat.Coalesce && HttpMethod != System.Net.Http.HttpMethod.Post.Method)
            return "If Format is Simple (2) the Http Method has to be Post or Put";
          break;
        case nameof(Url):
          if (!Uri.TryCreate(Url, UriKind.Absolute, out var urlResult) || (urlResult?.Scheme != Uri.UriSchemeHttp &&
              urlResult?.Scheme != Uri.UriSchemeHttps))
          {
            return $"{Url} is not a valid URL";
          }

          break;
        case nameof(Filter):
          if (!string.IsNullOrWhiteSpace(Filter) && !FilterParser.TryParse(Filter, out _))
            return $"\"{Filter}\" is not a valid filter";
          break;
        case nameof(TenantId):
          if (!TenantId.HasValue)
            return "Tenant ID cannot be empty";
          
          if (TenantId == Guid.Empty)
            return $"{Guid.Empty} is not a valid Tenant ID";
          break;
      }

      return null;
    }
  }
}