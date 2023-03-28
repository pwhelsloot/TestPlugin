import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { CoreUserPreferenceKeys } from '@core-module/models/preferences/core-user-preference-keys.model';
import { CoreUserPreference } from '@core-module/models/preferences/core-user-preference.model';
import { CoreAppMessagingAdapter } from '@core-module/services/config/core-app-messaging.service';
import { CoreUserPreferencesDataAccess } from '@core-module/services/preferences/data/core-user-preferences-data.access';
import { environment } from '@environments/environment';
import { Observable, of, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CoreAppFeaturesService } from '../config/core-app-features.service';
import { ContainerAppMessagingHandlerService } from '../config/messaging/messaging-handler.service';
import { ConfigurationStorageService } from '../feature-flag/configuration-storage';
import { CoreUserPreferencesService } from './core-user-preferences.service';
import { MockProvider } from 'ng-mocks';
import { MatDialogModule } from '@angular/material/dialog';
import { ExtensibilityModule } from '@amcs-extensibility/host';

describe('Service: CoreUserPreferencesService', () => {
  let service: CoreUserPreferencesService;
  const destroy: Subject<void> = new Subject();
  const observer: jasmine.Spy = jasmine.createSpy(
    'CoreUserPreferencesService Observer'
  );

  let containerAppMessagingHandlerService: ContainerAppMessagingHandlerService;
  const mockDataAccess = {} as CoreUserPreferencesDataAccess;

  let saveCount = 0;

  const errorKey = 'errorKey';
  const boolKey = 'boolKey';
  const stringKey = 'stringKey';

  beforeEach(() => {
    mockDataAccess.get = (key: string): Observable<CoreUserPreference> => {
      if (key === `${environment.applicationURLPrefix}-${errorKey}`) {
        // Any 'get' api errors always return an empty instance of the object type
        return of(new CoreUserPreference());
      }
      const result = new CoreUserPreference();
      result.key = key;

      switch (result.key) {
        case CoreUserPreferenceKeys.uiLanguage:
          result.value = '"en-gb"';
          break;

        case `${environment.applicationURLPrefix}-${boolKey}`:
          result.value = 'true';
          break;

        case `${environment.applicationURLPrefix}-${stringKey}`:
          result.value = '"some json blob"';
          break;
      }

      return of(result);
    };

    mockDataAccess.save = (): Observable<number> => {
      saveCount++;
      return of(1);
    };

    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        MatDialogModule,
        ExtensibilityModule.forRoot(''),
      ],
      providers: [
        CoreUserPreferencesService,
        MockProvider(CoreAppFeaturesService),
        MockProvider(ConfigurationStorageService),
        { provide: CoreUserPreferencesDataAccess, useValue: mockDataAccess },
        MockProvider(CoreAppMessagingAdapter),
      ],
    });
    service = TestBed.inject(CoreUserPreferencesService);
    containerAppMessagingHandlerService = TestBed.inject(
      ContainerAppMessagingHandlerService
    );
    service.userAuthenticated();
  });

  afterEach(() => {
    destroy.next();
    observer.calls.reset();
  });

  afterAll(() => {
    destroy.complete();
  });

  it('should be able to request a boolean user preferences', () => {
    // arrange + act
    service
      .get<boolean>(boolKey, false)
      .pipe(takeUntil(destroy))
      .pipe(takeUntil(destroy))
      .subscribe(observer);

    // assert
    expect(observer).toHaveBeenCalledWith(true);

    expect(observer).toHaveBeenCalledTimes(1);
  });

  it('should be able to request a string user preferences', () => {
    // arrange + act
    service
      .get<string>(stringKey, 'some default')
      .pipe(takeUntil(destroy))
      .subscribe(observer);

    // assert
    expect(observer).toHaveBeenCalledWith('some json blob');

    expect(observer).toHaveBeenCalledTimes(1);
  });

  it('should be able to handle error during user preference request and fallback to default value', () => {
    // arrange + act
    service
      .get<boolean>(errorKey, true)
      .pipe(takeUntil(destroy))
      .subscribe(observer);

    // assert
    expect(observer).toHaveBeenCalledWith(true);

    expect(observer).toHaveBeenCalledTimes(1);
  });

  it('should be able to save a user preference without subscribing', () => {
    // arrange + act
    const originalSaveCount = saveCount;
    service.save<string>('testString', 'some text');

    // assert
    expect(saveCount).toBe(originalSaveCount + 1);

    expect(observer).toHaveBeenCalledTimes(0);
  });

  it('should be able to request a core user preference and load from api', () => {
    // arrange + act
    service
      .get<string>(CoreUserPreferenceKeys.uiLanguage, 'some default lang')
      .pipe(takeUntil(destroy))
      .subscribe(observer);

    // assert
    expect(observer).toHaveBeenCalledWith('en-gb');

    expect(observer).toHaveBeenCalledTimes(1);
  });

  it('should be able to request a core user preference and load from core messaging', () => {
    // arrange + act
    service
      .get<string>(CoreUserPreferenceKeys.uiLanguage, 'some default lang')
      .pipe(takeUntil(destroy))
      .subscribe(observer);

    // assert
    expect(observer).toHaveBeenCalledWith('en-gb');

    expect(observer).toHaveBeenCalledTimes(1);

    containerAppMessagingHandlerService.handleUserPreferenceChange1({
      key: CoreUserPreferenceKeys.uiLanguage,
      value: '"some dummy value"',
    });

    expect(observer).toHaveBeenCalledWith('some dummy value');

    expect(observer).toHaveBeenCalledTimes(2);

    containerAppMessagingHandlerService.handleUserPreferenceChange1({
      key: CoreUserPreferenceKeys.uiLanguage + 'test',
      value: '"some dummy value"',
    });

    expect(observer).toHaveBeenCalledWith('some dummy value');

    expect(observer).toHaveBeenCalledTimes(2);
  });
});
