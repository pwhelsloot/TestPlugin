
import { Injectable } from '@angular/core';
import { ClassBuilder } from '@core-module/helpers/dto/class-builder';
import { FeatureToggleConfiguration } from '@core-module/models/feature-toggle-configuration.model';
import { ApiResourceResponse } from '@coreconfig/api-resource-response.interface';
import { ApiOptionsEnum } from '@coremodels/api/api-options.enum';
import { ApiRequest } from '@coremodels/api/api-request.model';
import { ApplicationConfiguration } from '@coremodels/application-configuration.model';
import { ApplicationInsightsConfiguration } from '@coremodels/application-insights-configuration.model';
import { DeveloperButtonsConfiguration } from '@coremodels/developer-buttons-configuration.model';
import { GeneralConfiguration } from '@coremodels/general-configuration.model';
import { MapConfiguration } from '@coremodels/map-configuration.model';
import { ApplicationConfigurationFormatter } from '@coreservices/config/data/application-configuration.formatter';
import { ErpApiService } from '@coreservices/erp-api.service';

@Injectable({ providedIn: 'root' })
export class ApplicationConfigurationServiceData {

    constructor(private erpApiService: ErpApiService) {
    }

    getConfiguration() {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['applicationUISettings'];
        apiRequest.apiOptions = ApiOptionsEnum.core;
        return this.erpApiService.getRequestHandleError<ApiResourceResponse>(apiRequest, this.configMap, true, this.handleError.bind(this));
    }

    private configMap(response: ApiResourceResponse) {
        const config: ApplicationConfiguration = ClassBuilder.buildFromApiResourceResponse<ApplicationConfiguration>(response['ApplicationConfiguration'], ApplicationConfiguration);
        config.appInsights = ClassBuilder.buildFromApiResourceResponse<ApplicationInsightsConfiguration>(response['ApplicationInsightsConfiguration'], ApplicationInsightsConfiguration);
        config.buttonConfiguration = ClassBuilder.buildFromApiResourceResponse<DeveloperButtonsConfiguration>(response['DeveloperButtonsConfiguration'], DeveloperButtonsConfiguration);
        config.featureToggleConfiguration = ClassBuilder.buildFromApiResourceResponse<FeatureToggleConfiguration>(response['FeatureToggleConfiguration'], FeatureToggleConfiguration);
        config.mapProviderConfiguration = ClassBuilder.buildFromApiResourceResponse<MapConfiguration>(response['MapProviderConfiguration'], MapConfiguration);
        config.generalConfiguration = ClassBuilder.buildFromApiResourceResponse<GeneralConfiguration>(response['GeneralConfiguration'], GeneralConfiguration);
        ApplicationConfigurationFormatter.default(config);
        return config;
    }

    // Want to throw it as I know i have a catch block in the service above this.
    private handleError(error: string) {
        throw (error);
    }
}
