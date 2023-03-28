using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL.Querying;
using AMCS.PlatformFramework.Entity.Api.Recipe;
using System.Collections.Generic;
using System.Linq;

namespace AMCS.PlatformFramework.Server.Api.Recipe
{
  public class ApiMeasurementService : EntityObjectService<ApiMeasurement>
  {
    public ApiMeasurementService(IEntityObjectAccess<ApiMeasurement> dataAccess)
    : base(dataAccess)
    {
    }

    public override ApiQuery GetApiCollection(ISessionToken userId, ICriteria criteria, bool includeCount, IDataSession existingDataSession = null)
    {
      var nameField = FieldUtils.ExtractFromCriteria(criteria, nameof(ApiMeasurement.Description), FieldComparison.Like);
      IList<ApiMeasurement> dummyData;
      
      if (nameField != null)
      {
        FieldUtils.Validate(nameField, typeof(Like), FieldComparison.Like, nameof(ApiMeasurement.Description));
        dummyData = StockData.GetMeasurements().Where(x => x.Description.ToLower().StartsWith(((Like)nameField.Value).Value.ToLower())).ToList(); 
      } 
      else 
      {
        dummyData = StockData.GetMeasurements();
      }
      
      return new ApiQuery(dummyData.Cast<EntityObject>().ToList(), dummyData.Count);
    }
  }
}
