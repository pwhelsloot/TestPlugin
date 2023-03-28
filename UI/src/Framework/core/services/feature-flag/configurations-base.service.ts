import { Injectable } from '@angular/core';
import { ConfigurationStorageService } from './configuration-storage';
import { Configuration } from './configuration.model';

@Injectable()
export class ConfigurationsBaseService {
  constructor(protected readonly storage: ConfigurationStorageService) {}

  protected createConfig<T = string | boolean>(configId: string) {
    const configuration = new Configuration(configId);
    this.storage.addConfiguration(configuration);
    return configuration;
  }
}
