using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.TestPlugin.Server.Entity;

namespace AMCS.TestPlugin.Server.User
{
  public interface IUserService : IEntityObjectService<UserEntity>
  {
    UserEntity GetByName(ISessionToken userId, string userName, IDataSession dataSession);
    UserEntity GetByEmailAddress(ISessionToken userId, string emailAddress, IDataSession dataSession);
  }
}
