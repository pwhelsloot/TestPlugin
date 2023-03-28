import { roundValue } from './amcs-round.function';

describe('roundValue', () => {
  describe('should return the input value rounded', () => {
    it('when value is null', () => {
      expect(roundValue(null, 1)).toEqual(0);
    });

    it('when decimal is null', () => {
      expect(roundValue(2.1165, null)).toEqual(2);
    });

    it('when double rounding is null', () => {
      expect(roundValue(2.1165, 1, null)).toEqual(2.1);
    });

    it('to zero decimals', () => {
      expect(roundValue(2.1165, 0)).toEqual(2);
    });

    it('to one decimal', () => {
      expect(roundValue(2.1165, 1)).toEqual(2.1);
    });

    it('to two decimals', () => {
      expect(roundValue(2.1165, 2)).toEqual(2.12);
    });

    it('to three decimals', () => {
      expect(roundValue(2.1165, 3)).toEqual(2.117);
    });

    it('with double rounding', () => {
      expect(roundValue(2.1146, 2, true)).toEqual(2.12);
    });

    it('without double rounding', () => {
      expect(roundValue(2.1146, 2, false)).toEqual(2.11);
    });
  });
});
