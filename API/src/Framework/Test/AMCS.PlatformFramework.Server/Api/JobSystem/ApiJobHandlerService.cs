using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL.Querying;
using AMCS.JobSystem.Scheduler.Api;
using AMCS.PlatformFramework.Entity.Api.JobSystem;

namespace AMCS.PlatformFramework.Server.Api.JobSystem
{
  public class ApiJobHandlerService : EntityObjectService<ApiJobHandler>
  {
    public ApiJobHandlerService(IEntityObjectAccess<ApiJobHandler> dataAccess)
      : base(dataAccess)
    {
    }

    public override ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null)
    {
      if (criteria.Expressions.Count > 0)
        throw new ArgumentException("Job handler API does not support filters");

      var result = new List<EntityObject>();

      foreach (var jobHandler in Jobs.Client.GetJobHandlers())
      {
        result.Add(SerializeJobHandler(jobHandler));
      }

      return new ApiQuery(result, result.Count);
    }

    private ApiJobHandler SerializeJobHandler(IJobHandlerInfo jobHandler)
    {
      return new ApiJobHandler
      {
        Type = jobHandler.Type,
        DisplayName = jobHandler.DisplayName,
        AllowScheduling = jobHandler.AllowScheduling,
        DuplicateMode = jobHandler.DuplicateMode.ToString(),
        RequestParameterInfo = SerializeRequestParameter(jobHandler.RequestParameterInfo),
        CompatibleQueues = jobHandler.CompatibleQueues.ToList()
      };
    }

    private ApiJobHandlerParameter SerializeRequestParameter(IJobHandlerParameterInfo requestParameter)
    {
      if (requestParameter == null)
        return null;

      return new ApiJobHandlerParameter
      {
        Type = requestParameter.Type,
        Properties = requestParameter.Properties.Select(SerializeProperty).ToList()
      };
    }

    private ApiJobHandlerProperty SerializeProperty(IJobHandlerPropertyInfo property)
    {
      return new ApiJobHandlerProperty
      {
        Name = property.Name,
        DisplayName = property.DisplayName,
        IsRequired = property.IsRequired,
        IsList = property.IsList,
        Type = property.Type.ToString()
      };
    }
  }
}
