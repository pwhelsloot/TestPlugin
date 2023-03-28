using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Services
{
  public interface IDataAccessIdService
  {
    Guid? GetGuidById(IDataSession session, Type entityType, int id);
    int? GetIdByGuid(IDataSession session, Type entityType, Guid guid);
  }
}
