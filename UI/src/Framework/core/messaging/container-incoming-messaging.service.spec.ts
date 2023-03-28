import { TestBed } from '@angular/core/testing';
import { ContainerAppMessagingHandlerService } from '@core-module/services/config/messaging/messaging-handler.service';
import { MockProvider } from 'ng-mocks';
import { ContainerIncomingMessagingService } from './container-incoming-messaging.service';
import { CoreMessagingKeys } from './messaging-keys.model';

describe('ContainerIncomingMessagingService', () => {
  let service: ContainerIncomingMessagingService;
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ContainerIncomingMessagingService, MockProvider(ContainerAppMessagingHandlerService)],
    });
    service = TestBed.inject(ContainerIncomingMessagingService);
  });

  it('can load instance', () => {
    expect(service).toBeTruthy();
  });

  describe('ensure core api is available', () => {
    it('coreAppChange2', () => {
      const methods = service.availableMethods();
      expect(methods[CoreMessagingKeys.CORE_APP_CHANGE_2]).toBeDefined();
    });
    it('coreSupportedFeatures1', () => {
      const methods = service.availableMethods();
      expect(methods[CoreMessagingKeys.CORE_SUPPORTED_FEATURES_1]).toBeDefined();
    });
    it('coreUserPreferenceChange1', () => {
      const methods = service.availableMethods();
      expect(methods[CoreMessagingKeys.CORE_USERPREFERENCE_CHANGE_1]).toBeDefined();
    });
  });
});
