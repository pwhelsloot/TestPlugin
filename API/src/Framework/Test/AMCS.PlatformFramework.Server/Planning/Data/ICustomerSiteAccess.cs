using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server;
using AMCS.PlatformFramework.Entity;

namespace AMCS.PlatformFramework.Server.Planning.Data
{
  public interface ICustomerSiteAccess : IEntityObjectAccess<CustomerSiteEntity>
  {
    string GetLocationTimeZoneId(IDataSession dataSession, int customerSiteId);
  }
}
