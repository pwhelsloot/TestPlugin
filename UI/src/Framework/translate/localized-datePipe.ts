import { DatePipe } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { TranslateService } from '@ngx-translate/core';
import { ObjectToDatePipe } from './object-to-date.pipe';

/**
 * @todo Move to shared/pipes folder
 */
@Pipe({
  name: 'localizedDate',
  pure: false
})
export class LocalizedDatePipe implements PipeTransform {

  constructor(private translateService: TranslateService,
    private objectToDatePipe: ObjectToDatePipe) {
  }

  transform(value: any, pattern: string = 'mediumDate', timezone?: string): any {
    if (!isTruthy(value) || !isTruthy(this.translateService) || !isTruthy(this.translateService.currentLang)) {
      return '';
    } else {
      // Convert value to Date. DatePipe cannot handle strings or numbers in iOS browsers.
      const dateValue = this.objectToDatePipe.transform(value);
      const datePipe = new DatePipe(this.translateService.currentLang);
      return datePipe.transform(dateValue, pattern, timezone);
    }
  }
}
