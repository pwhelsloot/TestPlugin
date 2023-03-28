using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.Configuration;

namespace AMCS.ApiService.Controllers.Responses
{
  public class ApplicationConfiguration : IPlatformUIApplicationConfiguration
  {
    private readonly IPlatformUIApplicationConfiguration wrapped;

    public ApplicationConfiguration(IPlatformUIApplicationConfiguration wrapped)
    {
      this.wrapped = wrapped;
    }

    public int? PollingIntervalSeconds => wrapped.PollingIntervalSeconds;

    public string ClickOnceExtensionURLChrome => wrapped.ClickOnceExtensionURLChrome;

    public string ClickOnceExtensionURLFirefox => wrapped.ClickOnceExtensionURLFirefox;

    public int? NotificationDurationShort => wrapped.NotificationDurationShort;

    public int? NotificationDurationNormal => wrapped.NotificationDurationNormal;

    public int? NotificationDurationLong => wrapped.NotificationDurationLong;

    public int? CommunicationCountPollingIntervalSeconds => wrapped.CommunicationCountPollingIntervalSeconds;

    public bool CanCancelCallLog => wrapped.CanCancelCallLog;

    public int? MaxJobDocuments => wrapped.MaxJobDocuments;

    public int? MaxJobDocumentsPerSave => wrapped.MaxJobDocumentsPerSave;

    public int? MaxJobDocumentSizeInMb => wrapped.MaxJobDocumentSizeInMb;

    public int? DurationToDisplayCallLogNotification => wrapped.DurationToDisplayCallLogNotification;

    public int? DefaultCustomerNoteCommunicationType => wrapped.DefaultCustomerNoteCommunicationType;

    public int? MaxManifestProfileDocumentsPerSave => wrapped.MaxManifestProfileDocumentsPerSave;

    public decimal? MaxManifestProfileDocumentSizeInMb => wrapped.MaxManifestProfileDocumentSizeInMb;

    public bool BrregIntegrationEnabled => wrapped.BrregIntegrationEnabled;

    public bool EnableCasualCustomerTemplateEmailFields => wrapped.EnableCasualCustomerTemplateEmailFields;

    public int? MaxServiceAgreementDocuments => wrapped.MaxServiceAgreementDocuments;

    public decimal? MaxServiceAgreementDocumentSizeInMb => wrapped.MaxServiceAgreementDocumentSizeInMb;

    public int? MaxHealthAndSafetyDocumentSizeInMb => wrapped.MaxHealthAndSafetyDocumentSizeInMb;

    public int? MaxHealthAndSafetyDocuments => wrapped.MaxHealthAndSafetyDocuments;

    public string WasteTransferNoteBiReportPath => wrapped.WasteTransferNoteBiReportPath;

    public bool EnableHierarchyPrice => wrapped.EnableHierarchyPrice;

    public bool DisableZoneFeature => wrapped.DisableZoneFeature;

    public bool ManageAPinWebUI => wrapped.ManageAPinWebUI;
  }
}
