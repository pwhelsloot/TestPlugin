using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server;
using AMCS.TestPlugin.Server.Entity;

namespace AMCS.TestPlugin.Server.User.Data
{
  public interface IUserAccess : IEntityObjectAccess<UserEntity>
  {
    UserEntity GetByName(IDataSession dataSession, string userName);
    UserEntity GetByEmailAddress(IDataSession dataSession, string emailAddress);
  }
}
