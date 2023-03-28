import { TestBed } from '@angular/core/testing';
import { EnhancedErpApiService } from '../enhanced-erp-api.service';
import { ConfigurationService } from './configuration.service';

describe('ConfigurationService', () => {
  let service: ConfigurationService;

  beforeEach(() => {
    const enhancedErpApiServiceStub = () => ({});
    TestBed.configureTestingModule({
      providers: [
        ConfigurationService,
        {
          provide: EnhancedErpApiService,
          useFactory: enhancedErpApiServiceStub
        }
      ]
    });
    service = TestBed.inject(ConfigurationService);
  });

  it('can load instance', () => {
    expect(service).toBeTruthy();
  });
});
