using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL;
using AMCS.PlatformFramework.Entity;

namespace AMCS.PlatformFramework.Server.User.Data.SQL
{
  public class SQLUserAccess : SQLEntityObjectAccess<UserEntity>, IUserAccess
  {
    public UserEntity GetByName(IDataSession dataSession, string userName)
    {
      return dataSession.Query("select * from [User] where [UserName] = @UserName")
        .Set("@UserName", userName)
        .Execute()
        .SingleOrDefault<UserEntity>();
    }

    public UserEntity GetByEmailAddress(IDataSession dataSession, string emailAddress)
    {
      return dataSession.Query("select * from [User] where [EmailAddress] = @EmailAddress")
        .Set("@EmailAddress", emailAddress)
        .Execute()
        .SingleOrDefault<UserEntity>();
    }
  }
}
