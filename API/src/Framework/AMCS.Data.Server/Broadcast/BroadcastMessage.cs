using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Broadcast
{
  public class BroadcastMessage<T>
  {
    public string Type { get; set; }
    public T Data { get; set; }
  }
}
