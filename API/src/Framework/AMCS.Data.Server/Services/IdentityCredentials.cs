using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Services
{
  public class IdentityCredentials : ICredentials
  {
    public string Identity { get; }

    public string Tenant { get; }

    public IdentityCredentials(string identity) 
    {
      Identity = identity;      
    }

    public IdentityCredentials(string identity, string tenant) 
      : this(identity)
    {
      Tenant = tenant;
    }
  }
}
