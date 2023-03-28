#if NETFRAMEWORK

namespace AMCS.ApiService.Controllers.Plugin
{
  using System;
  using System.IO;
  using System.Text;
  using System.Web.Mvc;
  using System.Xml;
  using AMCS.Data;
  using AMCS.PluginData.Data;
  using AMCS.PluginData.Data.Configuration;
  using AMCS.PluginData.Data.Metadata;
  using AMCS.PluginData.Data.Remove;
  using AMCS.PluginData.Data.Update;
  using AMCS.PluginData.Data.WebHook;
  using AMCS.PluginData.Services;

  public class PluginActionFilterAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      var serializationService = DataServices.Resolve<IPluginSerializationService>();
      using (var receiveStream = filterContext.HttpContext.Request.InputStream)
      {
        using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
        {
          var content = readStream.ReadToEndAsync().Result;
          var contentAsXml = new XmlDocument();
          contentAsXml.LoadXml(content);
          IPluginAction formattedInput = null;
          switch (contentAsXml.DocumentElement.NamespaceURI)
          {
            case PluginWebHook.Ns:
              formattedInput = serializationService.Deserialize<PluginWebHook>(content);
              break;
            case PluginConfiguration.Ns:
              formattedInput = serializationService.Deserialize<PluginConfiguration>(content);
              break;
            case PluginMetadata.Ns:
              formattedInput = serializationService.Deserialize<PluginMetadata>(content);
              break;
            case PluginRemove.Ns:
              formattedInput = serializationService.Deserialize<PluginRemove>(content);
              break;
            case PluginUpdate.Ns:
              formattedInput = serializationService.Deserialize<PluginUpdate>(content);
              break;
            default:
              break;
          }
          if (formattedInput == null)
            throw new ArgumentException("Invalid object");
          filterContext.Controller.ViewData.Model = formattedInput;
        }
      }
      base.OnActionExecuting(filterContext);
    }
  }
}

#endif