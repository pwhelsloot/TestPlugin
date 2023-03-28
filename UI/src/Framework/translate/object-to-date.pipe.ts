import { Pipe, PipeTransform } from '@angular/core';
import { AmcsDate } from '@core-module/models/date/amcs-date.model';

/**
 * @todo Move to shared/pipes folder
 */
@Pipe({
  name: 'objectToDate'
})
export class ObjectToDatePipe implements PipeTransform {
  transform(value: any): Date {
    return AmcsDate.convertObjectToDate(value);
  }
}
