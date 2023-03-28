namespace AMCS.Data.Server.Plugin.MetadataRegistry
{
  using System.Threading.Tasks;

  public interface IMetadataRegistryStrategy
  {
    Task<string> GetMetadataRegistryAsXmlAsync();
  }
}