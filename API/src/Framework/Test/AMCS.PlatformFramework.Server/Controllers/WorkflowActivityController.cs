namespace AMCS.PlatformFramework.Server.Controllers
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Net.Http;
  using System.Text;
  using System.Text.Json;
  using System.Threading;
  using System.Threading.Tasks;
  using AMCS.PluginData.Data.Workflow;
  using AMCS.PlatformFramework.Server.Configuration;
  using AMCS.Data;
  using AMCS.Data.Entity.Workflow;
  using Microsoft.AspNetCore.Mvc;
  using Newtonsoft.Json;
  using JsonSerializer = System.Text.Json.JsonSerializer;

  [ApiController]
  [Route("services/api")]
  public class WorkflowActivityController : Controller
  { 
    [Route("usercontext")]
    [HttpGet]
    public IActionResult GenerateUserContext(string email, string userConnectionId)
    {
      var jsonObject = new
      {
        __UserContext = JsonConvert.SerializeObject(new
        {
          sub_uid = email,
          sub_cid = userConnectionId,
          sub_tid = DataServices.Resolve<IPlatformFrameworkConfiguration>().TenantId
        })
      };
      
      return Json(jsonObject);
    }
    
    [Route("transaction")]
    [HttpGet]
    public IActionResult Transaction(int transactionId)
    {
      var jsonObject = new
      {
        ScaleId = transactionId * 4,
        MaterialId = transactionId * 3,
        MaterialGradeCode = $"ABC{transactionId * 2}DEF",
        Material = $"GOLD COMPOUND #{transactionId}"
      };
      
      return Json(jsonObject);
    }

    [Route("materialWeight")]
    [HttpGet]
    public IActionResult MaterialWeight(int materialId, int scaleId)
    {
      var jsonObject = new
      {
        MaterialWeight = materialId * scaleId * 0.1M
      };

      return Json(jsonObject);
    }

    [Route("materialCost")]
    [HttpGet]
    public IActionResult MaterialCost(int materialId, string materialGradeCode, decimal materialWeight)
    {
      var jsonObject = new
      {
        MaterialCost = materialId * materialWeight
      };

      return Json(jsonObject);
    }
    
    [Route("longRunningProcess")]
    [HttpPost]
    public IActionResult LongRunningProcess([FromBody] AsyncRestWorkflowActivityRequest asyncRestWorkflowActivityRequest)
    {
      if (string.IsNullOrWhiteSpace(asyncRestWorkflowActivityRequest.CallbackUrl))
        return BadRequest();

      ThreadPool.QueueUserWorkItem(callback => ExecuteLongRunningProcess(asyncRestWorkflowActivityRequest).Wait());
      return NoContent();
    }

    private static async Task ExecuteLongRunningProcess(AsyncRestWorkflowActivityRequest asyncRestWorkflowActivityRequest)
    {
      var stopWatch = Stopwatch.StartNew();
      
      using (var httpClient = new HttpClient())
      {
        try
        {
          decimal numberOfUpdates =
            asyncRestWorkflowActivityRequest.Parameters.TryGetProperty("NumberOfUpdates", out var numberOfUpdatesProperty)
              ? numberOfUpdatesProperty.GetUInt32()
              : 0;
          
          var action =
            asyncRestWorkflowActivityRequest.Parameters.TryGetProperty("Action", out var actionProperty)
              ? actionProperty.GetString()
              : "Completed";

          if (string.Compare(action, "Error", StringComparison.OrdinalIgnoreCase) == 0)
            throw new InvalidOperationException("Sample Error Message");
          
          for (var i = 1; i <= numberOfUpdates; i++)
          {
            var updateRequest = new HttpRequestMessage
            {
              RequestUri = new Uri(asyncRestWorkflowActivityRequest.CallbackUrl),
              Method = new HttpMethod(HttpMethod.Post.ToString())
            };

            updateRequest.Content = new StringContent(
              JsonConvert.SerializeObject(new WorkflowInstanceCallback
              {
                Status = WorkflowInstanceCallbackStatusType.Running,
                Progress = new WorkflowInstanceCallbackProgress
                {
                  Message = $"Callback #{i}",
                  Progress = i / numberOfUpdates
                }
              }), Encoding.UTF8, "application/json");

            var runningResult = await httpClient.SendAsync(updateRequest);
            runningResult.EnsureSuccessStatusCode();
          }

          var completedRequest = new HttpRequestMessage
          {
            RequestUri = new Uri(asyncRestWorkflowActivityRequest.CallbackUrl),
            Method = new HttpMethod(HttpMethod.Post.ToString())
          };

          var outputs = new Dictionary<string, object>()
          {
            { "ExecutionCount", numberOfUpdates },
            { "ExecutionTimeInMilliseconds", stopWatch.ElapsedMilliseconds }
          };

          var asyncCallbackRequest = new WorkflowInstanceCallback
          {
            Status = WorkflowInstanceCallbackStatusType.Completed,
            Parameters = JsonDocument
              .Parse(JsonSerializer.Serialize(outputs))
              .RootElement
          };
          
          completedRequest.Content = new StringContent(
            JsonSerializer.Serialize(asyncCallbackRequest), 
            Encoding.UTF8, 
            "application/json");

          var completedResult = await httpClient.SendAsync(completedRequest);
          completedResult.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
          var errorRequest = new HttpRequestMessage
          {
            RequestUri = new Uri(asyncRestWorkflowActivityRequest.CallbackUrl),
            Method = new HttpMethod(HttpMethod.Post.ToString())
          };
          
          errorRequest.Content = new StringContent(
            JsonConvert.SerializeObject(new WorkflowInstanceCallback
            {
              Status = WorkflowInstanceCallbackStatusType.Failed,
              Error = new WorkflowInstanceCallbackError
              {
                Message = ex.Message,
                Type = ex.GetType().ToString(),
                StackTrace = ex.StackTrace
              }
            }), Encoding.UTF8, "application/json");
          
          await httpClient.SendAsync(errorRequest);
        }
      }
    }

    public class AsyncActivityRequest
    {
      public string CallbackUrl { get; set; }
      public JsonElement Parameters { get; set; }
    }
  }
}