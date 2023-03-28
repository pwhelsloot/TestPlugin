using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.JobSystem;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets
{
  public class DataSetImportJob : JobHandler<DataSetImportJobRequest, DataSetImportJobResponse>
  {
    protected override DataSetImportJobResponse Execute(IJobContext context, ISessionToken userId, DataSetImportJobRequest request)
    {
      var service = DataServices.Resolve<IDataSetService>();
      var serializer = new JsonSerializer { DateParseHandling = DateParseHandling.None };

      var import = ReadImport(request.FileId, service, serializer);

      var result = service.Import(userId, import, new DataSetImportJobProgress(context));

      var fileId = WriteImportResult(result, service, serializer);

      return new DataSetImportJobResponse
      {
        FileId = fileId
      };
    }

    private DataSetImport ReadImport(string fileId, IDataSetService service, JsonSerializer serializer)
    {
      try
      {
        using (var stream = service.ImportManager.ReadFile(fileId))
        using (var reader = new StreamReader(stream))
        using (var json = new JsonTextReader(reader))
        {
          return serializer.Deserialize<DataSetImport>(json);
        }
      }
      finally
      {
        service.ImportManager.DeleteFile(fileId);
      }
    }

    private string WriteImportResult(DataSetImportResult result, IDataSetService service, JsonSerializer serializer)
    {
      using (var stream = new MemoryStream())
      {
        using (var writer = new StreamWriter(stream, Encoding.UTF8, 4096, true))
        using (var json = new JsonTextWriter(writer))
        {
          serializer.Serialize(json, result);
        }

        stream.Position = 0;

        return service.ImportManager.WriteFile(stream);
      }
    }
  }
}
