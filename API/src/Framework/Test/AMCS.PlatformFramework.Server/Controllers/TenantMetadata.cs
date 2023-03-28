namespace AMCS.PlatformFramework.Server.Controllers
{
  using Newtonsoft.Json;

  public class TenantMetadata
  {
    [JsonProperty("core_service_root")]
    public string CoreServiceRoot { get; set; }

    [JsonProperty("tenant_id")]
    public string TenantId { get; set; }
  }
}