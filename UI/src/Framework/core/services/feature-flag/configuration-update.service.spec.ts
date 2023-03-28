import { TestBed } from '@angular/core/testing';
import { ConfigValue } from './config-value.model';
import { ConfigurationStorageService } from './configuration-storage';
import { ConfigurationUpdateService } from './configuration-update.service';

describe('ConfigurationUpdateService', () => {
  let service: ConfigurationUpdateService;
  let storage: ConfigurationStorageService;

  beforeEach(() => {
    const configurationStorageStub = () => ({
      updateValue: (configId, value) => ({})
    });
    TestBed.configureTestingModule({
      providers: [ConfigurationUpdateService, { provide: ConfigurationStorageService, useFactory: configurationStorageStub }]
    });
    service = TestBed.inject(ConfigurationUpdateService);
    storage = TestBed.inject(ConfigurationStorageService);
  });

  it('should update value', () => {
    spyOn(storage, 'updateValue').and.callThrough();
    const expectedConfigValue = true;
    const expectedConfigId = '1';
    const config = new ConfigValue();
    config.value = expectedConfigValue;
    config.id = expectedConfigId;

    service.updateValues([config]);

    expect(storage.updateValue).toHaveBeenCalledWith(expectedConfigId, expectedConfigValue);
  });
});
