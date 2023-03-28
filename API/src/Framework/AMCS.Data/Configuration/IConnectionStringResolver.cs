using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Configuration
{
  public interface IConnectionStringResolver
  {
    IConnectionString GetConnectionString(string name);
  }
}