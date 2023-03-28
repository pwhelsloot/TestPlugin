#if NETFRAMEWORK

namespace AMCS.ApiService.Controllers.Plugin
{
  using System.Net;
  using System.Threading.Tasks;
  using System.Web.Mvc;
  using System.Collections.Generic;
  using AMCS.Data;
  using AMCS.Data.Server.Plugin.MetadataRegistry;
  using AMCS.PluginData.Data.Metadata.MetadataRegistries;
  using AMCS.Data.Entity.Plugin;
  using ActionResult = System.Web.Mvc.ActionResult;
  using System;
  using log4net;

  [RoutePrefix("metadata")]
  [Authenticated]
  public class PluginMetadataRegistryController : Controller
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(PluginMetadataRegistryController));

    [HttpGet]
    [Route("web-hooks.xml")]
    public async Task<ActionResult> GetWebHooks()
    {
      return await GetMetadataRegistryAsXmlAsync(MetadataRegistryType.WebHooks);
    }

    [HttpGet]
    [Route("workflow-activities.xml")]
    public async Task<ActionResult> GetWorkflowActivities()
    {
      return await GetMetadataRegistryAsXmlAsync(MetadataRegistryType.WorkflowActivities);
    }

    [HttpGet]
    [Route("ui-components.xml")]
    public async Task<ActionResult> GetUiComponents()
    {
        return await GetMetadataRegistryAsXmlAsync(MetadataRegistryType.UiComponents);
    }

    [HttpGet]
    [Route("business-objects.xml")]
    public async Task<ActionResult> GetBusinessObjects()
    {
      return await GetMetadataRegistryAsXmlAsync(MetadataRegistryType.BusinessObjects);
    }

    private async Task<ActionResult> GetMetadataRegistryAsXmlAsync(MetadataRegistryType type)
    {
      try
      {
        string xml = null;

        if (!DataServices.TryResolve<IPluginSystem>(out _))
          return new HttpStatusCodeResult(HttpStatusCode.NotFound);

        if (!DataServices.TryResolveKeyed<IMetadataRegistryStrategy>(type, out var metadataRegistryTypeService))
          return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        xml = await metadataRegistryTypeService.GetMetadataRegistryAsXmlAsync();

        return Content(xml, "text/xml");
      }
      catch (Exception exception)
      {
        Logger.Info($"Failed to get {type}.xml", exception);
        return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError);
      }
    }
  }
}

#endif