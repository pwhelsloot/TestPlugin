import { EventEmitter, Injectable } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { BaseService } from '@coreservices/base.service';
import { MyProfileForm } from '@app/home/models/my-profile.form.model';
import { HomeTranslationsService } from '@app/home/services/home-translations.service';
import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { takeUntil, combineLatest } from 'rxjs/operators';
import { PrinterLookup } from '../models/printer-lookup.model';
import { MyProfileData } from '@core-module/models/external-dependencies/profile/my-profile-data.model';
import { ApiBusinessService } from '@core-module/services/service-structure/api-business.service';
import { CoreUserPreferencesService } from '@core-module/services/preferences/core-user-preferences.service';
import { CoreUserPreferenceKeys } from '@core-module/models/preferences/core-user-preference-keys.model';

@Injectable()
export class MyProfileFormService extends BaseService {
  form: AmcsFormGroup;
  initialised = new EventEmitter<boolean>();
  selectedLanguage: string;
  printerLookups: PrinterLookup[];

  constructor(
    private readonly translationService: HomeTranslationsService,
    private readonly translateSettingsService: TranslateSettingService,
    private readonly businessService: ApiBusinessService,
    private readonly userPreferencesService: CoreUserPreferencesService,
    private readonly formBuilder: FormBuilder
  ) {
    super();

    this.translateSettingsService.selectedLanguage.pipe(takeUntil(this.unsubscribe)).subscribe((language: string) => {
      if (language && language !== '') {
        this.selectedLanguage = language;
      } else {
        // Default to en
        this.selectedLanguage = 'en';
      }
    });

    this.translationService.translations
      .pipe(
        takeUntil(this.unsubscribe),
        combineLatest(this.businessService.getArray<PrinterLookup>([], PrinterLookup, { isCoreRequest: true }))
      )
      .subscribe(([translations, printers]) => {
        for (const language of this.translateSettingsService.uiLanguages) {
          language.description = translations['home.myProfile.language.' + language.code];
        }
        this.printerLookups = printers;
      });
  }

  initialise() {
    const myProfileData = new MyProfileData();
    myProfileData.uiLanguage = this.selectedLanguage;
    this.form = new AmcsFormGroup(this.formBuilder.group(MyProfileForm.build(myProfileData)));
  }

  save() {
    if (this.form.formGroup.valid) {
      const codeLookup = this.translateSettingsService.uiLanguages.find((x) => x.code === this.form.formGroup.controls.uiLanguage.value);
      if (codeLookup) {
        this.userPreferencesService.save<String>(CoreUserPreferenceKeys.uiLanguage, codeLookup.code);
        this.translateSettingsService.setLanguage(codeLookup.code);
      }
      this.form.formGroup.markAsPristine();
    }
  }
}
