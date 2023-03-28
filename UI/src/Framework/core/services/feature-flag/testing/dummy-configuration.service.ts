import { Injectable } from '@angular/core';
import { ConfigurationStorageService } from '../configuration-storage';
import { ConfigurationsBaseService } from '../configurations-base.service';

@Injectable()
export class DummyConfigurationsService extends ConfigurationsBaseService {
  readonly feature1 = this.createConfig<boolean>('FeatureFlag1');

  // eslint-disable-next-line @typescript-eslint/no-useless-constructor
  constructor(storage: ConfigurationStorageService) {
    super(storage);
  }
}
