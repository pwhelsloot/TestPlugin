import { Pipe, PipeTransform } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';

@Pipe({
  name: 'gridEnumeration'
})
export class GridEnumerationPipe implements PipeTransform {
  transform(value: string, args: { key: number; value: string }[]): string {
    if (isTruthy(value) && isTruthy(args) && args.length > 0) {
      const val = args.find(x => x.key === +value);
      return isTruthy(val) ? val.value : null;
    } else {
      return null;
    }
  }
}
