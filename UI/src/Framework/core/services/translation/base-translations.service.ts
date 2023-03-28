import { Injectable } from '@angular/core';
import { BaseService } from '@coreservices/base.service';
import { TranslateService } from '@ngx-translate/core';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { Observable, ReplaySubject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';

@Injectable()
export abstract class BaseTranslationsService extends BaseService {
  /**
   * Active Translations according to selected locale
   *
   * @type {Observable<string[]>}
   * @memberof BaseTranslationsService
   */
  translations: Observable<string[]>;

  /**
   * Active locale, default is "en"
   *
   * @memberof BaseTranslationsService
   */
  locale = 'en';

  constructor(protected translateService: TranslateService, protected localSettings: TranslateSettingService) {
    super();
    this.setupLanguageStreams();
  }

  private innerTranslations: ReplaySubject<string[]> = new ReplaySubject<string[]>(1);

  /**
   * Get a translation
   *
   * @param {string} translationKey
   * @returns
   * @memberof BaseTranslationsService
   */
  getTranslation(translationKey: string) {
    return this.translateService.instant(translationKey);
  }

  /**
   * Sets language to language in localstorage or default
   *
   * @memberof BaseTranslationsService
   */
  setLanguageToExistingOrDefault() {
    const storedLang: string = this.localSettings.getUnauthenticatedLanguage();
    if (storedLang && storedLang !== '') {
      this.updateLanguage(storedLang);
    }
  }

  /**
   * Updates the language, sets both locale and locale in translateService
   *
   * @private
   * @param {string} language
   * @memberof BaseTranslationsService
   */
  private updateLanguage(language: string) {
    this.locale = language;
    this.translateService.use(language);
  }

  /**
   * Setup language streams on Service creation
   *
   * @private
   * @memberof BaseTranslationsService
   */
  private setupLanguageStreams() {
    this.localSettings.selectedLanguage
      .pipe(
        takeUntil(this.unsubscribe),
        filter((language: string) => language && language !== '')
      )
      .subscribe((language: string) => {
        if (language !== this.translateService.currentLang) {
          this.updateLanguage(language);
        } else {
          this.generateTranslations();
        }
      });

    this.translations = this.innerTranslations.asObservable();

    this.translateService.onLangChange.pipe(takeUntil(this.unsubscribe)).subscribe(() => {
      this.generateTranslations();
    });
  }

  /**
   * Add all your translations here, this is a replacement for generateTranslations
   * Used to extract translations
   *
   * @abstract
   * @memberof BaseTranslationsService
   */
  abstract mapTranslations(translationArray: string[]): void;

  private generateTranslations() {
    if (this.translateService.langs.length > 0) {
      const translationArray = new Array<string>();
      this.mapTranslations(translationArray);
      this.innerTranslations.next(translationArray);
    }
  }
}
