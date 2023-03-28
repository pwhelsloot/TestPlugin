import { TestBed } from '@angular/core/testing';
import { ConfigValue } from './config-value.model';
import { ConfigurationStorageService } from './configuration-storage';
import { Configuration } from './configuration.model';

describe('ConfigurationStorageService', () => {
  let service: ConfigurationStorageService;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [ConfigurationStorageService] });
    service = TestBed.inject(ConfigurationStorageService);
  });

  it('should have no configurations on startup', () => {
    expect(service.configurationValues.size).toBe(0);
  });

  it('should add configuration', () => {
    const expectedConfigId = 'my-awesome-config';
    const configuration = new Configuration<boolean>(expectedConfigId);

    expect(configuration.value).toBeUndefined();

    service.addConfiguration(configuration);

    expect(service.configurationValues.get('my-awesome-config')).toEqual(configuration);
  });

  it('should throw adding existing configuration', () => {
    const expectedConfigId = 'my-awesome-config';
    const configuration = new Configuration<boolean>(expectedConfigId);

    expect(configuration.value).toBeUndefined();

    service.addConfiguration(configuration);

    expect(() => service.addConfiguration(configuration)).toThrow(
      new Error(`Configuration with Id ${expectedConfigId} already exists and cannot be overwritten.`)
    );
  });

  it('should throw adding empty configuration', () => {
    expect(() => service.addConfiguration(undefined)).toThrow(new Error('Cannot add a empty configuration.'));
  });

  it('should update value for configuration', () => {
    const expectedConfigValue = false;
    const expectedConfigId = 'my-awesome-config';
    const config = createConfigValue(expectedConfigId, expectedConfigValue);

    const configuration = new Configuration<boolean>(expectedConfigId);
    expect(configuration.value).toBeUndefined();
    service.addConfiguration(configuration);

    service.updateValueByConfig(config);
    expect(configuration.value).toEqual(expectedConfigValue);
  });

  it('should throw error updating with invalid configuration Id', () => {
    const expectedConfigId = 'my-awesome-config';
    const configuration = new Configuration<boolean>(expectedConfigId);
    expect(configuration.value).toBeUndefined();
    expect(() => service.updateValueByConfig(undefined)).toThrow(new Error('Please provide a valid ConfigValue to update.'));
  });

  it('should throw error updating with invalid configuration Id', () => {
    const expectedConfigId = 'my-awesome-config';
    const configuration = new Configuration<boolean>(expectedConfigId);
    expect(configuration.value).toBeUndefined();
    expect(() => service.updateValue('', '')).toThrow(new Error('Please provide a valid configId.'));
  });
});

function createConfigValue(id: string, value: any) {
  const config = new ConfigValue();
  config.value = value;
  config.id = id;
  return config;
}
