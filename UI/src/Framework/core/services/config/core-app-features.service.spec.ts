import { TestBed } from '@angular/core/testing';
import { ConfigurationStorageService } from '../feature-flag/configuration-storage';
import { CoreAppFeaturesService } from './core-app-features.service';

describe('CoreAppFeaturesService', () => {
  let service: CoreAppFeaturesService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [CoreAppFeaturesService, ConfigurationStorageService],
    });
    service = TestBed.inject(CoreAppFeaturesService);
  });

  it('can load instance', () => {
    expect(service).toBeTruthy();
  });

  it('can get all features', () => {
    const features = service.getAllFeatures();

    const expectedFeatures = ['header:1'];

    const shouldHaveAll = expectedFeatures.every((element) => {
      return features.indexOf(element) !== -1;
    });

    expect(shouldHaveAll).toBeTrue();
  });
});
