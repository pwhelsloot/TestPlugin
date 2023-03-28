using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.Configuration;

namespace AMCS.ApiService.Controllers.Responses
{
  public class DeveloperConfiguration : IPlatformUIDeveloperConfiguration
  {
    private readonly IPlatformUIDeveloperConfiguration wrapped;

    public DeveloperConfiguration(IPlatformUIDeveloperConfiguration wrapped)
    {
      this.wrapped = wrapped;
    }

    public bool ShowCommunicationsAdd => wrapped.ShowCommunicationsAdd;

    public bool ShowCommunicationsDropdown => wrapped.ShowCommunicationsDropdown;

    public bool ShowContactDelete => wrapped.ShowContactDelete;

    public bool ShowContractsAdd => wrapped.ShowContractsAdd;

    public bool ShowContractsExpander => wrapped.ShowContractsExpander;

    public bool ShowCustomerAdd => wrapped.ShowCustomerAdd;

    public bool ShowDashboardOverviewEdit => wrapped.ShowDashboardOverviewEdit;

    public bool ShowHeaderDevTools => wrapped.ShowHeaderDevTools;

    public bool ShowHeaderERPAbout => wrapped.ShowHeaderERPAbout;

    public bool ShowServiceLocationsAdd => wrapped.ShowServiceLocationsAdd;

    public bool ShowServiceLocationsDelete => wrapped.ShowServiceLocationsDelete;

    public bool ShowMissedCollectionLogEvent => wrapped.ShowMissedCollectionLogEvent;

    public bool ShowMissedCollectionCreateCallout => wrapped.ShowMissedCollectionCreateCallout;

    public bool ShowFrequentDashboardActions => wrapped.ShowFrequentDashboardActions;

    public bool ShowSiteOrderActionMenu => wrapped.ShowSiteOrderActionMenu;

    public bool ShowSiteOrderAdd => wrapped.ShowSiteOrderAdd;

    public bool ShowLoadReceiving => wrapped.ShowLoadReceiving;

    public bool ShowAddUnplannedLoads => wrapped.ShowAddUnplannedLoads;
  }
}
