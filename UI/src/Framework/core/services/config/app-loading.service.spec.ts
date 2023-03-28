/* tslint:disable:no-unused-variable */

import { TestBed, inject } from '@angular/core/testing';
import { AppLoadingService } from './app-loading.service';

describe('Service: AppLoading', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AppLoadingService],
    });
  });

  it('should ...', inject([AppLoadingService], (service: AppLoadingService) => {
    expect(service).toBeTruthy();
  }));
});
