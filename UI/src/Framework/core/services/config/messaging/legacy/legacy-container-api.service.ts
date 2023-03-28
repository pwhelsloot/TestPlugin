import {
  AppChange2Request,
  AppChange2Response,
  AppRunComponent1FailureResponse,
  AppRunComponent1FailureResult,
  AppRunComponent1SuccessResponse,
  AppRunComponent1SuccessResult,
  AppSupportedFeature1Request,
  AppSupportedFeature1Response,
  AppTitleChange1Request,
  AppTitleChange1Response,
  AppUpdateFeatureConfiguration1Request,
  AppUpdateFeatureConfiguration1Response,
  AppUrlChange1Request,
  AppUrlChange1Response,
  AppUserContext1Request,
  AppUserContext1Response,
  AppUserPreferenceChange1Request,
  AppUserPreferenceChange1Response,
  IPluginToPlatformApi
} from '@amcs/platform-communication';
import { Injectable } from '@angular/core';
import { LegacyContainerAppApiBase } from '@core-module/messaging/legacy-container-api-base.service';
import { CoreMessagingKeys } from '@core-module/messaging/messaging-keys.model';

/**
 * Legacy CoreApi for ContainerApp
 */
@Injectable({ providedIn: 'root' })
export class LegacyCoreAppApiService extends LegacyContainerAppApiBase implements IPluginToPlatformApi {
  // eslint-disable-next-line @typescript-eslint/no-useless-constructor
  constructor() {
    super();
  }

  appChange2(request: AppChange2Request): AppChange2Response {
    this.sendMessage(CoreMessagingKeys.APP_CHANGE_2, request);
    return {} as AppChange2Response;
  }

  appTitleChange1(request: AppTitleChange1Request): AppTitleChange1Response {
    this.sendMessage(CoreMessagingKeys.APP_TITLECHANGE_1, request);
    return {} as AppTitleChange1Response;
  }

  appUrlChange1(request: AppUrlChange1Request): AppUrlChange1Response {
    this.sendMessage(CoreMessagingKeys.APP_URL_CHANGE_1, request);
    return {} as AppUrlChange1Response;
  }

  appUserPreferenceChange1(request: AppUserPreferenceChange1Request): AppUserPreferenceChange1Response {
    this.sendMessage(CoreMessagingKeys.APP_USERPREFERENCE_CHANGE_1, request);
    return {} as AppUserPreferenceChange1Response;
  }

  appSupportedFeatures1(request: AppSupportedFeature1Request): AppSupportedFeature1Response {
    this.sendMessage(CoreMessagingKeys.APP_SUPPORTED_FEATURES_1, request);
    return {} as AppSupportedFeature1Response;
  }

  appUserContextRequest1(request: AppUserContext1Request): Promise<AppUserContext1Response> {
    this.sendMessage(CoreMessagingKeys.APP_USERCONTEXT_REQUEST_1, request);
    return Promise.resolve({} as AppUserContext1Response);
  }

  appRunComponentSuccessResult1(result: AppRunComponent1SuccessResult): AppRunComponent1SuccessResponse {
    return {} as AppRunComponent1SuccessResponse;
  }

  appRunComponentFailureResult1(result: AppRunComponent1FailureResult): AppRunComponent1FailureResponse {
    return {} as AppRunComponent1FailureResponse;
  }

  appUpdateFeatureConfiguration1(request: AppUpdateFeatureConfiguration1Request): AppUpdateFeatureConfiguration1Response {
    return {} as AppUpdateFeatureConfiguration1Response;
  }

  setWindow(applicationName?: string) {
    this.contentWindow = window.parent;
  }
}
