import { ExtensibilityService } from '@amcs-extensibility/host';
import {
  PlatformAppChange2Request,
  PlatformHeaderNavigationItem,
  PlatformHeaderNavigationItems1Request,
  PlatformRunComponent1Request,
  PlatformSupportedFeature1Request,
  PlatformUserPreferenceChange1Request
} from '@amcs/platform-communication';
import { Injectable, OnDestroy, RendererFactory2 } from '@angular/core';
import { Router } from '@angular/router';
import { PluginLogger } from '@core-module/logging/plugin-logger';
import { CoreMessagingKeys } from '@core-module/messaging/messaging-keys.model';
import { BaseService } from '@core-module/services/base.service';
import { ConfigurationStorageService } from '@core-module/services/feature-flag/configuration-storage';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { AppLoadingService } from '../app-loading.service';
import { CoreAppFeaturesService } from '../core-app-features.service';

@Injectable({ providedIn: 'root' })
export class ContainerAppMessagingHandlerService extends BaseService implements OnDestroy {
  headerNavigationItems$: Observable<PlatformHeaderNavigationItem[]>;

  constructor(
    private readonly router: Router,
    private readonly configurationStorageService: ConfigurationStorageService,
    private readonly coreAppFeaturesService: CoreAppFeaturesService,
    private readonly rendererFactory: RendererFactory2,
    private readonly extensibilityService: ExtensibilityService,
    private readonly appLoadingService: AppLoadingService
  ) {
    super();
    this.headerNavigationItems$ = this.headerNavigationItemsSubject.asObservable();
  }

  private readonly coreUserPreferenceChangedSubject = new Subject<{ key: string; value: string }>();
  private readonly headerNavigationItemsSubject = new BehaviorSubject<PlatformHeaderNavigationItem[]>([]);

  private removeClickEventListener: () => void;
  private removeTouchEventListener: () => void;

  ngOnDestroy(): void {
    if (this.removeClickEventListener) {
      this.removeClickEventListener();
    }

    if (this.removeTouchEventListener) {
      this.removeTouchEventListener();
    }
  }

  /**
   * Send a message to Header to close menu
   */
  sendCloseMenuRequest() {
    //TODO: Add this to PenPal messaging
    window.parent.postMessage([CoreMessagingKeys.APP_CLOSE_MENU_1, {}], '*');
  }

  /**
   *
   * @param features Available Features from Core
   */
  handleSupportedFeatures(request: PlatformSupportedFeature1Request) {
    request.features.forEach((feature) => {
      this.configurationStorageService.updateValue(feature, true);
    });
    this.setHeaderV1();
  }

  handleUserPreferenceChange1(request: PlatformUserPreferenceChange1Request) {
    this.coreUserPreferenceChangedSubject.next(request);
  }

  handleRunComponent1Request(request: PlatformRunComponent1Request) {
    if (this.coreAppFeaturesService.extensibilityV1.value) {
      this.appLoadingService.loadingComplete().subscribe(() => {
        PluginLogger.log(`handleRunComponent1Request -> runComponent for payload ${JSON.stringify(request)}`);
        this.extensibilityService.runComponent({
          name: request.name,
          allowedActions: request.allowedActions,
          parameters: request.parameters,
          callbackKey: request.callbackKey,
        });
      });
    }
  }

  handleHeaderNavigationItemsChange1(request: PlatformHeaderNavigationItems1Request) {
    this.headerNavigationItemsSubject.next(request.items);
  }

  coreUserPreferenceChanged<T>(key: string): Observable<T> {
    return this.coreUserPreferenceChangedSubject.pipe(
      filter((x) => x.key === key),
      map((preference: { key: string; value: string }) => {
        return JSON.parse(preference.value);
      })
    );
  }

  handleAppChange2(request: PlatformAppChange2Request) {
    this.router.navigate([this.trimStart(request.url)]);
  }

  private setHeaderV1() {
    if (this.coreAppFeaturesService.headerV1?.value) {
      const renderer = this.rendererFactory.createRenderer(null, null);
      const eventCallback = (event: any) => {
        this.sendCloseMenuRequest();
      };
      this.removeClickEventListener = renderer.listen('document', 'click', eventCallback);
      this.removeTouchEventListener = renderer.listen('document', 'touchmove', eventCallback);
    }
  }

  private trimStart(prefix: string): string {
    if (prefix.charAt(0) === '/') {
      return prefix.substr(1);
    }
    return prefix;
  }
}
