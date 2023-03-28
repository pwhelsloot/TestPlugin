using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.PlatformCredentials
{
  public interface IPlatformCredentialsTokenManager
  {
    PlatformCredentials Deserialize(string token);

    string Serialize(PlatformCredentials platformCredentials);
  }
}
