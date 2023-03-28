import { TestBed } from '@angular/core/testing';
import {
  PlatformAppChange2Request,
  PlatformSupportedFeature1Request,
  PlatformUserPreferenceChange1Request,
} from '@amcs/platform-communication';
import { ContainerAppMessagingHandlerService } from './messaging-handler.service';
import { ContainerAppApiService } from './container-api-handler';
import { MockProvider } from 'ng-mocks';

describe('ContainerAppApiService', () => {
  let service: ContainerAppApiService;
  let containerAppMessagingHandlerService: ContainerAppMessagingHandlerService;
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ContainerAppApiService, MockProvider(ContainerAppMessagingHandlerService)],
    });
    service = TestBed.inject(ContainerAppApiService);
    containerAppMessagingHandlerService = TestBed.inject(ContainerAppMessagingHandlerService);
  });

  it('can load instance', () => {
    expect(service).toBeTruthy();
  });

  describe('coreSupportedFeatures1', () => {
    it('makes expected calls', () => {
      const coreSupportedFeature1Request = {} as PlatformSupportedFeature1Request;
      spyOn(containerAppMessagingHandlerService, 'handleSupportedFeatures').and.callThrough();
      service.platformSupportedFeatures1(coreSupportedFeature1Request);
      expect(containerAppMessagingHandlerService.handleSupportedFeatures).toHaveBeenCalled();
    });
  });

  describe('coreAppChange2', () => {
    it('makes expected calls', () => {
      const coreAppChange2Request = {} as PlatformAppChange2Request;
      spyOn(containerAppMessagingHandlerService, 'handleAppChange2').and.callThrough();
      service.platformAppChange2(coreAppChange2Request);
      expect(containerAppMessagingHandlerService.handleAppChange2).toHaveBeenCalled();
    });
  });

  describe('coreUserPreferenceChange1', () => {
    it('makes expected calls', () => {
      const coreUserPreferenceChange1Request = {} as PlatformUserPreferenceChange1Request;
      spyOn(containerAppMessagingHandlerService, 'handleUserPreferenceChange1').and.callThrough();
      service.platformUserPreferenceChange1(coreUserPreferenceChange1Request);
      expect(containerAppMessagingHandlerService.handleUserPreferenceChange1).toHaveBeenCalled();
    });
  });
});
