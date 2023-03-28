using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Services
{
  public class PrivateKeyCredentials : ICredentials
  {
    public string PrivateKey { get; }

    public PrivateKeyCredentials(string privateKey)
    {
      PrivateKey = privateKey;
    }
  }
}
