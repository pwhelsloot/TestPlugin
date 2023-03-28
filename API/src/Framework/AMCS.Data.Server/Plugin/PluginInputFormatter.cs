namespace AMCS.Data.Server.Plugin
{
  using System;
  using System.IO;
  using System.Text;
  using System.Threading.Tasks;
  using System.Xml;
  using AMCS.PluginData.Data;
  using AMCS.PluginData.Data.Configuration;
  using AMCS.PluginData.Data.Metadata;
  using AMCS.PluginData.Data.Remove;
  using AMCS.PluginData.Data.Update;
  using AMCS.PluginData.Data.WebHook;
  using AMCS.PluginData.Services;
  using Microsoft.AspNetCore.Mvc.Formatters;
  using Microsoft.Net.Http.Headers;
  using WebHook = AMCS.PluginData.Data.Metadata.WebHooks.WebHook;
  
  public class PluginInputFormatter : TextInputFormatter
  {
    public PluginInputFormatter()
    {
      SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/xml"));

      SupportedEncodings.Add(Encoding.UTF8);
      SupportedEncodings.Add(Encoding.Unicode);
    }

    protected override bool CanReadType(Type type)
    {
      return typeof(IPluginAction).IsAssignableFrom(type);
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
      var serializationService = DataServices.Resolve<IPluginSerializationService>();

      using(var reader = new StreamReader(context.HttpContext.Request.Body))
      {
        var content = await reader.ReadToEndAsync();
        var contentAsXml = new XmlDocument();
        contentAsXml.LoadXml(content);

        switch(contentAsXml.DocumentElement.NamespaceURI)
        {
          case PluginWebHook.Ns:
            var pluginWebHook = serializationService.Deserialize<PluginWebHook>(content);
            return await InputFormatterResult.SuccessAsync(pluginWebHook);
          case PluginConfiguration.Ns:
            var pluginConfiguration = serializationService.Deserialize<PluginConfiguration>(content);
            return await InputFormatterResult.SuccessAsync(pluginConfiguration);
          case PluginMetadata.Ns:
            var pluginMetadata = serializationService.Deserialize<PluginMetadata>(content);
            return await InputFormatterResult.SuccessAsync(pluginMetadata);
          case PluginRemove.Ns:
            var pluginRemove = serializationService.Deserialize<PluginRemove>(content);
            return await InputFormatterResult.SuccessAsync(pluginRemove);
          case PluginUpdate.Ns:
            var pluginUpdate = serializationService.Deserialize<PluginUpdate>(content);
            return await InputFormatterResult.SuccessAsync(pluginUpdate);
          default:
            break;
        }

        return await InputFormatterResult.NoValueAsync();
      }
    }
  }
}
