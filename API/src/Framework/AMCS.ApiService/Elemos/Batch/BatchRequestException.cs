using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Elemos.Batch
{
  internal class BatchRequestException : Exception
  {
    public BatchRequestException()
    {
    }

    public BatchRequestException(string message)
      : base(message)
    {
    }

    public BatchRequestException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
