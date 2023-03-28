import { EventEmitter } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { LangChangeEvent, TranslateService } from '@ngx-translate/core';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { Subject } from 'rxjs';

describe('SharedTranslationsService', () => {
  let service: SharedTranslationsService;
  let translateService: TranslateService;
  let mockTranslateSettings = {} as TranslateSettingService;
  const destroy: Subject<void> = new Subject();
  const observer: jasmine.Spy = jasmine.createSpy('SharedTranslationsService Observer');
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
        SharedTranslationsService,
        { provide: TranslateService, useFactory: mockTranslateServiceStub },
        { provide: TranslateSettingService, useValue: mockTranslateSettings },
      ],
    });
    service = TestBed.inject(SharedTranslationsService);
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
