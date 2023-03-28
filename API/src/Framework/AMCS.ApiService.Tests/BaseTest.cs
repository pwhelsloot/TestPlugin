using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Tests
{
  public class BaseTest
  {
    protected BaseTest()
    {
      TestServiceSetup.Setup();
    }
  }
}