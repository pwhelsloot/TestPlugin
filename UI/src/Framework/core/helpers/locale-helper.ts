import { NumberSeparator } from './number-separator';

export function getNumberSeparators(language: string): NumberSeparator {
  switch (language) {
    case 'nb':
    case 'es-MX':
    case 'nl':
      return new NumberSeparator('.', ',');
    case 'fr-FR':
    case 'de-DE':
      return new NumberSeparator(' ', ',');
    case 'en-US':
    case 'en-AU':
    case 'en-GB':
    default:
      return new NumberSeparator(',', '.');
  }
}
export function getNumberFormatLocale(language: string): string {
  switch (language) {
    case 'nb':
    case 'es-MX':
    case 'nl':
      return 'nl';
    case 'fr-FR':
    case 'de-DE':
      return 'fr-FR';
    case 'en-US':
    case 'en-AU':
    case 'en-GB':
    default:
      return 'en-US';
  }
}
