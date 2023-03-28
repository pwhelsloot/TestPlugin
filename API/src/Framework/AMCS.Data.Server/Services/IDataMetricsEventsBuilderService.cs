using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Services
{
  public interface IDataMetricsEventsBuilderService
  {
    void Add(IDataMetricsEvents events);

    IDataMetricsEvents Build();
  }
}
