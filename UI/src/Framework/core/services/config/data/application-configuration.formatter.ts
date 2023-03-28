import { FeatureToggleConfiguration } from '@core-module/models/feature-toggle-configuration.model';
import { ApplicationConfiguration } from '@coremodels/application-configuration.model';
import { ApplicationInsightsConfiguration } from '@coremodels/application-insights-configuration.model';
import { DeveloperButtonsConfiguration } from '@coremodels/developer-buttons-configuration.model';
import { GeneralConfiguration } from '@coremodels/general-configuration.model';
import { MapConfiguration } from '@coremodels/map-configuration.model';
import { AmcsMapDisplayProvider } from '@shared-module/amcs-leaflet-map/enums/amcs-map-display-provider';

export class ApplicationConfigurationFormatter {

    static default(config: ApplicationConfiguration): ApplicationConfiguration {
        config.pollingIntervalSeconds = this.setDefault(config.pollingIntervalSeconds, 900);
        config.communicationCountPollingIntervalSeconds = this.setDefault(config.communicationCountPollingIntervalSeconds, 300);
        // eslint-disable-next-line max-len
        config.clickOnceExtensionURLChrome = this.setDefault(config.clickOnceExtensionURLChrome, 'https://chrome.google.com/webstore/detail/meta4-clickonce-launcher/jkncabbipkgbconhaajbapbhokpbgkdc?hl=en');
        config.clickOnceExtensionURLFirefox = this.setDefault(config.clickOnceExtensionURLFirefox, 'https://addons.mozilla.org/en-GB/firefox/addon/meta4clickoncelauncher/');
        config.notificationDurationShort = this.setDefault(config.notificationDurationShort, 500);
        config.notificationDurationNormal = this.setDefault(config.notificationDurationNormal, 1500);
        config.notificationDurationLong = this.setDefault(config.notificationDurationLong, 3000);
        config.mapProviderConfiguration = this.setDefault(config.mapProviderConfiguration, new MapConfiguration());
        // eslint-disable-next-line max-len
        config.mapProviderConfiguration.googleMapsUrl = this.setDefault(config.mapProviderConfiguration.googleMapsUrl, 'http://{s}.google.com/vt/lyrs=m&amp;x={x}&amp;y={y}&amp;z={z}');
        // eslint-disable-next-line max-len
        config.mapProviderConfiguration.hereMapsUrl = this.setDefault(config.mapProviderConfiguration.hereMapsUrl, 'http://1.base.maps.cit.api.here.com/maptile/2.1/maptile/newest/normal.day/{z}/{x}/{y}/256/png8?app_id=PrT4jl18ziV9P3NB6yBs&amp;app_code=WC-Vbk8rztbJsEeCd8u4mA');
        config.mapProviderConfiguration.mapDisplayProvider = this.setDefault(config.mapProviderConfiguration.mapDisplayProvider, AmcsMapDisplayProvider.Google);
        config.appInsights = this.setDefault(config.appInsights, new ApplicationInsightsConfiguration());
        config.appInsights.instrumentationKey = this.setDefault(config.appInsights.instrumentationKey, 'feecd04c-e263-436c-a7c9-f04b6b3a1525');
        config.appInsights.appInsightsOn = this.setDefault(config.appInsights.appInsightsOn, true);
        config.appInsights.consoleLoggingOn = this.setDefault(config.appInsights.consoleLoggingOn, true);
        config.appInsights.trackingOptionsPageViewLoadTimesUrlBased = this.setDefault(config.appInsights.trackingOptionsPageViewLoadTimesUrlBased, true);
        config.appInsights.trackingOptionspageViewLoadTimesManualComponentNameBased = this.setDefault(config.appInsights.trackingOptionspageViewLoadTimesManualComponentNameBased, true);
        config.appInsights.trackingOptionsComponentLifecycle = this.setDefault(config.appInsights.trackingOptionsComponentLifecycle, false);
        config.appInsights.trackingOptionsUiInteractionEvent = this.setDefault(config.appInsights.trackingOptionsUiInteractionEvent, false);
        config.appInsights.trackingOptionsLoginEvents = this.setDefault(config.appInsights.trackingOptionsLoginEvents, true);
        config.appInsights.trackingOptionsDesktopEvents = this.setDefault(config.appInsights.trackingOptionsDesktopEvents, true);
        config.appInsights.trackingOptionsGlobalSearchEvents = this.setDefault(config.appInsights.trackingOptionsGlobalSearchEvents, true);
        config.appInsights.trackingOptionsGlobalSearchTerms = this.setDefault(config.appInsights.trackingOptionsGlobalSearchTerms, true);
        config.appInsights.trackingOptionsCustomerContactEvents = this.setDefault(config.appInsights.trackingOptionsCustomerContactEvents, true);
        config.appInsights.trackingOptionsCustomerCalendarEvents = this.setDefault(config.appInsights.trackingOptionsCustomerCalendarEvents, true);
        config.appInsights.trackingOptionsCustomerOperationEvents = this.setDefault(config.appInsights.trackingOptionsCustomerOperationEvents, true);
        config.appInsights.trackingOptionsCustomerServiceLocationEvents = this.setDefault(config.appInsights.trackingOptionsCustomerServiceLocationEvents, true);
        config.buttonConfiguration = this.setDefault(config.buttonConfiguration, new DeveloperButtonsConfiguration());
        config.buttonConfiguration.showCommunicationsDropdown = this.setDefault(config.buttonConfiguration.showCommunicationsDropdown, false);
        config.buttonConfiguration.showServiceLocationsAdd = this.setDefault(config.buttonConfiguration.showServiceLocationsAdd, false);
        config.buttonConfiguration.showServiceLocationsDelete = this.setDefault(config.buttonConfiguration.showServiceLocationsDelete, false);
        config.buttonConfiguration.showContractsAdd = this.setDefault(config.buttonConfiguration.showContractsAdd, false);
        config.buttonConfiguration.showContractsExpander = this.setDefault(config.buttonConfiguration.showContractsExpander, false);
        config.buttonConfiguration.showHeaderERPAbout = this.setDefault(config.buttonConfiguration.showHeaderERPAbout, false);
        config.buttonConfiguration.showHeaderDevtools = this.setDefault(config.buttonConfiguration.showHeaderDevtools, false);
        config.buttonConfiguration.showSiteOrderAdd = this.setDefault(config.buttonConfiguration.showSiteOrderAdd, false);
        config.buttonConfiguration.showFinanceEdit = this.setDefault(config.buttonConfiguration.showFinanceEdit, false);
        config.buttonConfiguration.showContactDelete = this.setDefault(config.buttonConfiguration.showContactDelete, false);
        config.buttonConfiguration.showMissedCollectionLogEvent = this.setDefault(config.buttonConfiguration.showMissedCollectionLogEvent, true);
        config.buttonConfiguration.showMissedCollectionCreateCallout = this.setDefault(config.buttonConfiguration.showMissedCollectionCreateCallout, true);
        config.buttonConfiguration.showFrequentDashboardActions = this.setDefault(config.buttonConfiguration.showFrequentDashboardActions, true);
        config.buttonConfiguration.showSiteOrderActionMenu = this.setDefault(config.buttonConfiguration.showSiteOrderActionMenu, true);
        config.buttonConfiguration.showLoadReceiving = this.setDefault(config.buttonConfiguration.showLoadReceiving, false);
        config.buttonConfiguration.showAddUnplannedLoads = this.setDefault(config.buttonConfiguration.showAddUnplannedLoads, false);
        config.featureToggleConfiguration = this.setDefault(config.featureToggleConfiguration, new FeatureToggleConfiguration());
        config.featureToggleConfiguration.showFeatureCustomerService = this.setDefault(config.featureToggleConfiguration.showFeatureCustomerService, false);
        config.featureToggleConfiguration.showFeatureCustomerService = this.setDefault(config.featureToggleConfiguration.showFeatureCustomerService, false);
        config.featureToggleConfiguration.showFeatureRouting = this.setDefault(config.featureToggleConfiguration.showFeatureRouting, false);
        config.featureToggleConfiguration.showFeatureScale = this.setDefault(config.featureToggleConfiguration.showFeatureScale, false);
        config.featureToggleConfiguration.showFeatureMaterials = this.setDefault(config.featureToggleConfiguration.showFeatureMaterials, false);
        config.featureToggleConfiguration.showFeatureEquipmentInventory = this.setDefault(config.featureToggleConfiguration.showFeatureEquipmentInventory, false);
        config.featureToggleConfiguration.showFeatureAccounting = this.setDefault(config.featureToggleConfiguration.showFeatureAccounting, false);
        config.featureToggleConfiguration.showFeaturePricesAndProducts = this.setDefault(config.featureToggleConfiguration.showFeaturePricesAndProducts, false);
        config.featureToggleConfiguration.showFeatureReportsAnalytics = this.setDefault(config.featureToggleConfiguration.showFeatureReportsAnalytics, false);
        config.featureToggleConfiguration.showFeatureSettings = this.setDefault(config.featureToggleConfiguration.showFeatureSettings, false);
        config.featureToggleConfiguration.showDesignLibrary = this.setDefault(config.featureToggleConfiguration.showDesignLibrary, false);
        config.featureToggleConfiguration.showFeatureExtensibility = this.setDefault(config.featureToggleConfiguration.showFeatureExtensibility, false);
        config.generalConfiguration = this.setDefault(config.generalConfiguration, new GeneralConfiguration());
        config.generalConfiguration.hideSiteOrderActions = this.setDefault(config.generalConfiguration.hideSiteOrderActions, true);
        config.generalConfiguration.isTabControlReadOnly = this.setDefault(config.generalConfiguration.isTabControlReadOnly, false);
        config.generalConfiguration.canCancelCallLog = this.setDefault(config.generalConfiguration.canCancelCallLog, true);
        config.generalConfiguration.durationToDisplayCallLogNotification = this.setDefault(config.generalConfiguration.durationToDisplayCallLogNotification, 10000);
        config.generalConfiguration.maximumPricesForScaleTicket = this.setDefault(config.generalConfiguration.maximumPricesForScaleTicket, 10);

        config.enableCasualCustomerTemplateEmailFields = this.setDefault(config.enableCasualCustomerTemplateEmailFields, false);
        config.maxServiceAgreementDocuments = this.setDefault(config.maxServiceAgreementDocuments, 10);
        config.maxServiceAgreementDocumentSizeInMb = this.setDefault(config.maxServiceAgreementDocumentSizeInMb, 0.25);
        config.enableHierarchyPrice = this.setDefault(config.enableHierarchyPrice, false);
        config.disableZoneFeature = this.setDefault(config.disableZoneFeature, false);
        return config;
    }

    private static setDefault(value: any, defaultValue: any) {
        if (value != null) {
            return value;
        } else {
            return defaultValue;
        }
    }
}
