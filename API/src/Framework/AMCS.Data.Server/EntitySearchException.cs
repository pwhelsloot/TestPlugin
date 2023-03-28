using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server
{
  [Serializable]
  public class EntitySearchException : Exception
  {
    public EntitySearchException(string message)
      : base(message)
    {
    }

    public EntitySearchException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
