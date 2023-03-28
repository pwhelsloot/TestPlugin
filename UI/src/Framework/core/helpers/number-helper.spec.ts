import { convertToNumberWithLocale, getIntValue } from './number-helper';

describe('getIntValue', () => {
  describe('should return "null" for input', () => {
    it('that is a non-numeric text string', () => {
      expect(getIntValue('asdf')).toBeNull();
    });

    it('that is an empty string', () => {
      expect(getIntValue('')).toBeNull();
    });

    it('that is a string that represents a number with a decimal place', () => {
      expect(getIntValue('1.3')).toBeNull();
    });

    it('that is a string that starts with a number but as a non-digit character', () => {
      expect(getIntValue('0A')).toBeNull();
    });

    it('that is a number with a decimal place', () => {
      expect(getIntValue(1.3)).toBeNull();
    });

    it('that is an object', () => {
      expect(getIntValue({})).toBeNull();
    });
  });

  describe('should return the correct integer for input', () => {
    it('that is a string type which represents an integer', () => {
      expect(getIntValue('1')).toEqual(1);
    });

    it('that is a string type with a leading zero which represents an integer', () => {
      expect(getIntValue('08')).toEqual(8);
    });

    it('that is a number type which represents an integer', () => {
      expect(getIntValue(11)).toEqual(11);
    });
  });
});

describe('convertToNumberWithLocale', () => {
  describe('should return NaN for input', () => {
    it('that is null', () => {
      expect(convertToNumberWithLocale(null, 'en-US')).toEqual(NaN);
    });
    it('that is a text', () => {
      expect(convertToNumberWithLocale('asdf', 'en-US')).toEqual(NaN);
    });
  });

  describe('should return valid integer for input', () => {
    it('that is a string of locale en-US', () => {
      expect(convertToNumberWithLocale('10,000.1234', 'en-US')).toEqual(10000.1234);
    });
    it('that is a string of locale en-GB', () => {
      expect(convertToNumberWithLocale('10,000.1234', 'en-GB')).toEqual(10000.1234);
    });
    it('that is a string of locale en-AU', () => {
      expect(convertToNumberWithLocale('10,000.1234', 'en-AU')).toEqual(10000.1234);
    });
    it('that is a string of locale de-DE', () => {
      expect(convertToNumberWithLocale('10 000,1234', 'de-DE')).toEqual(10000.1234);
    });
    it('that is a string of locale fr-FR', () => {
      expect(convertToNumberWithLocale('10 000,1234', 'fr-FR')).toEqual(10000.1234);
    });
    it('that is a string of locale nb', () => {
      expect(convertToNumberWithLocale('10.000,1234', 'nb')).toEqual(10000.1234);
    });
    it('that is a string of locale es-MX', () => {
      expect(convertToNumberWithLocale('10.000,1234', 'es-MX')).toEqual(10000.1234);
    });
    it('that is a string of locale nl', () => {
      expect(convertToNumberWithLocale('10.000,1234', 'nl')).toEqual(10000.1234);
    });
    it('that is a string without any locale', () => {
      expect(convertToNumberWithLocale('10,000.1234')).toEqual(10000.1234);
    });
  });
});
