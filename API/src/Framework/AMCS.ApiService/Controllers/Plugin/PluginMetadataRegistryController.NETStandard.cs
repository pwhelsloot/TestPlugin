#if !NETFRAMEWORK

namespace AMCS.ApiService.Controllers.Plugin
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Data;
  using AMCS.Data.Entity.Plugin;
  using AMCS.Data.Server.Plugin.MetadataRegistry;
  using AMCS.PluginData.Data.Metadata.MetadataRegistries;
  using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;
  using System.Net;
  using log4net;


  [Route("metadata")]
  [ApiAuthorize]
  public class PluginMetadataRegistryController : Controller
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(PluginMetadataRegistryController));

    [HttpGet("{type}.xml")]
    public async Task<ActionResult> Get([FromRoute] string type)
    {
      try
      {
        string xml = null;

        if (!DataServices.TryResolve<IPluginSystem>(out _))
         return NotFound();

        if (!Enum.TryParse<MetadataRegistryType>(type?.Replace("-", string.Empty), true, out var metadataRegistryType)
            || !DataServices.TryResolveKeyed<IMetadataRegistryStrategy>(metadataRegistryType, out var metadataRegistryTypeService))
        {
          return BadRequest();
        }
        
        xml = await metadataRegistryTypeService.GetMetadataRegistryAsXmlAsync();

        return Content(xml, "text/xml");
      }
      catch (Exception exception)
      {
        Logger.Info($"Failed to get {type}.xml", exception);
        return StatusCode((int)HttpStatusCode.InternalServerError);
      }
    }
  }
}

#endif