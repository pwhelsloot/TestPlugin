namespace AMCS.Data.Server.Plugin.MetadataRegistry.BusinessObject
{
  using AMCS.PluginData.Data.MetadataRegistry.BusinessObjects;

  public interface IBusinessObjectMetadataRegistryService
  {
    BusinessObjectRegistry CreateBusinessObjectMetadataRegistry();
  }
}