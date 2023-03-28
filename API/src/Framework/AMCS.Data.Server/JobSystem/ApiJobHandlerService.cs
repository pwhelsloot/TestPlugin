﻿using System;
using System.Collections.Generic;
using System.Linq;
using AMCS.Data.Entity;
using AMCS.Data.Entity.JobSystem;
using AMCS.Data.Server.SQL.Querying;
using AMCS.JobSystem.Scheduler.Api;

namespace AMCS.Data.Server.JobSystem
{
  public class ApiJobHandlerService : EntityObjectService<ApiJobHandler>
  {
    public override ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null)
    {
      if (criteria.Expressions.Count > 0)
        throw new ArgumentException("Job handler API does not support filters");

      var result = new List<EntityObject>();
      var jobSystemService = DataServices.Resolve<IJobSystemService>();

      foreach (var jobHandler in jobSystemService.Client.GetJobHandlers())
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
