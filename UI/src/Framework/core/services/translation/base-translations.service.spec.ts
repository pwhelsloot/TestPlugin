import { Injectable } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { TranslateService } from '@ngx-translate/core';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { BaseTranslationsService } from './base-translations.service';

@Injectable()
class DummyTranslationsService extends BaseTranslationsService {
  constructor(readonly translateService: TranslateService, readonly localSettings: TranslateSettingService) {
    super(translateService, localSettings);
  }

  mapTranslations(translationArray: string[]) {}
}

describe('BaseTranslationsService', () => {
  let service: DummyTranslationsService;

  beforeEach(() => {
    const translateServiceMock = () => ({
      instant: () => ({}),
      use: () => ({}),
      langs: [],
      currentLang: {},
      onLangChange: { pipe: () => ({ subscribe: (f) => f({}) }) },
    });
    const translateSettingServiceMock = () => ({
      getUnauthenticatedLanguage: () => ({}),
      selectedLanguage: { pipe: () => ({ subscribe: (f) => f({}) }) },
    });
    TestBed.configureTestingModule({
      providers: [
        DummyTranslationsService,
        { provide: TranslateService, useFactory: translateServiceMock },
        {
          provide: TranslateSettingService,
          useFactory: translateSettingServiceMock,
        },
      ],
    });

    service = TestBed.inject(DummyTranslationsService);
  });

  describe('setLanguageToExistingOrDefault sets language', () => {
    it('sets language', () => {
      const translateSettingServiceMock = TestBed.inject(TranslateSettingService);
      const expectedLanguage = 'es'; // something different than default
      spyOn(translateSettingServiceMock, 'getUnauthenticatedLanguage').and.returnValue(expectedLanguage);
      service.setLanguageToExistingOrDefault();
      expect(translateSettingServiceMock.getUnauthenticatedLanguage).toHaveBeenCalled();
      expect(service.locale).toEqual(expectedLanguage);
    });

    it('does not set language when provided with undefined', () => {
      const translateSettingServiceMock = TestBed.inject(TranslateSettingService);
      const expectedLanguage = 'nl'; // something different than default
      service.locale = expectedLanguage;
      spyOn(translateSettingServiceMock, 'getUnauthenticatedLanguage').and.returnValue('');
      service.setLanguageToExistingOrDefault();
      expect(translateSettingServiceMock.getUnauthenticatedLanguage).toHaveBeenCalled();
      expect(translateSettingServiceMock.getUnauthenticatedLanguage).toHaveBeenCalledTimes(1);
      expect(service.locale).toEqual(expectedLanguage);
    });
  });

  it('get synchronous translation', () => {
    const translateService = TestBed.inject(TranslateService);
    const expectedTranslationValue = 'yep';
    const expectedTranslationKey = 'SomeKey';
    spyOn(translateService, 'instant').and.returnValue(expectedTranslationValue);

    const syncTranslation = service.getTranslation(expectedTranslationKey);
    expect(translateService.instant).toHaveBeenCalledOnceWith(expectedTranslationKey);
    expect(syncTranslation).toEqual(expectedTranslationValue);
  });
});
