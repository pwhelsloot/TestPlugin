import { Injectable } from '@angular/core';
import { ConfigValue } from './config-value.model';
import { ConfigurationModelType } from './configuration-model.type';
import { Configuration } from './configuration.model';

@Injectable()
export class ConfigurationStorageService {
  readonly configurationValues = new Map<string, Configuration<ConfigurationModelType>>();

  /**
   * Update the configuration attached to this config id
   *
   * @param {string} configId
   * @param {*} value
   * @memberof ConfigurationStorageService
   */
  updateValue(configId: string, value: any): void {
    if (!configId) {
      throw new Error('Please provide a valid configId.');
    }

    this.getConfiguration(configId)?.setValue(value);
  }

  /**
   * Update the configuration using the given configValue
   * @param configValue
   */
  updateValueByConfig(configValue: ConfigValue) {
    if (!configValue) {
      throw new Error('Please provide a valid ConfigValue to update.');
    }

    this.updateValue(configValue.id, configValue.value);
  }

  /**
   * Add a Configuration to storage
   * @param config Configuration to add to Storage
   */
  addConfiguration(config: Configuration) {
    if (!config) {
      throw new Error('Cannot add a empty configuration.');
    }

    if (this.configurationValues.has(config.configId)) {
      throw new Error(`Configuration with Id ${config.configId} already exists and cannot be overwritten.`);
    }

    this.configurationValues.set(config.configId, config);
  }

  /**
   * Get the Configuration attached to this config id
   *
   * @private
   * @param {string} configId
   * @returns {ReplaySubject<any>}
   * @memberof ConfigurationStorageService
   */
  private getConfiguration(configId: string) {
    return this.configurationValues.get(configId);
  }
}
