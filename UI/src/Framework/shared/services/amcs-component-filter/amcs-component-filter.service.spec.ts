import { ComponentFilter } from '@shared-module/components/amcs-component-filter/component-filter.model';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ComponentFilterService } from './amcs-component-filter.service';

describe('ComponentFilterService', () => {
  let target: ComponentFilterService;

  const destroy: Subject<void> = new Subject();
  const observer: jasmine.Spy = jasmine.createSpy('Map service Observer');

  beforeEach(() => {

    target = new ComponentFilterService();
    target.appliedFilters$.pipe(takeUntil(destroy)).subscribe(observer);
  });

  afterEach(() => {
    destroy.next();
    observer.calls.reset();
  });

  afterAll(() => {
    destroy.complete();
  });

  it('should create the service', () => {
      expect(target).toBeDefined();
  });

  describe('applyFilter', () => {
    it('should emit the applied filter', () => {
      const filter = new ComponentFilter();
      filter.filterTypeId = 1;

      target.applyFilter(filter);

      expect(observer).toHaveBeenCalledTimes(1);
      expect(observer).toHaveBeenCalledOnceWith([filter]);
    });
  });
});
