using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.Configuration;

namespace AMCS.ApiService.Controllers.Responses
{
  public class FeatureToggleConfiguration : IPlatformUIFeatureToggleConfiguration
  {
    private readonly IPlatformUIFeatureToggleConfiguration wrapped;

    public FeatureToggleConfiguration(IPlatformUIFeatureToggleConfiguration wrapped)
    {
      this.wrapped = wrapped;
    }

    public bool ShowFeatureCustomerService => wrapped.ShowFeatureCustomerService;

    public bool ShowFeatureRouting => wrapped.ShowFeatureRouting;

    public bool ShowFeatureScale => wrapped.ShowFeatureScale;

    public bool ShowFeatureMaterials => wrapped.ShowFeatureMaterials;

    public bool ShowFeatureEquipmentInventory => wrapped.ShowFeatureEquipmentInventory;

    public bool ShowFeatureAccounting => wrapped.ShowFeatureAccounting;

    public bool ShowFeaturePricesAndProducts => wrapped.ShowFeaturePricesAndProducts;

    public bool ShowFeatureReportsAnalytics => wrapped.ShowFeatureReportsAnalytics;

    public bool ShowFeatureSettings => wrapped.ShowFeatureSettings;

    public bool ShowDesignLibrary => wrapped.ShowDesignLibrary;

    public bool ShowCustomerSiteHealthAndSafetyForms => wrapped.ShowCustomerSiteHealthAndSafetyForms;

    public bool ShowCustomerPaidToDateBrowser => wrapped.ShowCustomerPaidToDateBrowser;

    public bool ShowSiteOrderBrowserSoldWeight => wrapped.ShowSiteOrderBrowserSoldWeight;

    public bool ShowFeatureExtensibility => wrapped.ShowFeatureExtensibility;

  }
}
