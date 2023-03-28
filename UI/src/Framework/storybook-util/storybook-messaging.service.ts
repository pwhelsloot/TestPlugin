import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ContainerIncomingMessagingService } from '@core-module/messaging/container-incoming-messaging.service';
import { CoreAppMessagingAdapter } from '@core-module/services/config/core-app-messaging.service';
import { CoreAppApiHandler } from '@core-module/services/config/messaging/core-app-api-handler.service';
import { StoryBookTranslationService } from '@storybook-util/storybook-translations.service';

@Injectable()
export class StoryBookMessagingService extends CoreAppMessagingAdapter {
  constructor(
    protected readonly containerIncomingMessagingService: ContainerIncomingMessagingService,
    protected readonly ngRouter: Router,
    protected readonly coreApiHandler: CoreAppApiHandler,
    // Hack to ensure StoryBookTranslationService is always loaded
    readonly translationService: StoryBookTranslationService
  ) {
    super(containerIncomingMessagingService, ngRouter, coreApiHandler);
    StoryBookTranslationService.storyBookTranslationServiceReference = translationService;
    }

  supportedFeatures(): string[] {
    return ['headerBar:1'];
  }
}
