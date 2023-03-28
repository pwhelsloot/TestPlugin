import { fakeAsync, TestBed } from '@angular/core/testing';
import { IApiRequestGetOptions } from '@core-module/models/api/api-request-get-options.interface';
import { IFilter } from '@core-module/models/api/filters/iFilter';
import { GlossaryLanguage } from '@core-module/models/glossary/glossary-language.model';
import { GlossaryService } from '@core-module/services/glossary/glossary.service';
import { ApiBusinessService } from '@core-module/services/service-structure/api-business.service';
import { ApiFilter } from '@core-module/services/service-structure/api-filter';
import { ApiFilters } from '@core-module/services/service-structure/api-filter-builder';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { Observable, of, ReplaySubject, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

describe('Service: GlossaryService', () => {
  let service: GlossaryService;
  let businessService: ApiBusinessService;
  let defaultGlossaryLanguages = [];
  let languageChangeSubject: ReplaySubject<string>;
  const mockTranslateSettingsService = {} as TranslateSettingService;
  const defaultLanguageCode = 'en-gb';
  const destroy: Subject<void> = new Subject();
  const observer: jasmine.Spy = jasmine.createSpy('GlossaryService Observer');

  beforeEach(() => {
    languageChangeSubject = new ReplaySubject<string>(1);
    languageChangeSubject.next(defaultLanguageCode);
    mockTranslateSettingsService.selectedLanguage = languageChangeSubject.asObservable();
    const mockApiBusinessServiceStub = () => ({
      getArray: (filters: IFilter[], responseType, options: IApiRequestGetOptions): Observable<GlossaryLanguage[]> => {
        return of(defaultGlossaryLanguages);
      },
    });
    TestBed.configureTestingModule({
      providers: [
        GlossaryService,
        { provide: ApiBusinessService, useFactory: mockApiBusinessServiceStub },
        {
          provide: TranslateSettingService,
          useValue: mockTranslateSettingsService,
        },
      ],
    });
    businessService = TestBed.inject(ApiBusinessService);
    spyOn(businessService, 'getArray').and.callThrough();
    service = TestBed.inject(GlossaryService);
  });

  afterEach(() => {
    destroy.next();
    observer.calls.reset();
  });

  afterAll(() => {
    destroy.complete();
  });

  it('glossary should be loaded on instantiation', fakeAsync(async () => {
    // assert
    expect(businessService.getArray).toHaveBeenCalledOnceWith(
      new ApiFilters().add(ApiFilter.equal('LanguageCode', defaultLanguageCode)).build(),
      GlossaryLanguage,
      { isCoreRequest: true, suppressErrorModal: true }
    );
  }));

  it('glossary should be reloaded on language change', fakeAsync(async () => {
    // assert/arrange
    service.glossaryUpdated$.pipe(takeUntil(destroy)).subscribe(observer);
    expect(businessService.getArray).toHaveBeenCalledOnceWith(
      new ApiFilters().add(ApiFilter.equal('LanguageCode', defaultLanguageCode)).build(),
      GlossaryLanguage,
      { isCoreRequest: true, suppressErrorModal: true }
    );

    // act
    const secondLanguageCode = 'en-us';
    languageChangeSubject.next(secondLanguageCode);

    // assert
    expect(observer).toHaveBeenCalledOnceWith(undefined);
    expect(businessService.getArray).toHaveBeenCalledTimes(2);
    expect(businessService.getArray).toHaveBeenCalledWith(
      new ApiFilters().add(ApiFilter.equal('LanguageCode', secondLanguageCode)).build(),
      GlossaryLanguage,
      { isCoreRequest: true, suppressErrorModal: true }
    );
  }));

  it('only full matches can be swapped with glossary', fakeAsync(async () => {
    // arrange
    const defaultGlossaryLanguage = new GlossaryLanguage();
    defaultGlossaryLanguage.original = 'hi. I am a sentence.';
    defaultGlossaryLanguage.translated = 'hello. This sentence has been swapped.';

    const defaultGlossaryLanguageSingleWord = new GlossaryLanguage();
    defaultGlossaryLanguageSingleWord.original = 'sentence';
    defaultGlossaryLanguageSingleWord.translated = 'sentenceReplacement';

    defaultGlossaryLanguages = [defaultGlossaryLanguage, defaultGlossaryLanguageSingleWord];

    languageChangeSubject.next(defaultLanguageCode);

    const exactMatch = 'hi. I am a sentence.';
    const nearMatch = 'hi. I am a sentence. ';
    const singleWord = 'sentence';

    // act
    const exactMatchTranslation = service.getGlossaryTranslation(exactMatch);
    const nearMatchTranslation = service.getGlossaryTranslation(nearMatch);
    const singleWordTranslation = service.getGlossaryTranslation(singleWord);

    // assert
    expect(exactMatchTranslation).toEqual('hello. This sentence has been swapped.');
    expect(nearMatchTranslation).toEqual('hi. I am a sentence. ');
    expect(singleWordTranslation).toEqual('sentenceReplacement');
  }));

  it('no matches if glossary disabled', fakeAsync(async () => {
    // arrange
    const defaultGlossaryLanguage = new GlossaryLanguage();
    defaultGlossaryLanguage.original = 'hi. I am a sentence.';
    defaultGlossaryLanguage.translated = 'hello. This sentence has been swapped.';

    const defaultGlossaryLanguageSingleWord = new GlossaryLanguage();
    defaultGlossaryLanguageSingleWord.original = 'sentence';
    defaultGlossaryLanguageSingleWord.translated = 'sentenceReplacement';

    defaultGlossaryLanguages = [defaultGlossaryLanguage, defaultGlossaryLanguageSingleWord];

    languageChangeSubject.next(defaultLanguageCode);

    const exactMatch = 'hi. I am a sentence.';
    const nearMatch = 'hi. I am a sentence. ';
    const singleWord = 'sentence';

    // act
    service.disableGlossary();

    const exactMatchTranslation = service.getGlossaryTranslation(exactMatch);
    const nearMatchTranslation = service.getGlossaryTranslation(nearMatch);
    const singleWordTranslation = service.getGlossaryTranslation(singleWord);

    // assert
    expect(exactMatchTranslation).toEqual(exactMatch);
    expect(nearMatchTranslation).toEqual(nearMatch);
    expect(singleWordTranslation).toEqual(singleWord);
  }));

  it('no matches if glossary disabled', fakeAsync(async () => {
    // arrange
    const defaultGlossaryLanguage = new GlossaryLanguage();
    defaultGlossaryLanguage.original = 'hi. I am a sentence.';
    defaultGlossaryLanguage.translated = 'hello. This sentence has been swapped.';

    const defaultGlossaryLanguageSingleWord = new GlossaryLanguage();
    defaultGlossaryLanguageSingleWord.original = 'sentence';
    defaultGlossaryLanguageSingleWord.translated = 'sentenceReplacement';

    defaultGlossaryLanguages = [defaultGlossaryLanguage, defaultGlossaryLanguageSingleWord];

    languageChangeSubject.next(defaultLanguageCode);

    const exactMatch = 'hi. I am a sentence.';
    const nearMatch = 'hi. I am a sentence. ';
    const singleWord = 'sentence';

    // act
    service.disableGlossary();

    const exactMatchTranslation = service.getGlossaryTranslation(exactMatch);
    const nearMatchTranslation = service.getGlossaryTranslation(nearMatch);
    const singleWordTranslation = service.getGlossaryTranslation(singleWord);

    // assert
    expect(exactMatchTranslation).toEqual(exactMatch);
    expect(nearMatchTranslation).toEqual(nearMatch);
    expect(singleWordTranslation).toEqual(singleWord);
  }));

  it('no matches if glossary disabled', fakeAsync(async () => {
    // arrange
    const defaultGlossaryLanguage = new GlossaryLanguage();
    defaultGlossaryLanguage.original = 'hi. I am a sentence.';
    defaultGlossaryLanguage.translated =
      'hello. This sentence has been swapped.';

    const defaultGlossaryLanguageSingleWord = new GlossaryLanguage();
    defaultGlossaryLanguageSingleWord.original = 'sentence';
    defaultGlossaryLanguageSingleWord.translated = 'sentenceReplacement';

    defaultGlossaryLanguages = [
      defaultGlossaryLanguage,
      defaultGlossaryLanguageSingleWord,
    ];

    languageChangeSubject.next(defaultLanguageCode);

    const exactMatch = 'hi. I am a sentence.';
    const nearMatch = 'hi. I am a sentence. ';
    const singleWord = 'sentence';

    // act
    service.disableGlossary();

    const exactMatchTranslation = service.getGlossaryTranslation(exactMatch);
    const nearMatchTranslation = service.getGlossaryTranslation(nearMatch);
    const singleWordTranslation = service.getGlossaryTranslation(singleWord);

    // assert
    expect(exactMatchTranslation).toEqual(exactMatch);
    expect(nearMatchTranslation).toEqual(nearMatch);
    expect(singleWordTranslation).toEqual(singleWord);
  }));

  it('glossary keys are not case sensitive and translated value casing is returned', fakeAsync(async () => {
    // arrange
    const defaultGlossaryLanguage = new GlossaryLanguage();
    defaultGlossaryLanguage.original = 'hi. I am a sentence.';
    defaultGlossaryLanguage.translated = 'hello. This sentence has been swapped.';

    const defaultGlossaryLanguageSingleWord = new GlossaryLanguage();
    defaultGlossaryLanguageSingleWord.original = 'sentence';
    defaultGlossaryLanguageSingleWord.translated = 'sentenceReplacement';

    defaultGlossaryLanguages = [defaultGlossaryLanguage, defaultGlossaryLanguageSingleWord];

    languageChangeSubject.next(defaultLanguageCode);

    const exactMatch = 'hi. I am a sentence.';
    const differentCasingMatch = 'Hi. I am a sentence.';
    const singleWord = 'Sentence';

    // act
    service.enableGlossary();

    const exactMatchTranslation = service.getGlossaryTranslation(exactMatch);
    const differentCasingMatchTranslation = service.getGlossaryTranslation(differentCasingMatch);
    const singleWordTranslation = service.getGlossaryTranslation(singleWord);

    // assert
    expect(exactMatchTranslation).toEqual('hello. This sentence has been swapped.');
    expect(differentCasingMatchTranslation).toEqual('hello. This sentence has been swapped.');
    expect(singleWordTranslation).toEqual('sentenceReplacement');
  }));

  it('null, undefined and empty strings return original value', fakeAsync(async () => {
    // arrange
    const defaultGlossaryLanguage = new GlossaryLanguage();
    defaultGlossaryLanguage.original = 'hi';
    defaultGlossaryLanguage.translated = 'hello';
    defaultGlossaryLanguages = [defaultGlossaryLanguage];

    languageChangeSubject.next(defaultLanguageCode);

    // act
    const nullTranslation = service.getGlossaryTranslation(null);
    const undefinedTranslation = service.getGlossaryTranslation(undefined);
    const emptyStringTranslation = service.getGlossaryTranslation('');

    // assert
    expect(nullTranslation).toBeNull();
    expect(undefinedTranslation).toBeUndefined();
    expect(emptyStringTranslation).toEqual('');
  }));

  describe('can get the original value from a Translated value', () => {
    const expectedOriginalText = 'my original value';
    const expectedTranslatedText = 'my translated value';
    const notAGlossaryTranslation = 'this will not exist';
    beforeEach(() => {
      // arrange
      const glossaryLanguage = new GlossaryLanguage();
      glossaryLanguage.original = expectedOriginalText;
      glossaryLanguage.translated = expectedTranslatedText;

      const testLanguage1 = new GlossaryLanguage();
      testLanguage1.original = 'hi. I am a sentence.';
      testLanguage1.translated = 'hello. This sentence has been swapped.';

      const testLanguage2 = new GlossaryLanguage();
      testLanguage2.original = 'sentencE';
      testLanguage2.translated = 'sentenceReplacement';

      defaultGlossaryLanguages = [glossaryLanguage, testLanguage1, testLanguage2];
      languageChangeSubject.next(defaultLanguageCode);
    });
    it('returns original text when present', fakeAsync(async () => {
      // act
      const originalText = service.getOriginalText(expectedTranslatedText);

      // assert
      expect(originalText).toEqual(expectedOriginalText);
    }));

    it('returns translated text when no glossary entry is found', fakeAsync(async () => {
      // act
      const notFound = service.getOriginalText(notAGlossaryTranslation);

      // assert
      expect(notFound).toEqual(notAGlossaryTranslation);
    }));

    it('getOriginalText is case insensitive and original key casing is returned', fakeAsync(async () => {
      // act
      const originalText = service.getOriginalText('SentenceReplacement');

      // assert
      expect(originalText).toEqual('sentencE');
    }));
  });
});
