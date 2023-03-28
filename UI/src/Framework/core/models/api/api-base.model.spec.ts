import { ApiBaseModel } from './api-base.model';

class DummyModel extends ApiBaseModel {
  name: string;
  id: number;
}

describe('ApiBaseModel', () => {
  let sut = new DummyModel();

  beforeEach(() => {
    sut = new DummyModel();
    sut.id = 1234;
    sut.name = 'Filtering is fun';
  });

  describe('filterByValues', () => {
    it('should find value in numeric property', () => {
      const filterResult = sut.filterByValues('23', [sut.id, sut.name]);
      expect(filterResult).toEqual(true);
    });

    it('should find value in string property', () => {
      const filterResult = sut.filterByValues('is', [sut.id, sut.name]);
      expect(filterResult).toEqual(true);
    });

    it('should find value in string property (ignoring search term casing)', () => {
      const filterResult = sut.filterByValues('FI', [sut.id, sut.name]);
      expect(filterResult).toEqual(true);
    });

    it('should find value in string property (ignoring prop value casing)', () => {
      const filterResult = sut.filterByValues('fi', [sut.id, sut.name]);
      expect(filterResult).toEqual(true);
    });

    it('should be false if no matches found', () => {
      const filterResult = sut.filterByValues('rhino', [sut.id, sut.name]);
      expect(filterResult).toEqual(false);
    });
  });

  // Test relies on build agent performance, that is not consistent and so this fails unnecessarily sometimes.
  // describe('filterByValues performance', () => {
  //   it('should run 10000 searches in less than 10ms', () => {
  //     const startTime = performance.now();

  //     // 5000 iterations, over 2 fields = 10,000 search operations
  //     for (let i = 0; i < 5000; i++) {
  //       sut.filterByValues('is', [sut.id, sut.name]);
  //     }

  //     const endTime = performance.now();
  //     const durationInMs = endTime - startTime;

  //     // was taking less than 3ms when run locally, using Chrome (headless)
  //     expect(durationInMs).toBeLessThan(10);
  //   });
  // });
});
