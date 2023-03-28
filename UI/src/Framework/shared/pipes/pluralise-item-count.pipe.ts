import { Pipe, PipeTransform } from '@angular/core';
import { isNullOrUndefined } from '@core-module/helpers/is-truthy.function';

@Pipe({
  name: 'pluraliseItemCount',
})
export class PluraliseItemCountPipe implements PipeTransform {
  transform(items: any[], singularText: string, pluralText: string): string {
    if (isNullOrUndefined(items) || isNullOrUndefined(singularText) || isNullOrUndefined(pluralText)) {
      return '';
    }
    const itemsCount = items.length;
    const pluralisedText = itemsCount === 1
      ? singularText
      : pluralText;
    return `${itemsCount} ${pluralisedText}`;
  }
}
