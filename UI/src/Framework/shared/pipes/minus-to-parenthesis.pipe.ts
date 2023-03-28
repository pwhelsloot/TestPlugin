import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'minusToParenthesis'
})
export class MinusToParenthesis implements PipeTransform {

  transform(value: string): string {
    if (value !== null && value !== undefined && value.length > 0) {
      return value.charAt(0) === '-' ?
        '(' + value.substring(1, value.length) + ')' :
        value;
    } else {
      return null;
    }
  }
}
