import { isNullOrUndefined, isTruthy } from './is-truthy.function';

function isInt(n: number) {
  return n % 1 === 0;
}

// we want to only return an integer if the input is a pure integer (either of string or number type)
// all others (even floats) will yield a 'null' return
export function getIntValue(data: any): number {
  const inputType = typeof data;

  if (inputType === 'string') {
    const onlyDigitsRegex = /^\d+$/;
    const inputIsStrictlyNumeric = onlyDigitsRegex.test(data);

    return inputIsStrictlyNumeric ? parseInt(data, 10) : null;
  } else if (inputType === 'number') {
    return isInt(data) ? data : null;
  } else {
    return null;
  }
}

export function isValidNumber(value: any): boolean {
  return isTruthy(value) && !isNaN(+value);
}

export function convertToNumber(value: any, defaultValue = 0): number {
  return isValidNumber(value) ? +value : defaultValue;
}

export function convertToNumberWithLocale(num: string, locale: string = 'en-US'): number {
  if (isNullOrUndefined(num) || /[a-zA-Z]/g.test(num)) {
    return NaN;
  }
  const { format } = new Intl.NumberFormat(locale);
  const [, decimalSign] = /^0(.)1$/.exec(format(0.1));
  switch (locale) {
    case 'nb':
    case 'es-MX':
    case 'nl':
      return +num
        .replace(new RegExp(`[^,\\d]`, 'g'), '')
        .replace(',', '.');
    default:
      return +num
        .replace(new RegExp(`[^${decimalSign}\\d]`, 'g'), '')
        .replace(decimalSign, '.');
  }
}
