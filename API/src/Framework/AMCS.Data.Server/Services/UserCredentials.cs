using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Services
{
  public class UserCredentials : ICredentials
  {
    public string UserName { get; }

    public string Password { get; }

    public UserCredentials(string userName, string password)
    {
      UserName = userName;
      Password = password;
    }
  }
}
