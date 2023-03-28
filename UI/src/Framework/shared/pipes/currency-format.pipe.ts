import { getCurrencySymbol } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { NumberFormatPipe } from './number-format.pipe';

@Pipe({
  name: 'currencyFormatter'
})
export class CurrencyFormatPipe implements PipeTransform {
  constructor(private numberFormatter: NumberFormatPipe) { }

  transform(val: string | number, currencyCode: string = null, digitsInfo: string = '1.2-2'): string {
    return `${isTruthy(currencyCode) ? getCurrencySymbol(currencyCode, 'narrow') : ''}${this.numberFormatter.transform(val, digitsInfo)}`;
  }
}
