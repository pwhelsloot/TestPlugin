namespace AMCS.Data.Server.Plugin.MetadataRegistry.BusinessObject
{
  using System;
  using System.Threading.Tasks;
  using AMCS.PluginData.Services;

  public class BusinessObjectMetadataRegistryStrategy : IMetadataRegistryStrategy
  {
    public Task<string> GetMetadataRegistryAsXmlAsync()
    {
      if (!DataServices.TryResolve<IBusinessObjectMetadataRegistryService>(out var businessObjectMetadataRegistryService))
        throw new InvalidOperationException("Business objects have not been configured for this system");

      var businessObjectRegistry = businessObjectMetadataRegistryService.CreateBusinessObjectMetadataRegistry();

      var xml = DataServices
        .Resolve<IPluginSerializationService>()
        .Serialize(businessObjectRegistry);

      return Task.FromResult(xml);
    }
  }
}