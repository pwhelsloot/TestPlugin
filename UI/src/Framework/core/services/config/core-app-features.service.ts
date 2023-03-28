import { Injectable } from '@angular/core';
import { ConfigurationStorageService } from '../feature-flag/configuration-storage';
import { ConfigurationsBaseService } from '../feature-flag/configurations-base.service';

@Injectable({ providedIn: 'root' })
export class CoreAppFeaturesService extends ConfigurationsBaseService {
  readonly headerV1 = this.createConfig<boolean>('header:1');
  readonly betterMessagingV1 = this.createConfig<boolean>('betterMessaging:1');
  readonly extensibilityV1 = this.createConfig<boolean>('extensibility:1');

  // eslint-disable-next-line @typescript-eslint/no-useless-constructor
  constructor(storage: ConfigurationStorageService) {
    super(storage);
  }

  /**
   *
   * @returns List of all Features available
   */
  getAllFeatures(): string[] {
    return Array.from(this.storage.configurationValues.keys());
  }
}
