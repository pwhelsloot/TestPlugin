using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
#if NETFRAMEWORK
using System.ServiceModel.Configuration;
#endif
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AMCS.Data.Configuration
{
  internal class ServiceRootResolver : IServiceRootResolver
  {
    private readonly Dictionary<string, EndpointAddress> endpoints = new Dictionary<string, EndpointAddress>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> serviceRoots;

    public string ServiceRoot { get; }

    public ServiceRootResolver(IServiceRootsConfiguration configuration, string serviceRootName)
    {
      serviceRoots = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      foreach (var serviceRoot in configuration.GetServiceRoots())
      {
        if (!string.IsNullOrEmpty(serviceRoot.Url))
        {
          serviceRoots.Add(serviceRoot.Name, $"{serviceRoot.Url.TrimEnd('/')}/");
        }
      }

      ServiceRoot = serviceRoots[serviceRootName];

#if NETFRAMEWORK
      var clientSection = (ClientSection)ConfigurationManager.GetSection("system.serviceModel/client");

      foreach (var channelEndpoint in clientSection.Endpoints.Cast<ChannelEndpointElement>())
      {
        string address = channelEndpoint.Address.ToString();
        if (!Uri.IsWellFormedUriString(address, UriKind.Absolute))
        {
          address = Regex.Replace(address, "{([a-z0-9]+)}", p => this.Replace(p, serviceRoots), RegexOptions.IgnoreCase);

          if (!Uri.IsWellFormedUriString(address, UriKind.Absolute))
          {
            continue;
          }
        }

        this.endpoints.Add(channelEndpoint.Name, new EndpointAddress(address));
      }
#endif

      // This is not nice, but there's really no way around this. AMCS.Weights.Client
      // uses the old implementation directly. Since we can't change that library, we
      // need to support the old mechanism. Because of this, we're writing service roots
      // into the AppSettings so they can still be resolved.
      foreach (var serviceRoot in configuration.GetServiceRoots())
      {
        ConfigurationManager.AppSettings[serviceRoot.Name] = serviceRoot.Url;
      }
    }

    public string GetServiceRoot(string name)
    {
      return serviceRoots[name];
    }

    public string GetProjectServiceRoot()
    {
      var projectServiceRoot = GetServiceRoot(DataServices.Resolve<IProjectConfiguration>().ServiceRootName);
      return projectServiceRoot;
    }

#if NETFRAMEWORK
    public EndpointAddress ResolveEndpointAddress(string name)
    {
      return this.endpoints[name];
    }

    private string Replace(Match match, Dictionary<string, string> serviceRoots)
    {
      if (serviceRoots.TryGetValue(match.Groups[1].Value, out string url))
      {
        return url;
      }

      return match.Value;
    }
#endif
  }
}
