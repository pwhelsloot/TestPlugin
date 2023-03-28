import { TestBed } from '@angular/core/testing';
import { Subject } from 'rxjs';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { LangChangeEvent, TranslateService } from '@ngx-translate/core';
import { EventEmitter } from '@angular/core';
import { TemplateTranslationsService } from '@app/template/services/template-translations.service';

describe('Service: TemplateTranslationsService', () => {
  let service: TemplateTranslationsService;
  let translateService: TranslateService;
  let mockTranslateSettings = {} as TranslateSettingService;
  const destroy: Subject<void> = new Subject();
  const observer: jasmine.Spy = jasmine.createSpy('TemplateTranslationsService Observer');
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
        TemplateTranslationsService,
        { provide: TranslateService, useFactory: mockTranslateServiceStub },
        { provide: TranslateSettingService, useValue: mockTranslateSettings },
      ],
    });
    service = TestBed.inject(TemplateTranslationsService);
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
