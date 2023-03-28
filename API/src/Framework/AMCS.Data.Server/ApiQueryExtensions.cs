using AMCS.Data.Entity;

namespace AMCS.Data.Server
{
  public static class ApiQueryExtensions
  {
    public static ApiQuery WithSummary(this ApiQuery apiQuery, EntityObject summary)
    { 
      return new ApiQuery(apiQuery.Entities, apiQuery.Count, summary);
    }
  }
}