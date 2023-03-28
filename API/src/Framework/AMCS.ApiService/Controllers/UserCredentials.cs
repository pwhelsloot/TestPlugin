using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Controllers
{
  public class UserCredentials
  {
    public string Username { get; set; }

    public string Password { get; set; }

    public string TemporaryToken { get; set; }

    public string PrivateKey { get; set; }
  }
}