using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Mocking
{
  public class MockDataServicesException : Exception
  {
    public MockDataServicesException()
    {
    }

    public MockDataServicesException(string message)
      : base(message)
    {
    }

    public MockDataServicesException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
