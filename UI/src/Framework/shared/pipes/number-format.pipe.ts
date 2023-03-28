import { DecimalPipe } from '@angular/common';
import { OnDestroy, Pipe, PipeTransform } from '@angular/core';
import { getNumberFormatLocale } from '@core-module/helpers/locale-helper';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { Subscription } from 'rxjs';

@Pipe({
  name: 'numberFormatter'
})
export class NumberFormatPipe implements PipeTransform, OnDestroy {
  locale = 'en';
  constructor(private decimalPipe: DecimalPipe, private translateSettingService: TranslateSettingService) {
    this.selectedLanguageSubscription = this.translateSettingService.selectedLanguage
      .subscribe((language: string) => {
        this.locale = getNumberFormatLocale(language);
      });
  }
  private selectedLanguageSubscription: Subscription;
  transform(val: string | number, digitsInfo: string = '1.0-2'): string {
    return this.decimalPipe.transform(val, digitsInfo, this.locale);
  }
  ngOnDestroy() {
    this.selectedLanguageSubscription?.unsubscribe();

  }
}
