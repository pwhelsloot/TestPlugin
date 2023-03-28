using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server;
using AMCS.PlatformFramework.Entity;

namespace AMCS.PlatformFramework.Server.User.Data
{
  public interface IUserAccess : IEntityObjectAccess<UserEntity>
  {
    UserEntity GetByName(IDataSession dataSession, string userName);
    UserEntity GetByEmailAddress(IDataSession dataSession, string emailAddress);
  }
}
