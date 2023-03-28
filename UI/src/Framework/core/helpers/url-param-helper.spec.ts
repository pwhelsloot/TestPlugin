import { parseEntityId } from './url-param-helper';

describe('parseEntityId', () => {
  describe('should return "null" for input', () => {
    it('that is a non-numeric text string', () => {
      expect(parseEntityId('asdf')).toBeNull();
    });

    it('that is an empty string', () => {
      expect(parseEntityId('')).toBeNull();
    });

    it('that has a value of zero', () => {
      expect(parseEntityId('0')).toBeNull();
    });
  });

  describe('should return the correct integer for input', () => {
    it('that is a positive integer value', () => {
      expect(parseEntityId('123')).toEqual(123);
    });
  });
});
