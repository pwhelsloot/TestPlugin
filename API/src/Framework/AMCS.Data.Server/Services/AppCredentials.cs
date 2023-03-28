using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Services
{
  public class AppCredentials : ICredentials
  {
    public string App { get; }

    public string Tenant { get; }

    public AppCredentials(string app, string tenant)
    {
      App = app;
      Tenant = tenant;
    }
  }
}