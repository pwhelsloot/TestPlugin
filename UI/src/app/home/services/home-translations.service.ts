import { Injectable } from '@angular/core';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { TranslateService } from '@ngx-translate/core';
import { BaseTranslationsService } from '@core-module/services/translation/base-translations.service';

@Injectable()
export class HomeTranslationsService extends BaseTranslationsService {
  constructor(readonly translateService: TranslateService, readonly localSettings: TranslateSettingService) {
    super(translateService, localSettings);
  }

  mapTranslations(translationArray: string[]): void {
    translationArray['search.component.AllTab'] = this.translateService.instant('search.component.AllTab');
    translationArray['search.component.CustomerTab'] = this.translateService.instant('search.component.CustomerTab');
    translationArray['search.component.ContractTab'] = this.translateService.instant('search.component.ContractTab');
    translationArray['search.component.ServiceLocationTab'] = this.translateService.instant('search.component.ServiceLocationTab');
    translationArray['customer-search.ResultsHeader'] = this.translateService.instant('customer-search.ResultsHeader');
    translationArray['customer-search.Add'] = this.translateService.instant('customer-search.Add');
    translationArray['contract-search.ResultsHeader'] = this.translateService.instant('contract-search.ResultsHeader');
    translationArray['service-locations-search.ResultsHeader'] = this.translateService.instant('service-locations-search.ResultsHeader');
    // Language naming must follow 'home.myProfile.language.'+region/local so that it can be linked in code
    translationArray['home.myProfile.language.en-GB'] = this.translateService.instant('home.myProfile.language.en-GB');
    translationArray['home.myProfile.language.en'] = this.translateService.instant('home.myProfile.language.en');
    translationArray['home.myProfile.language.nb'] = this.translateService.instant('home.myProfile.language.nb');
    translationArray['customer-search.Add'] = this.translateService.instant('customer-search.Add');
    translationArray['search.customers'] = this.translateService.instant('search.customers');
    translationArray['search.Contracts'] = this.translateService.instant('search.Contracts');
    translationArray['search.serviceLocations'] = this.translateService.instant('search.serviceLocations');
    translationArray['home.myProfile.image.size'] = this.translateService.instant('home.myProfile.image.size');
  }
}
