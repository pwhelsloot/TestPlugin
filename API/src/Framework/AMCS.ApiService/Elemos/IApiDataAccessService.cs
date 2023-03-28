using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server;

namespace AMCS.ApiService.Elemos
{
  public interface IApiDataAccessService
  {
    Guid? GetGuidById(IDataSession session, Type entityType, int? id);
  }
}