using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL;
using AMCS.PlatformFramework.Entity;

namespace AMCS.PlatformFramework.Server.Planning.Data.SQL
{
  public class SQLCustomerSiteAccess : SQLEntityObjectAccess<CustomerSiteEntity>, ICustomerSiteAccess
  {
    public string GetLocationTimeZoneId(IDataSession dataSession, int customerSiteId)
    {
      return dataSession.Query(@"
          select l.TimeZoneId
          from CustomerSite cs left join Location l on cs.LocationId = l.LocationId
          where cs.CustomerSiteId = @CustomerSiteId
        ")
        .Set("@CustomerSiteId", customerSiteId)
        .Execute()
        .SingleScalar<string>();
    }
  }
}
