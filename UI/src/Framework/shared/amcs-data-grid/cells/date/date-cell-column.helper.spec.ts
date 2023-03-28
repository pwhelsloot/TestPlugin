import { AmcsDateCellColumnHelper } from './date-cell-column.helper';

describe('AmcsDateCellColumnHelper', () => {
  describe('isGreaterThan', () => {
    it('returns true when same as min date', () => {
      const expected = true;

      const date = new Date();
      const min = new Date();

      const result = AmcsDateCellColumnHelper.isGreaterThan(date, min);

      expect(result).toBe(expected);
    });

    it('returns true when after min date', () => {
      const expected = true;

      const date = new Date();
      date.setDate(new Date().getDate() + 3);

      const min = new Date();

      const result = AmcsDateCellColumnHelper.isGreaterThan(date, min);

      expect(result).toBe(expected);
    });

    it('returns false when before min date', () => {
      const expected = false;

      const date = new Date();
      date.setDate(new Date().getDate() - 3);

      const min = new Date();

      const result = AmcsDateCellColumnHelper.isGreaterThan(date, min);

      expect(result).toBe(expected);
    });
  });

  describe('isInBetween', () => {
    it('returns true when between', () => {
      const expected = true;

      const date = new Date();

      const min = new Date();
      min.setDate(new Date().getDate() - 5);

      const max = new Date();
      max.setDate(new Date().getDate() + 3);

      const result = AmcsDateCellColumnHelper.isInBetween(date, min, max);

      expect(result).toBe(expected);
    });
    it('returns false when below min', () => {
      const expected = false;

      const date = new Date();
      date.setDate(new Date().getDate() - 1);

      const min = new Date();

      const max = new Date();
      max.setDate(new Date().getDate() + 1);

      const result = AmcsDateCellColumnHelper.isInBetween(date, min, max);

      expect(result).toBe(expected);
    });

    it('returns false when above max', () => {
      const expected = false;

      const date = new Date();
      date.setDate(new Date().getDate() + 2);

      const min = new Date();

      const max = new Date();
      max.setDate(new Date().getDate() + 1);

      const result = AmcsDateCellColumnHelper.isInBetween(date, min, max);

      expect(result).toBe(expected);
    });

    it('returns true when same as max date', () => {
      const expected = true;

      const date = new Date();
      date.setDate(new Date().getDate() + 1);

      const min = new Date();

      const max = new Date();
      max.setDate(new Date().getDate() + 1);

      const result = AmcsDateCellColumnHelper.isInBetween(date, min, max);

      expect(result).toBe(expected);
    });

    it('returns true when same as min date', () => {
      const expected = true;

      const date = new Date();
      const min = new Date();
      const max = new Date();
      max.setDate(new Date().getDate() + 1);

      const result = AmcsDateCellColumnHelper.isInBetween(date, min, max);

      expect(result).toBe(expected);
    });
  });
});
