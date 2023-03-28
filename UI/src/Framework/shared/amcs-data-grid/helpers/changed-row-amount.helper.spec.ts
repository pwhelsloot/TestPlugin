/* tslint:disable:no-unused-variable */

import { ChangedRowAmountHelper } from './changed-row-amount-helper';

describe('ChangedRowAmountHelper', () => {
  let helper: ChangedRowAmountHelper;
  beforeEach(() => {
    helper = new ChangedRowAmountHelper();
  });

  describe('getChangeAmount', () => {
    it('returns 0 with no data', () => {
      const result = helper.getChangeAmount();
      const expected = 0;
      expect(result).toBe(expected);
    });

    it('returns 2 with 2 unique row changes', () => {
      helper.storeEdit('0', 'isExcluded');
      helper.storeEdit('1', 'isExcluded');

      const result = helper.getChangeAmount();
      const expected = 2;
      expect(result).toBe(expected);
    });

    it('returns 4 with 4 unique row changes', () => {
      helper.storeEdit('0', 'isExcluded');
      helper.storeEdit('1', 'isExcluded');
      helper.storeEdit('2', 'isExcluded');
      helper.storeEdit('3', 'isExcluded');
      helper.storeEdit('1', 'Test');
      helper.storeEdit('2', 'Test');

      const result = helper.getChangeAmount();
      const expected = 4;
      expect(result).toBe(expected);
    });

    it('returns 0 after reset', () => {
      helper.storeEdit('0', 'isExcluded');
      helper.storeEdit('1', 'isExcluded');

      const result = helper.getChangeAmount();
      const expected = 2;
      expect(result).toBe(expected);

      helper.reset();

      expect(helper.getChangeAmount()).toBe(0);
    });
  });
});
