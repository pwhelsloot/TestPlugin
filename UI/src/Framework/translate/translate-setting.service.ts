import { Injectable } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { CoreUserPreferenceKeys } from '@core-module/models/preferences/core-user-preference-keys.model';
import { BaseService } from '@core-module/services/base.service';
import { CoreUserPreferencesService } from '@core-module/services/preferences/core-user-preferences.service';
import { UiLanguage } from '@coremodels/language/ui-language.model';
import { environment } from '@environments/environment';
import { Observable, ReplaySubject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class TranslateSettingService extends BaseService {
  uiLanguages: UiLanguage[] = [];
  selectedLanguage: Observable<string>;

  constructor(private userPreferencesService: CoreUserPreferencesService) {
    super();

    this.selectedLanguage = this.innerSelectedLanguage;

    for (const language of environment.languages) {
      const uiLanguage = new UiLanguage();
      uiLanguage.code = language;
      this.uiLanguages.push(uiLanguage);
    }

    this.userPreferencesService
      .get<string>(CoreUserPreferenceKeys.uiLanguage, 'en')
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((language: string) => {
        if (isTruthy(language)) {
          this.setLanguage(language);
        } else {
          const safeNavigatorLanguage = this.getUnauthenticatedLanguage();
          this.userPreferencesService.save<string>(CoreUserPreferenceKeys.uiLanguage, safeNavigatorLanguage);
          this.setLanguage(safeNavigatorLanguage);
        }
      });
  }

  private innerSelectedLanguage: ReplaySubject<string> = new ReplaySubject<string>(1);

  getUnauthenticatedLanguage(): string {
    let tempLanguage: string;
    if (localStorage && localStorage['language']) {
      tempLanguage = localStorage['language'];
    } else {
      tempLanguage = navigator.language;
    }

    if (!environment.languages.find((x) => x === tempLanguage)) {
      // Language not found in list of supported languages so default to 'en'
      tempLanguage = environment.languages[0];
      // Also update localStorage so unsupported language is no longer stored
      if (localStorage) {
        localStorage['language'] = tempLanguage;
      }
    }

    return tempLanguage;
  }

  setLanguage(language: string) {
    if (isTruthy(language)) {
      this.innerSelectedLanguage.next(language);
      if (localStorage) {
        localStorage['language'] = language;
      }
    }
  }
}
