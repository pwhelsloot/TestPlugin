import { EventEmitter } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { CoreTranslationsService } from '@core-module/services/translation/core-translation.service';
import { LangChangeEvent, TranslateService } from '@ngx-translate/core';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { Subject } from 'rxjs';

describe('CoreTranslationsService', () => {
  let service: CoreTranslationsService;
  let translateService: TranslateService;
  let mockTranslateSettings = {} as TranslateSettingService;
  const destroy: Subject<void> = new Subject();
  const observer: jasmine.Spy = jasmine.createSpy('CoreTranslationsService Observer');
  const testKey = 'test';

  beforeEach(() => {
    mockTranslateSettings.selectedLanguage = new Subject<string>();
    const mockTranslateServiceStub = () => ({
      onLangChange: new EventEmitter<LangChangeEvent>(),
      instant: (key: string | string[], interpolateParams?: Object): string | any => {
        return testKey;
      },
    });
    TestBed.configureTestingModule({
      providers: [
        CoreTranslationsService,
        { provide: TranslateService, useFactory: mockTranslateServiceStub },
        { provide: TranslateSettingService, useValue: mockTranslateSettings },
      ],
    });
    service = TestBed.inject(CoreTranslationsService);
    translateService = TestBed.inject(TranslateService);
  });

  afterEach(() => {
    destroy.next();
    observer.calls.reset();
  });

  afterAll(() => {
    destroy.complete();
  });

  it('language service is loaded', () => {
    expect(service).toBeDefined();
  });

  it('mapTranslations loaded', () => {
    spyOn(translateService, 'instant').and.callThrough();

    service.mapTranslations([]);

    expect(service.translations).toBeDefined();
    expect(translateService.instant).toHaveBeenCalled();
  });
});
