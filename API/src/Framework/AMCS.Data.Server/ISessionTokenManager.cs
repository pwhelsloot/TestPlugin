using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server
{
  public interface ISessionTokenManager
  {
    SessionTokenStatus TryDeserialize(string token, out ISessionToken sessionToken);

    string Serialize(ISessionToken token);
  }
}