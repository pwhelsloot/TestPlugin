using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL;
using AMCS.Data.Server.SQL.Querying;
using AMCS.PlatformFramework.Entity;

namespace AMCS.PlatformFramework.Server.Planning.Data.SQL
{
  public class SQLJobAccess : SQLEntityObjectAccess<JobEntity>
  {
    protected override ISQLReadable GetData(IDataSession dataSession, bool all, int id, bool includeDeleted, string orderByCsvFieldNames = null)
    {
      return dataSession.Query(@"
          select j.*, l.TimeZoneId as LocationTimeZoneId
          from Job j
            left join CustomerSite cs on j.CustomerSiteId = cs.CustomerSiteId
            left join Location l on cs.LocationId = l.LocationId
          where
            (@All = 1 or j.JobId = @Id)
        ")
        .Set("@All", all)
        .Set("@Id", id)
        .Execute();
    }

    public override int? Save(IDataSession dataSession, ISessionToken userId, JobEntity entity, string[] restrictToFields = null)
    {
      // We unconditionally overwrite the time zone ID to ensure the conversion is correct.
      entity.LocationTimeZoneId = DataAccessManager.GetAccessForInterface<ICustomerSiteAccess>().GetLocationTimeZoneId(dataSession, entity.CustomerSiteId);

      return base.Save(dataSession, userId, entity, restrictToFields);
    }

    public override ISQLReadable GetByCriteria(IDataSession dataSession, ISessionToken userId, ICriteria criteria, CriteriaQueryType queryType)
    {
      var query = SQLQueryBuilder.FromCriteria(criteria, queryType);

      if (queryType != CriteriaQueryType.Count)
      {
        query.SetSelect("Job.*, Location.TimeZoneId as LocationTimeZoneId");

        query.SetFrom(@"
          Job
            left join CustomerSite on Job.CustomerSiteId = CustomerSite.CustomerSiteId
            left join Location on CustomerSite.LocationId = Location.LocationId
        ");
      }

      return dataSession.Query(query).Execute();
    }
  }
}
