import { Injectable } from '@angular/core';
import { BaseService } from '@core-module/services/base.service';
import { ConfigValue } from './config-value.model';
import { ConfigurationStorageService } from './configuration-storage';

@Injectable()
export class ConfigurationUpdateService extends BaseService {
  constructor(private readonly storage: ConfigurationStorageService) {
    super();
  }

  updateValues(values: ConfigValue[]): void {
    values.forEach((value) => this.updateValue(value.id, value.value));
  }

  updateValue(configId: string, value: any): void {
    this.storage.updateValue(configId, value);
  }
}
