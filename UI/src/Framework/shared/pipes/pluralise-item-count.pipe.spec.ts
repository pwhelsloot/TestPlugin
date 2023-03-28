import { PluraliseItemCountPipe } from './pluralise-item-count.pipe';

describe('pluraliseItemCountPipe', () => {
  const pluraliseItemCountPipe = new PluraliseItemCountPipe();
  describe('should return correct title for input', () => {
    it('that is an array of length 0 and has singular and plural text provided', () => {
      expect(pluraliseItemCountPipe.transform([], 'job found', 'jobs found')).toEqual('0 jobs found');
    });

    it('that is an array of length 1 and has singular and plural text provided', () => {
      expect(pluraliseItemCountPipe.transform([5], 'job found', 'jobs found')).toEqual('1 job found');
    });

    it('that is an array of length 2 and has singular and plural text provided', () => {
      expect(pluraliseItemCountPipe.transform([1, 2], 'job found', 'jobs found')).toEqual('2 jobs found');
    });
  });

  describe('should return empty string input', () => {
    it('that is an array of length 2, has plural text provided but does not have singular text provided ', () => {
      expect(pluraliseItemCountPipe.transform([1, 2], null, 'jobs found')).toEqual('');
    });
    it('that is a null array, has plural text provided but does not have singular text provided ', () => {
      expect(pluraliseItemCountPipe.transform(null, 'job found', 'jobs found')).toEqual('');
    });
    it('that is an array of length 2, has singular text provided but does not have plural text provided ', () => {
      expect(pluraliseItemCountPipe.transform([1, 2], 'job found', null)).toEqual('');
    });
  });
});
