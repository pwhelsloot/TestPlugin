import { getNumberFormatLocale, getNumberSeparators } from './locale-helper';
import { NumberSeparator } from './number-separator';

describe('getNumberSeparators', () => {
  describe('should return "," and "." as delimiters for invalid input', () => {
    it('that is a non-matching language text string', () => {
      expect(getNumberSeparators('asdf')).toEqual(new NumberSeparator(',', '.'));
    });

    it('that is an empty string', () => {
      expect(getNumberSeparators('')).toEqual(new NumberSeparator(',', '.'));
    });

  });

  describe('should return the correct delimiters for language', () => {
    it('that is en-US', () => {
      expect(getNumberSeparators('en-US')).toEqual(new NumberSeparator(',', '.'));
    });

    it('that is en-AU', () => {
      expect(getNumberSeparators('en-AU')).toEqual(new NumberSeparator(',', '.'));
    });

    it('that is en-GB', () => {
      expect(getNumberSeparators('en-GB')).toEqual(new NumberSeparator(',', '.'));
    });

    it('that is nb', () => {
      expect(getNumberSeparators('nb')).toEqual(new NumberSeparator('.', ','));
    });

    it('that is es-MX', () => {
      expect(getNumberSeparators('es-MX')).toEqual(new NumberSeparator('.', ','));
    });

    it('that is nl', () => {
      expect(getNumberSeparators('nl')).toEqual(new NumberSeparator('.', ','));
    });

    it('that is fr-FR', () => {
      expect(getNumberSeparators('fr-FR')).toEqual(new NumberSeparator(' ', ','));
    });

    it('that is de', () => {
      expect(getNumberSeparators('de-DE')).toEqual(new NumberSeparator(' ', ','));
    });
  });
});


describe('getNumberFormatLocale', () => {
  describe('should return "en" invalid input', () => {
    it('that is a non-matching language text string', () => {
      expect(getNumberFormatLocale('asdf')).toEqual('en-US');
    });

    it('that is an empty string', () => {
      expect(getNumberFormatLocale('')).toEqual('en-US');
    });

  });

  describe('should return the correct delimiters for language', () => {
    it('that is en-US', () => {
      expect(getNumberFormatLocale('en-US')).toEqual('en-US');
    });

    it('that is en-AU', () => {
      expect(getNumberFormatLocale('en-AU')).toEqual('en-US');
    });

    it('that is en-GB', () => {
      expect(getNumberFormatLocale('en-GB')).toEqual('en-US');
    });

    it('that is nb', () => {
      expect(getNumberFormatLocale('nb')).toEqual('nl');
    });

    it('that is es-MX', () => {
      expect(getNumberFormatLocale('es-MX')).toEqual('nl');
    });

    it('that is nl', () => {
      expect(getNumberFormatLocale('nl')).toEqual('nl');
    });

    it('that is fr-FR', () => {
      expect(getNumberFormatLocale('fr-FR')).toEqual('fr-FR');
    });

    it('that is de', () => {
      expect(getNumberFormatLocale('de-DE')).toEqual('fr-FR');
    });
  });
});
