using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL.Querying;
using AMCS.PlatformFramework.Entity.Api.Recipe;
using System.Collections.Generic;
using System.Linq;

namespace AMCS.PlatformFramework.Server.Api.Recipe
{
  public class ApiMapExampleService : EntityObjectService<ApiMapExample>
  {
    public ApiMapExampleService(IEntityObjectAccess<ApiMapExample> dataAccess)
    : base(dataAccess)
    {
    }

    public override int? Save(ISessionToken userId, ApiMapExample entity, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);

      try
      {
        if (StockData.MapExamples == null)
          StockData.MapExamples = new List<ApiMapExample>();

        if (entity.MapExampleId == null)
        {
          entity.MapExampleId = StockData.MapExamples.Count + 1;
          StockData.MapExamples.Add(entity);
        }
        else
        {
          var data = StockData.MapExamples.FirstOrDefault(x => x.MapExampleId == entity.MapExampleId);
          data.Latitude = entity.Latitude;
          data.Longitude = entity.Longitude;
          data.Description = entity.Description;
        }

        return entity.MapExampleId;
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    public override void Delete(ISessionToken userId, ApiMapExample entity, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);

      try
      {
        if (StockData.MapExamples == null)
          return;

        if (entity.MapExampleId == null)
          return;

        StockData.MapExamples.Remove(entity);
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }

    public override ApiMapExample GetById(ISessionToken userId, int id, IDataSession existingDataSession = null)
    {
      return StockData.MapExamples.FirstOrDefault(x => x.MapExampleId == id);
    }

    public override ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null)
    {
      IDataSession ds = GetDataSession(userId, existingDataSession);

      try
      {
        List<EntityObject> mapExamples = new List<EntityObject>(); 

        var mapExampleIdCriteria = FieldUtils.ExtractFromCriteria(criteria, nameof(ApiMapExample.MapExampleId), FieldComparison.Eq);
        if (mapExampleIdCriteria != null)
        {
          FieldUtils.Validate(mapExampleIdCriteria, typeof(int), FieldComparison.Eq, nameof(ApiMapExample.MapExampleId));
          // This would normally be a ds.GetById<ApiMapExample>(userId, (int)mapExampleIdCriteria.Value); call
          mapExamples.Add(StockData.MapExamples?.FirstOrDefault(x => x.MapExampleId == (int)mapExampleIdCriteria.Value));
        }
        else
        {
          if (StockData.MapExamples != null)
          {
            mapExamples.AddRange(StockData.MapExamples);
          }
        }

        return new ApiQuery(mapExamples, null);
      }
      finally
      {
        DisposeDataSession(ds, existingDataSession != null);
      }
    }
  }
}
