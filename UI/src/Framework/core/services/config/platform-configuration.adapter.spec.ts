import { CommonModule } from '@angular/common';
import { TestBed } from '@angular/core/testing';
import { RouterModule } from '@angular/router';
import { Action, Store } from '@ngrx/store';
import { AmcsModalConfig } from '@shared-module/components/amcs-modal/amcs-modal-config.model';
import { AmcsModalService } from '@shared-module/components/amcs-modal/amcs-modal.service';
import { FakePlatformConfigurationService } from '@testing/fake-platform-configuration-adapter';
import { ErrorNotificationService } from '../error-notification.service';
import { ReportingBiHelperService } from '../reportingBi/reportingBi-helper';
import { PlatformConfigurationServiceData } from './data/platform-configuration.service.data';
import { PlatformTitleService } from './platform-title.service';

describe('Service: PlatformConfigurationAdapter', () => {
  let service: FakePlatformConfigurationService;
  let errorNotificationService: ErrorNotificationService;
  let amcsModalService: AmcsModalService;

  beforeEach(() => {
    const storeStub = () => ({ dispatch: (action: Action) => ({}) });
    const amcsModalServiceStub = () => ({
      getErrorModalsCount: () => ({}),
      createModal: (config: AmcsModalConfig) => ({})
    });
    const appTranslationsServiceStub = () => ({
      translations: { pipe: () => ({ subscribe: sub => sub({}) }) }
    });
    const reportingBiHelperServiceStub = () => ({
      Redirect: (parameter: string[]) => ({})
    });
    const platformConfigurationServiceDataStub = () => ({
      getConfiguration: (urlKey, type) => ({
        pipe: () => ({ subscribe: sub => sub({}) })
      })
    });
    const platformTitleServiceStub = () => ({
      getTitle: () => 'title',
      setTitle: title => ({})
    });

    TestBed.configureTestingModule({
      imports: [CommonModule, RouterModule.forRoot([])],
      providers: [
        FakePlatformConfigurationService,
        ErrorNotificationService,
        { provide: Store, useFactory: storeStub },
        { provide: AmcsModalService, useFactory: amcsModalServiceStub },
        {
          provide: ReportingBiHelperService,
          useFactory: reportingBiHelperServiceStub
        },
        {
          provide: PlatformConfigurationServiceData,
          useFactory: platformConfigurationServiceDataStub
        },
        { provide: PlatformTitleService, useFactory: platformTitleServiceStub }
      ]
    });
    service = TestBed.inject(FakePlatformConfigurationService);
    errorNotificationService = TestBed.inject(ErrorNotificationService);
    amcsModalService = TestBed.inject(AmcsModalService);
  });

  describe('#initialise', () => {
    it('shows modal on error', () => {
      const expectedErrorMessage = 'Something went poof';
      spyOn(amcsModalService, 'createModal').and.callThrough();
      spyOn(amcsModalService, 'getErrorModalsCount').and.returnValue(0);

      service.initialise();
      errorNotificationService.notifyError(expectedErrorMessage);

      expect(amcsModalService.createModal).toHaveBeenCalled();
      expect(amcsModalService.createModal).toHaveBeenCalledTimes(1);
    });

    it('does not show multiple modals on error', () => {
      const expectedErrorMessage = 'Something went poof';
      spyOn(amcsModalService, 'getErrorModalsCount').and.callThrough();
      spyOn(amcsModalService, 'createModal').and.callThrough();

      service.initialise();
      errorNotificationService.notifyError(expectedErrorMessage);
      errorNotificationService.notifyError(expectedErrorMessage);

      expect(amcsModalService.createModal).toHaveBeenCalledTimes(0);
    });
  });
});
