#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebSockets;
using AMCS.ApiService.Support;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.Data.Server.DataSets;
using AMCS.JobSystem;
using AMCS.JobSystem.Scheduler.Api;
using Newtonsoft.Json;

namespace AMCS.ApiService.DataSets
{
  [Authenticated]
  public class DataSetsController : Controller
  {
    [Route("datasets/metadata")]
    public ActionResult GetDataSets()
    {
      return new JsonSerializerResult(
        DataServices.Resolve<IDataSetService>().DataSets,
        CreateSerializer()
      );
    }

    [Route("datasets/query")]
    [HttpPost]
    public ActionResult GetQuery(int? cursor = null)
    {
      var query = ParseBody<DataSetQuery>();
      DataSetQueryCursor queryCursor = null;
      if (cursor.HasValue)
        queryCursor = new DataSetQueryCursor(cursor.Value);

      var userId = HttpContext.GetAuthenticatedUser();
      DataSetQueryResult result;

      using (var session = BslDataSessionFactory.GetDataSession(userId))
      using (var transaction = session.CreateTransaction())
      {
        result = query.DataSet.Query(userId, query, queryCursor, null, session);

        transaction.Commit();
      }

      return new JsonSerializerResult(result, CreateSerializer());
    }

    [Route("datasets/import")]
    [HttpPost]
    public ActionResult StartImport()
    {
      Request.InputStream.Position = 0;

      var dataSetService = DataServices.Resolve<IDataSetService>();

      string key = dataSetService.ImportManager.WriteFile(Request.InputStream);

      var request = new JobBuilder()
        .Queue(dataSetService.Configuration.Queue)
        .Handler(typeof(DataSetImportJob))
        .Parameters(new DataSetImportJobRequest
        {
          FileId = key
        })
        .PersistenceMode(PersistenceMode.Persist)
        .Build();

      var jobId = DataServices.Resolve<SchedulerClient>().Post(
        JobHandler.GetJobUserId(HttpContext.GetAuthenticatedUser()),
        request);

      return new JsonSerializerResult(new
      {
        JobId = jobId
      });
    }

    [Route("datasets/import/progress")]
    [HttpGet]
    public ActionResult GetImportProgress(Guid id)
    {
      var job = DataServices.Resolve<IDataSetService>().ImportManager.GetJob(id);
      if (job == null)
        return HttpNotFound("Job not found");

      return new JsonSerializerResult(new
      {
        Status = job.Status.ToString(),
        job.LastProgress,
        job.LastStatus
      });
    }

    [Route("datasets/import/result")]
    [HttpGet]
    public ActionResult GetImportResult(Guid id)
    {
      var dataSetImportManager = DataServices.Resolve<IDataSetService>().ImportManager;

      var job = dataSetImportManager.GetJob(id);
      if (job == null)
        return HttpNotFound("Job not found");
      if (!job.Status.IsCompleted())
        return new HttpStatusCodeResult(HttpStatusCode.NoContent, "Job has not yet completed");
      if (job.Status != JobStatus.Completed)
        return new HttpStatusCodeResult(HttpStatusCode.NoContent, "Job did not complete successfully");

      var response = JsonConvert.DeserializeObject<DataSetImportJobResponse>(job.Result);

      return new FileStreamResult(
        dataSetImportManager.ReadFile(response.FileId),
        "application/json");
    }

    [Route("datasets/import/cancel")]
    [HttpPost]
    public void CancelImport(Guid id)
    {
      DataServices.Resolve<IDataSetService>().ImportManager.CancelJob(id);
    }

    [Route("datasets/import/dismiss")]
    [HttpPost]
    public ActionResult DismissImport(Guid id)
    {
      var dataSetImportManager = DataServices.Resolve<IDataSetService>().ImportManager;

      var job = dataSetImportManager.GetJob(id);
      if (job == null)
        return HttpNotFound("Job not found");

      if (job.Status == JobStatus.Completed)
      {
        var response = JsonConvert.DeserializeObject<DataSetImportJobResponse>(job.Result);

        dataSetImportManager.DeleteFile(response.FileId);
      }

      dataSetImportManager.DismissJob(id);

      return null;
    }

    private T ParseBody<T>()
    {
      Request.InputStream.Position = 0;

      using (var reader = new StreamReader(Request.InputStream))
      using (var json = new JsonTextReader(reader))
      {
        return CreateSerializer().Deserialize<T>(json);
      }
    }

    private static JsonSerializer CreateSerializer()
    {
      return new JsonSerializer
      {
        DateParseHandling = DateParseHandling.None
      };
    }
  }
}

#endif