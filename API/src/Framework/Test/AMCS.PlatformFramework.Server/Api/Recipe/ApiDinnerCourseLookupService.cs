using AMCS.Data;
using AMCS.Data.Server;
using AMCS.PlatformFramework.Entity.Api.Recipe;
using System.Collections.Generic;

namespace AMCS.PlatformFramework.Server.Api.Recipe
{
  // RDM - You'd never normally need create a service/sql layer for a lookup, this is just to get test data.
  public class ApiDinnerCourseLookupService : EntityObjectService<ApiDinnerCourseLookup>
  {
    public ApiDinnerCourseLookupService(IEntityObjectAccess<ApiDinnerCourseLookup> dataAccess)
    : base(dataAccess)
        {
        }

    private int idCount = 1;

    public override IList<ApiDinnerCourseLookup> GetAllById(ISessionToken userId, int id, bool includeDeleted, IDataSession existingDataSession = null)
    {
      var items = StockData.GetCourses();
      List<ApiDinnerCourseLookup> courses = new List<ApiDinnerCourseLookup>();
      foreach (var item in items)
      {
        courses.Add(new ApiDinnerCourseLookup()
        {
          DinnerCourseId = idCount++,
          Description = item.Key,
        });
      }
      return courses;
    }
  }
}