#if NETFRAMEWORK

namespace AMCS.ApiService.Controllers.Plugin
{
  using System;
  using System.Net;
  using System.Collections.Generic;
  using System.Web.Mvc;
  using AMCS.Data;
  using AMCS.Data.Server.Configuration;
  using AMCS.PluginData.Data.Metadata;
  using AMCS.Data.Server;
  using AMCS.Data.Server.Plugin;
  using System.Linq;
  using AMCS.Data.Entity.Plugin;
  using AMCS.PluginData.Data.Configuration;
  using AMCS.PluginData.Services;
  using Support;

  [Route("plugin/mex")]
  [Authenticated(RequiredIdentity = WellKnownIdentities.CoreApp)]
  public class PluginMetadataExchangeController : Controller
  {
    [HttpPost]
    [PluginActionFilter]
    public ActionResult PerformMex()
    {
      try
      {
        if (!DataServices.TryResolve<IPluginSystem>(out _))
          return HttpNotFound();

        var pluginAction = ViewData.Model;

        if (pluginAction is PluginConfiguration configuration)
          return GetMetadata(configuration);

        if (pluginAction is PluginMetadata metadata)
          return ProcessMetadata(metadata);
        
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      catch (Exception ex)
      {
        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    private ActionResult GetMetadata(PluginConfiguration pluginConfiguration)
    {
      var (isValid, actionResult) = ValidateIncomingRequest(pluginConfiguration.GetValidationErrors);

      if (!isValid)
        return actionResult;

      var tenantId = HttpContext.GetAuthenticatedUser().TenantId;
      var pluginMetadata = DataServices.Resolve<IPluginMetadataService>()
        .GetMetadata(tenantId, pluginConfiguration);

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
        return new HttpNotFoundResult();

      var (isValid, actionResult) = ValidateIncomingRequest(pluginMetadata.GetValidationErrors);

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

      return new HttpStatusCodeResult(HttpStatusCode.NoContent);
    }

    private (bool isValid, ActionResult) ValidateIncomingRequest(
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

      return (false,
        new HttpStatusCodeResult(HttpStatusCode.BadRequest,
          string.Join(",", validationErrors.Select(error => $"{error.Name}: {error.Message}"))));
    }
  }
}

#endif