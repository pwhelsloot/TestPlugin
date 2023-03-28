import { PlatformCommunicationMethods } from '@amcs/platform-communication';
import { Injectable } from '@angular/core';
import { IncomingMessagingBase } from '@core-module/messaging/incoming-messaging-base.service';
import { ContainerAppMessagingHandlerService } from '@core-module/services/config/messaging/messaging-handler.service';
import { CoreMessagingKeys } from './messaging-keys.model';

@Injectable({ providedIn: 'root' })
export class ContainerIncomingMessagingService extends IncomingMessagingBase {
  constructor(private readonly messagingHandlerService: ContainerAppMessagingHandlerService) {
    super();
  }

  availableMethods(): PlatformCommunicationMethods {
    return {
      [CoreMessagingKeys.CORE_APP_CHANGE_2]: this.messagingHandlerService.handleAppChange2.bind(this.messagingHandlerService),
      [CoreMessagingKeys.CORE_SUPPORTED_FEATURES_1]: this.messagingHandlerService.handleSupportedFeatures.bind(
        this.messagingHandlerService
      ),
      [CoreMessagingKeys.CORE_USERPREFERENCE_CHANGE_1]: this.messagingHandlerService.handleUserPreferenceChange1.bind(
        this.messagingHandlerService
      ),
      [CoreMessagingKeys.APP_RUN_EXT_COMPONENT_REQUEST_1]: this.messagingHandlerService.handleRunComponent1Request.bind(
        this.messagingHandlerService
      ),
      [CoreMessagingKeys.CORE_HEADER_NAVIGATION_ITEMS_CHANGE_1]: this.messagingHandlerService.handleHeaderNavigationItemsChange1.bind(
        this.messagingHandlerService
      ),
    } as PlatformCommunicationMethods;
  }
}
