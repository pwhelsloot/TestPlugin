namespace AMCS.Data.Server.Webhook.Engine
{
  using System;
  using System.Collections.Generic;
  using System.Net.Http;
  using System.Threading.Tasks;
  using Entity.WebHook;
  using AMCS.Data.Server.WebHook.BslTrigger;
  using AMCS.PluginData.Data.WebHook;
  using HttpCallbacks;
  using Webhook.Exceptions;

  internal class PostCommitWebHookExecutor : IWebHookExecutor
  {
    public async Task ExecuteAll(List<WebHookEntity> webHooks, WebHookConfiguration configuration)
    {
      var taskList = GenerateExecutionTasks(webHooks, configuration);
      await Task.WhenAll(taskList);
    }

    private static List<Task<HttpResponseMessage>> GenerateExecutionTasks(List<WebHookEntity> webHooks, WebHookConfiguration configuration)
    {
      var list = new List<Task<HttpResponseMessage>>();

      foreach (var webHook in webHooks)
      {
        var format = (WebHookFormat)webHook.Format;

        if (!DataServices.TryResolveKeyed<IWebHookTriggerService>(format, out var callbackService))
          throw new WebHookExecuteException($"Unknown format {format} specified specified for webHook callback factory");

        list.Add(callbackService.Execute(webHook, configuration));
      }

      return list;
    }
  }
}