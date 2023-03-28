using AMCS.ApiService.Abstractions;
using AMCS.Data;
using Newtonsoft.Json;
using System.Linq;

namespace AMCS.ApiService.Configuration.DynamicAppConfig
{
  using Data.Server.Configuration.DynamicAppConfig;

  [ServiceRoute("configuration/dynamicAppConfiguration")]
  public class ApiConfigurationService : IMessageService<ApiConfigurationRequest, ApiConfigurationResponse>
  {
    public ApiConfigurationService()
    {
    }

    public ApiConfigurationResponse Perform(ISessionToken userId, ApiConfigurationRequest request)
    {
      IConfigurationService configurationService = DataServices.Resolve<IConfigurationService>();
      return new ApiConfigurationResponse()
      {
        ConfigurationValues = configurationService.GetPublicConfigValues(userId).Select(config => MapToApiConfig(config))
      };
    }


    /// <summary>
    /// Maps a internal ConfigValue to ApiConfigValue
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static ApiConfigValue MapToApiConfig(ConfigValue value)
    {
      ApiConfigValue apiConfigValue = new ApiConfigValue();
      apiConfigValue.ConfigId = value.ConfigId;
      apiConfigValue.Value = JsonConvert.SerializeObject(value.Value);
      return apiConfigValue;
    }
  }
}
