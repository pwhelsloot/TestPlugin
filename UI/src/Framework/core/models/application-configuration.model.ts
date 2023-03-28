
import { ApplicationInsightsProperties } from '@core-module/models/application-insights-properties.interface';
import { alias } from '@coreconfig/api-dto-alias.function';
import { ApplicationInsightsConfiguration } from '@coremodels/application-insights-configuration.model';
import { DeveloperButtonsConfiguration } from '@coremodels/developer-buttons-configuration.model';
import { MapConfiguration } from '@coremodels/map-configuration.model';
import { FeatureToggleConfiguration } from './feature-toggle-configuration.model';
import { GeneralConfiguration } from './general-configuration.model';

export class ApplicationConfiguration {

    @alias('PollingIntervalSeconds')
    pollingIntervalSeconds: number;

    @alias('CommunicationCountPollingIntervalSeconds')
    communicationCountPollingIntervalSeconds: number;

    @alias('ClickOnceExtensionURLChrome')
    clickOnceExtensionURLChrome: string;

    @alias('ClickOnceExtensionURLFirefox')
    clickOnceExtensionURLFirefox: string;

    @alias('NotificationDurationShort')
    notificationDurationShort: number;

    @alias('NotificationDurationNormal')
    notificationDurationNormal: number;

    @alias('NotificationDurationLong')
    notificationDurationLong: number;

    @alias('MaxJobDocuments')
    maxJobDocuments: number;

    @alias('MaxJobDocumentsPerSave')
    maxJobDocumentsPerSave: number;

    @alias('MaxJobDocumentSizeInMb')
    maxJobDocumentSizeInMb: number;

    @alias('MaxManifestProfileDocumentsPerSave')
    maxManifestProfileDocumentsPerSave: number;

    @alias('MaxManifestProfileDocumentSizeInMb')
    maxManifestProfileDocumentSizeInMb: number;

    @alias('BrregIntegrationEnabled')
    brregIntegrationEnabled: boolean;

    @alias('EnableCasualCustomerTemplateEmailFields')
    enableCasualCustomerTemplateEmailFields: boolean;

    @alias('MapProviderConfiguration')
    mapProviderConfiguration: MapConfiguration;

    @alias('ApplicationInsightsConfiguration')
    appInsights: ApplicationInsightsConfiguration;

    @alias('DeveloperButtonConfiguration')
    buttonConfiguration: DeveloperButtonsConfiguration;

    @alias('FeatureToggleConfiguration')
    featureToggleConfiguration: FeatureToggleConfiguration;

    @alias('GeneralConfiguration')
    generalConfiguration: GeneralConfiguration;

    @alias('MaxServiceAgreementDocuments')
    maxServiceAgreementDocuments: number;

    @alias('MaxServiceAgreementDocumentSizeInMb')
    maxServiceAgreementDocumentSizeInMb: number;

    @alias('MaxHealthAndSafetyDocumentSizeInMb')
    maxHealthAndSafetyDocumentSizeInMb: number;

    @alias('MaxHealthAndSafetyDocuments')
    maxHealthAndSafetyDocuments: number;

    @alias('EnableHierarchyPrice')
    enableHierarchyPrice: boolean;

    @alias('DisableZoneFeature')
    disableZoneFeature: boolean;

    getAppInsightProperties(): ApplicationInsightsProperties {
        return {
            pollingIntervalSeconds: this.pollingIntervalSeconds,
            communicationCountPollingIntervalSeconds: this.communicationCountPollingIntervalSeconds,
            clickOnceExtensionURLChrome: this.clickOnceExtensionURLChrome,
            clickOnceExtensionURLFirefox: this.clickOnceExtensionURLFirefox,
            googleMapsUrl: this.mapProviderConfiguration.googleMapsUrl,
            hereMapsUrl: this.mapProviderConfiguration.hereMapsUrl,
            instrumentationKey: this.appInsights.instrumentationKey
        };
    }
}
