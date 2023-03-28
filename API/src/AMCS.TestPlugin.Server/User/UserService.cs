using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.TestPlugin.Server.Entity;
using AMCS.TestPlugin.Server.User.Data;

namespace AMCS.TestPlugin.Server.User
{
  public class UserService : EntityObjectService<UserEntity>, IUserService
  {
    private new IUserAccess DataAccess => (IUserAccess)base.DataAccess;

    public UserService(IEntityObjectAccess<UserEntity> dataAccess)
      : base(dataAccess)
    {
    }

    public UserEntity GetByName(ISessionToken userId, string userName, IDataSession dataSession)
    {
      return DataAccess.GetByName(dataSession, userName);
    }

    public UserEntity GetByEmailAddress(ISessionToken userId, string emailAddress, IDataSession dataSession)
    {
      return DataAccess.GetByEmailAddress(dataSession, emailAddress);
    }
  }
}
