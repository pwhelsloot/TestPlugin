import { TestBed } from '@angular/core/testing';
import { LegacyCoreAppApiService } from './legacy-container-api.service';

describe('LegacyCoreAppApiService', () => {
  let service: LegacyCoreAppApiService;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [LegacyCoreAppApiService] });
    service = TestBed.inject(LegacyCoreAppApiService);
  });

  it('can load instance', () => {
    expect(service).toBeTruthy();
  });
});
