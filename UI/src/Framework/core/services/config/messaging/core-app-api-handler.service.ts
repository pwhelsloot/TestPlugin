import { ExtensibilityService } from '@amcs-extensibility/host';
import {
  AppChange2Request,
  AppSupportedFeature1Request,
  AppTitleChange1Request,
  AppUrlChange1Request,
  AppUserContext1Request,
  AppUserContext1Response,
  AppUserPreferenceChange1Request,
  PluginToPlatformApi,
  PlatformFeature,
} from '@amcs/platform-communication';
import { Injectable, OnDestroy } from '@angular/core';
import { CoreMessagingKeys } from '@core-module/messaging/messaging-keys.model';
import { environment } from '@environments/environment';
import { ErrorCode } from 'penpal';
import { Subscription } from 'rxjs';
import { filter, take } from 'rxjs/operators';
import { CoreAppFeaturesService } from '../core-app-features.service';
import { ContainerAppApiService } from './container-api-handler';
import { LegacyCoreAppApiService } from './legacy/legacy-container-api.service';
import { SignalRUserContextService } from './signalr-user-context.service';

/**
 * Contains the ContainerAppApi
 */
@Injectable({ providedIn: 'root' })
export class CoreAppApiHandler implements OnDestroy {
  constructor(
    private readonly service: LegacyCoreAppApiService,
    private readonly containerAppApiHandlerService: ContainerAppApiService,
    private readonly featureService: CoreAppFeaturesService,
    private readonly extensibilityService: ExtensibilityService,
    private readonly signalRUserContextService: SignalRUserContextService
  ) {
    this.handleBetterMessagingV1();
    this.listenForExtensibilityResults();
  }

  private legacy: LegacyCoreAppApiService;
  private handler: PluginToPlatformApi;
  private featureSubscription: Subscription;

  ngOnDestroy(): void {
    this.featureSubscription?.unsubscribe();
  }

  /**
   * Initialise Legacy messaging
   */
  registerLegacyMessagingForApplication() {
    this.service.setWindow();
    this.legacy = this.service;
  }

  /**
   * Intialise PenPal messaging
   */
  registerMessagingForApplication() {
    this.containerAppApiHandlerService.connectToPlatform(window.coreServiceRoot, !environment.production).then(
      (child) => {
        this.handler = child;
        this.afterConnectionSuccess();
      },
      (reason: ErrorCode) => {
        console.log(reason);
      }
    );
  }

  /**
   * Notify Core that the page title needs to be changed
   * @param title The new title
   */
  sendAppChangeTitleRequest(title: string) {
    const request = { title } as AppTitleChange1Request;
    this.getApi().appTitleChange1?.(request);
  }

  /**
   * Notify Core that a user preference has been changed
   * @param key
   * @param value
   */
  sendCorePreferenceChangeNotification(key: string, value: string) {
    const request = {
      sourcePrefix: `/${environment.applicationURLPrefix}`,
      key,
      value,
    } as AppUserPreferenceChange1Request;
    this.getApi().appUserPreferenceChange1?.(request);
  }

  sendAppUrlChange1(url: string) {
    const request = { url } as AppUrlChange1Request;
    this.getApi().appUrlChange1?.(request);
  }

  sendSupportedFeaturesMessage(features: string[], configurations?: PlatformFeature[]) {
    const request = {
      sourcePrefix: `/${environment.applicationURLPrefix}`,
      features,
      configurations,
    } as AppSupportedFeature1Request;
    this.getApi().appSupportedFeatures1?.(request);
  }

  /**
   * Notify Core to change the activate app
   * @param sourcePrefix Application prefix
   * @param route Route to navigate to
   */
  sendAppChangeNavigationRequest(sourcePrefix: string, route: string) {
    const request = { sourcePrefix, route } as AppChange2Request;
    this.getApi().appChange2?.(request);
  }

  private listenForExtensibilityResults() {
    this.extensibilityService.listenForSuccess().subscribe((result) => {
      this.getApi().appRunComponentSuccessResult1?.(result);
    });

    this.extensibilityService.listenForFailure().subscribe((result) => {
      this.getApi().appRunComponentFailureResult1?.(result);
    });
  }

  /**
   * Messages for after connection with core is successful. This ensures these messages use penpal.
   */
  private afterConnectionSuccess() {
    this.sendUserContextRequestMessage();
  }

  private async sendUserContextRequestMessage() {
    const request = {} as AppUserContext1Request;
    const response: AppUserContext1Response = await this.getApi().appUserContextRequest1?.(request);
    if (!environment.production) {
      console.log(`Received ${CoreMessagingKeys.APP_USERCONTEXT_REQUEST_1}:${JSON.stringify(response)}`);
    }

    this.signalRUserContextService.setUserContext(response);
  }

  private getApi() {
    return this.handler ?? this.legacy;
  }

  /**
   * Enable the BetterMessagingV1 if feature is enabled
   */
  private handleBetterMessagingV1() {
    this.featureSubscription = this.featureService.betterMessagingV1.valueAsync
      .pipe(
        take(1),
        filter((value) => value === true)
      )
      .subscribe(() => {
        this.registerMessagingForApplication();
      });
  }
}
