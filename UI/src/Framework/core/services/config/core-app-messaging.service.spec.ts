import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ContainerIncomingMessagingService } from '@core-module/messaging/container-incoming-messaging.service';
import { MockProvider } from 'ng-mocks';
import { ConfigurationStorageService } from '../feature-flag/configuration-storage';
import { CoreAppFeaturesService } from './core-app-features.service';
import { CoreAppMessagingAdapter } from './core-app-messaging.service';
import { CoreAppApiHandler } from './messaging/core-app-api-handler.service';

describe('CoreAppMessagingAdapter', () => {
  let service: CoreAppMessagingAdapter;
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      providers: [
        CoreAppMessagingAdapter,
        MockProvider(CoreAppApiHandler),
        MockProvider(ContainerIncomingMessagingService),
        MockProvider(ConfigurationStorageService),
        MockProvider(CoreAppFeaturesService),
      ],
    });
    service = TestBed.inject(CoreAppMessagingAdapter);
  });

  it('can load instance', () => {
    expect(service).toBeTruthy();
  });
});
