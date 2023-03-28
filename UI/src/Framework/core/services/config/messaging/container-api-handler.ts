import {
  AppUserPreferenceChange1Response,
  PlatformAppChange2Request,
  PlatformAppChange2Response,
  PluginToPlatformCommunicator,
  PlatformRunComponent1Request,
  PlatformRunComponent1Response,
  PlatformSupportedFeature1Request,
  PlatformSupportedFeature1Response,
  PlatformUserPreferenceChange1Request,
  PlatformHeaderNavigationItems1Request,
  PlatformHeaderNavigationItems1Response,
  IPluginToPlatformApi,
} from '@amcs/platform-communication';
import { Injectable } from '@angular/core';
import { Connection } from 'penpal';
import { ContainerAppMessagingHandlerService } from './messaging-handler.service';

/**
 * Penpal based ContainerAppApi
 * Handles incoming penpal messages from Platform
 */
@Injectable({ providedIn: 'root' })
export class ContainerAppApiService extends PluginToPlatformCommunicator {
  activeConnection: Connection<IPluginToPlatformApi>;

  constructor(private readonly messagingHandlerService: ContainerAppMessagingHandlerService) {
    super();
  }

  platformSupportedFeatures1(request: PlatformSupportedFeature1Request): PlatformSupportedFeature1Response {
    this.messagingHandlerService.handleSupportedFeatures(request);
    return {} as PlatformSupportedFeature1Response;
  }

  platformAppChange2(request: PlatformAppChange2Request): PlatformAppChange2Response {
    this.messagingHandlerService.handleAppChange2(request);
    return {} as PlatformAppChange2Response;
  }

  platformUserPreferenceChange1(request: PlatformUserPreferenceChange1Request): AppUserPreferenceChange1Response {
    this.messagingHandlerService.handleUserPreferenceChange1(request);
    return {} as AppUserPreferenceChange1Response;
  }

  platformRunComponentRequest1(request: PlatformRunComponent1Request): PlatformRunComponent1Response {
    this.messagingHandlerService.handleRunComponent1Request(request);
    return {} as PlatformRunComponent1Response;
  }

  platformHeaderNavigationItems1(request: PlatformHeaderNavigationItems1Request): PlatformHeaderNavigationItems1Response {
    this.messagingHandlerService.handleHeaderNavigationItemsChange1(request);
    return {} as PlatformHeaderNavigationItems1Response;
  }
}
