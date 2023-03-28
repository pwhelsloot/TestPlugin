import { ExtensibilityModule } from '@amcs-extensibility/host';
import {
  AppChange2Response,
  AppTitleChange1Response,
  AppUrlChange1Response,
  AppUserPreferenceChange1Response,
  AppSupportedFeature1Response,
  PluginToPlatformApi,
  AppUserContext1Response,
  AppRunComponent1FailureResponse,
  AppRunComponent1SuccessResponse,
  AppUpdateFeatureConfiguration1Response,
} from '@amcs/platform-communication';
import { TestBed } from '@angular/core/testing';
import { MatDialogModule } from '@angular/material/dialog';
import { RouterTestingModule } from '@angular/router/testing';
import { FeatureFlagModule } from '@core-module/services/feature-flag/feature-flag.module';
import { MockProvider } from 'ng-mocks';
import { ContainerAppApiService } from './container-api-handler';
import { CoreAppApiHandler } from './core-app-api-handler.service';
import { LegacyCoreAppApiService } from './legacy/legacy-container-api.service';

const createDummyPenpal = () => {
  const dummyPenpal: Promise<PluginToPlatformApi> = new Promise(async (resolve) => {
    resolve({
      appChange2: () => new Promise<AppChange2Response>(() => {}),
      appTitleChange1: () => new Promise<AppTitleChange1Response>(() => {}),
      appUrlChange1: () => new Promise<AppUrlChange1Response>(() => {}),
      appUserPreferenceChange1: () => new Promise<AppUserPreferenceChange1Response>(() => {}),
      appSupportedFeatures1: () => new Promise<AppSupportedFeature1Response>(() => {}),
      appUserContextRequest1: () => new Promise<AppUserContext1Response>(() => {}),
      appUpdateFeatureConfiguration1: () => new Promise<AppUpdateFeatureConfiguration1Response>(() => {}),
      appRunComponentSuccessResult1: () => new Promise<AppRunComponent1SuccessResponse>(() => {}),
      appRunComponentFailureResult1: () => new Promise<AppRunComponent1FailureResponse>(() => {}),
    });
  });
  return dummyPenpal;
};

describe('CoreAppApiHandler', () => {
  let service: CoreAppApiHandler;
  let legacyCoreAppApiService: LegacyCoreAppApiService;
  let containerAppApiService: ContainerAppApiService;
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule, FeatureFlagModule, MatDialogModule, ExtensibilityModule.forRoot('')],
      providers: [CoreAppApiHandler, MockProvider(ContainerAppApiService), MockProvider(LegacyCoreAppApiService)],
    });
    service = TestBed.inject(CoreAppApiHandler);
    legacyCoreAppApiService = TestBed.inject(LegacyCoreAppApiService);
    containerAppApiService = TestBed.inject(ContainerAppApiService);
  });

  it('can load instance', () => {
    expect(service).toBeTruthy();
  });

  describe('registerLegacyMessagingForApplication', () => {
    it('makes expected calls', () => {
      spyOn(legacyCoreAppApiService, 'setWindow').and.callThrough();
      service.registerLegacyMessagingForApplication();
      expect(legacyCoreAppApiService.setWindow).toHaveBeenCalled();
    });
  });

  describe('registerMessagingForApplication', () => {
    it('makes expected calls', () => {
      const dummyPenPal = createDummyPenpal();
      spyOn(containerAppApiService, 'connectToPlatform').and.returnValue(dummyPenPal);
      service.registerMessagingForApplication();
      expect(containerAppApiService.connectToPlatform).toHaveBeenCalled();
    });
  });
});
