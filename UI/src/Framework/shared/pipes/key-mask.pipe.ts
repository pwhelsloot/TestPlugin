import { Pipe, PipeTransform } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';

@Pipe({
  name: 'keyMask'
})

export class KeyMaskPipe implements PipeTransform {
  transform(plainText: string): string {
    if (!isTruthy(plainText)) {
      return plainText;
    }
    return plainText + '***';
  }
}
