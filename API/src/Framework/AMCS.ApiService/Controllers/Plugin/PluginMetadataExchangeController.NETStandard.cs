#if !NETFRAMEWORK

namespace AMCS.ApiService.Controllers.Plugin
{
  using System;
  using System.Net;
  using Data;
  using AMCS.Data.Entity.Plugin;
  using Data.Server;
  using AMCS.Data.Server.Configuration;
  using AMCS.Data.Server.Plugin;
  using Data.Server.Services;
  using AMCS.PluginData.Data;
  using AMCS.PluginData.Data.Configuration;
  using AMCS.PluginData.Data.Metadata;
  using PluginData.Services;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.ModelBinding;
  using System.Collections.Generic;
  using log4net;
  using Support;

  [Route("plugin/mex")]
  [ApiAuthorize(Policy = ApiPolicy.RequiresCoreIdentity)]
  public class PluginMetadataExchangeController : Controller
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(PluginMetadataExchangeController));

    [HttpPost]
    public ActionResult PerformMex([FromBody] IPluginAction pluginAction)
    {
      try
      {
        if (!DataServices.TryResolve<IPluginSystem>(out _))
          return NotFound();
        
        if (pluginAction is PluginConfiguration configuration)
          return GetMetadata(configuration);

        if (pluginAction is PluginMetadata metadata)
          return ProcessMetadata(metadata);

        return BadRequest();
      }
      catch (Exception ex)
      {
        return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    private ActionResult GetMetadata(PluginConfiguration pluginConfiguration)
    {
      var (isValid, actionResult) = ValidateRequest(pluginConfiguration.GetValidationErrors);

      if (!isValid)
        return actionResult;

      var tenantId = HttpContext.GetAuthenticatedUser().TenantId;
      var pluginMetadata = DataServices
        .Resolve<IPluginMetadataService>()
        .GetMetadata(tenantId, pluginConfiguration);

      if (DataServices.Resolve<ISlotSwitchService>().IsSlotSwitching(pluginMetadata, pluginConfiguration))
      {
        Logger.Warn($"Version mismatch. Expected version from Core App: {pluginConfiguration.Version}, actual current version: {pluginMetadata.Version}");
        
        var retryAfter = DataServices.Resolve<IServerConfiguration>().PluginUpdateRetryAfterInSeconds ?? 60;
        HttpContext.Response.Headers.Add("Retry-After", retryAfter.ToString());
        return StatusCode((int)HttpStatusCode.ServiceUnavailable);
      }
      
      var xml = DataServices
        .Resolve<IPluginSerializationService>()
        .Serialize(pluginMetadata);

      xml = DataServices
        .Resolve<IMexPostProcessingService>()
        .ExecuteGetMetadataPostProcessing(pluginConfiguration, xml);
      
      return Content(xml, "text/xml");
    }

    private ActionResult ProcessMetadata(PluginMetadata pluginMetadata)
    {
      if (!DataServices.TryResolve<IMetadataProcessor>(out var metadataProcessor))
        return NotFound();

      var (isValid, actionResult) = ValidateRequest(pluginMetadata.GetValidationErrors);

      if (!isValid)
        return actionResult;

      var sessionToken = HttpContext.GetAuthenticatedUser();

      using (var dataSession = BslDataSessionFactory.GetDataSession(sessionToken))
      using (var transaction = dataSession.CreateTransaction())
      {
        metadataProcessor.ProcessMetadata(pluginMetadata, sessionToken, dataSession);

        if (pluginMetadata.Mode == PluginMetadataMode.Commit)
          transaction.Commit();
      }

      return NoContent();
    }

    private (bool isValid, ActionResult) ValidateRequest(
      Func<(bool isValid, List<(string Name, string Message)> validationErrors)> getValidationErrors)
    {
      var (isValid, validationErrors) = getValidationErrors();
      
      if (isValid)
        return (true, null);
      
      var modelStateDictionary = new ModelStateDictionary();
      foreach (var (key, value) in validationErrors)
      {
        modelStateDictionary.AddModelError(key, value);
      }
      return (false, BadRequest(modelStateDictionary));
    }
  }
}

#endif