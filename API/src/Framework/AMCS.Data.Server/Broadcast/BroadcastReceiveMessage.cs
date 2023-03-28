using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AMCS.Data.Server.Broadcast
{
  public class BroadcastReceiveMessage
  {
    public string Type { get; set; }
    public JRaw Data { get; set; }
  }
}
