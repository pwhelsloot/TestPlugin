import { Injectable } from '@angular/core';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { TranslateService } from '@ngx-translate/core';
import { BaseTranslationsService } from '@core-module/services/translation/base-translations.service';

@Injectable()
export class TemplateAppTranslationsService extends BaseTranslationsService {

  constructor(readonly translateService: TranslateService, readonly localSettings: TranslateSettingService) {
    super(translateService, localSettings);
  }

   mapTranslations(translationArray: string[]): void {
    translationArray['title.app.home'] = this.translateService.instant('title.app.home');
    translationArray['title.app.dashboard'] = this.translateService.instant('title.app.dashboard');
    translationArray['title.app.ingredients'] = this.translateService.instant('title.app.ingredients');
    translationArray['title.app.unitsOfMeasurement'] = this.translateService.instant('title.app.unitsOfMeasurement');
    translationArray['title.app.cssExample'] = this.translateService.instant('title.app.cssExample');
    translationArray['title.app.stepperExample'] = this.translateService.instant('title.app.stepperExample');
    translationArray['title.app.mapExample'] = this.translateService.instant('title.app.mapExample');
    translationArray['title.app.snippetsExample'] = this.translateService.instant('title.app.snippetsExample');
    translationArray['title.app.amcsPlatform'] = this.translateService.instant('title.app.amcsPlatform');

   }
}
