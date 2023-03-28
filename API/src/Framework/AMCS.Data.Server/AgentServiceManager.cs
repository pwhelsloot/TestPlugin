using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Configuration;
using AMCS.JobSystem.Agent;

namespace AMCS.Data.Server
{
  internal class AgentServiceManager
  {
    private readonly AgentService agent;

    public AgentServiceManager(AgentService agent, ISetupService setupService)
    {
      this.agent = agent;
      setupService.RegisterSetup(Start, -500);
    }

    public void Start()
    {
      agent.Start();
    }
  }
}
