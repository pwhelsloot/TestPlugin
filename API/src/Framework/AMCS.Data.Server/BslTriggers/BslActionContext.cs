using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.BslTriggers
{
  public class BslActionContext : IBslActionContext
  {
    private IDictionary<string, object> _context = new Dictionary<string, object>();

    public IDictionary<string, object> Context => _context;
  }
}
