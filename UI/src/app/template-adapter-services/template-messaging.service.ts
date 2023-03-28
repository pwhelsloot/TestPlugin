import { PlatformFeatureBuilder } from '@amcs/platform-communication';
import { Injectable } from '@angular/core';
import { CoreAppMessagingAdapter } from '@core-module/services/config/core-app-messaging.service';

@Injectable({ providedIn: 'root' })
export class TemplateMessagingService extends CoreAppMessagingAdapter {
  supportedFeatures(): PlatformFeatureBuilder {
    return PlatformFeatureBuilder.create().withBetterMessagingV1().withExtensibilityV1().withHeaderV1();
  }
}
