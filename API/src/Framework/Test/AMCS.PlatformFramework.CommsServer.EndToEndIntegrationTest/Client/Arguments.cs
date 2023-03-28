using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace AMCS.PlatformFramework.CommsServer.EndToEndIntegrationTest.Client
{
  public class Arguments
  {
    [Option("endpoint", HelpText = "Endpoint where the Comms Server at", Required = true)]
    public string Endpoint { get; set; }
  }
}
