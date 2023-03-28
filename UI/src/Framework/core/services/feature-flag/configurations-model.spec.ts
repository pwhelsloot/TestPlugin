import { TestBed } from '@angular/core/testing';
import { ConfigurationStorageService } from './configuration-storage';
import { DummyConfigurationsService } from './testing/dummy-configuration.service';

describe('ConfigurationsBase', () => {
  let service: DummyConfigurationsService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [DummyConfigurationsService, ConfigurationStorageService]
    });
    service = TestBed.inject(DummyConfigurationsService);
  });

  it('can load instance', () => {
    expect(service).toBeTruthy();
  });

  it('can create configuration', ()=> {
    expect(service.feature1).toBeDefined();
  });
});
