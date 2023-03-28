import { PlatformFeature, PlatformFeatureBuilder, PlatformFeatures } from '@amcs/platform-communication';
import { Injectable, OnDestroy } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { ContainerIncomingMessagingService } from '@core-module/messaging/container-incoming-messaging.service';
import { takeUntil } from 'rxjs/operators';
import { BaseService } from '../base.service';
import { ICoreAppMessagingService } from './core-app-messaging.service.interface';
import { CoreAppApiHandler } from './messaging/core-app-api-handler.service';

@Injectable()
export abstract class CoreAppMessagingAdapter extends BaseService implements ICoreAppMessagingService, OnDestroy {
  constructor(
    private incomingMessagingService: ContainerIncomingMessagingService,
    private router: Router,
    private coreAppApiHandler: CoreAppApiHandler
  ) {
    super();
  }

  /**
   * All app specific features
   */
  abstract supportedFeatures(): string[] | PlatformFeatureBuilder;

  initialise() {
    this.coreAppApiHandler.registerLegacyMessagingForApplication();
    this.listenForCoreMessages();
    this.setUpURLChangeMessages();
    this.sendSupportedFeaturesMessage();
  }

  /**
   * Notify Core to change the activate app
   * @param sourcePrefix Application prefix
   * @param route Route to navigate to
   */
  sendAppChangeNavigationRequest(sourcePrefix: string, route: string) {
    this.coreAppApiHandler.sendAppChangeNavigationRequest(sourcePrefix, route);
  }

  /**
   * Notify Core that the page title needs to be changed
   * @param title The new title
   */
  sendAppChangeTitleRequest(title: string) {
    this.coreAppApiHandler.sendAppChangeTitleRequest(title);
  }

  /**
   * Notify Core that a user preference has been changed
   * @param key
   * @param value
   */
  sendCorePreferenceChangeNotification(key: string, value: string) {
    this.coreAppApiHandler.sendCorePreferenceChangeNotification(key, value);
  }

  private sendAppUrlChange1(url: string) {
    this.coreAppApiHandler.sendAppUrlChange1(url);
  }

  private sendSupportedFeaturesMessage() {
    const configuration = this.getAppFeatureConfiguration();
    this.coreAppApiHandler.sendSupportedFeaturesMessage(configuration.features, configuration.configurations);
  }

  private listenForCoreMessages() {
    this.incomingMessagingService.initialiseIncomingMessaging();
    window.onmessage = this.incomingMessagingService.handleMessage.bind(this.incomingMessagingService);
  }

  /**
   *
   * @returns All Framework and App specific features
   */
  private getAppFeatureConfiguration() {
    const supportedFeatures = this.supportedFeatures();
    let appFeatures = new Array<string>();
    let appFeatureConfigurations = new Array<PlatformFeature>();

    if (supportedFeatures instanceof PlatformFeatureBuilder) {
      // backwards compatibility incase core is older
      const builderFeatures = supportedFeatures.build();
      appFeatures = [PlatformFeatures.BetterMessagingV1.name, ...builderFeatures.map((t) => t.name)];
      appFeatureConfigurations = [PlatformFeatures.BetterMessagingV1, ...builderFeatures];
    } else {
      appFeatures = [PlatformFeatures.BetterMessagingV1.name, ...supportedFeatures];
    }
    return {
      features: [...new Set(appFeatures)],
      configurations: [...new Set(appFeatureConfigurations)],
    };
  }

  private setUpURLChangeMessages() {
    this.router.events.pipe(takeUntil(this.unsubscribe)).subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.sendAppUrlChange1(event.url);
      }
    });
  }
}
