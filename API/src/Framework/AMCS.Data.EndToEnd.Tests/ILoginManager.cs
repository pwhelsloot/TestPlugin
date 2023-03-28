using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EndToEnd.Tests
{
  public interface ILoginManager
  {
    ISessionToken Login();

    string GeneratePrivateKey(ISessionToken sessionKey);
  }
}
