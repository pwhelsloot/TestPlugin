import { TestBed } from '@angular/core/testing';
import { CountCollectionModel } from '@core-module/models/api/count-collection.model';
import { FilterOperation } from '@core-module/models/api/filters/filter-operation.enum';
import { IFilter } from '@core-module/models/api/filters/iFilter';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { ModalGridSelectorServiceAdapter } from '@core-module/services/forms/modal-grid-selector.adapter';
import { BehaviorSubject, Subject } from 'rxjs';
import { ModalDetailsService } from './modal-details.service';

describe('ModalDetailsService', () => {
  let service: ModalDetailsService;
  const mockAdapter = {} as ModalGridSelectorServiceAdapter;
  let mockAdapterItemsSubject: Subject<CountCollectionModel<ILookupItem>>;
  const loading = new BehaviorSubject<boolean>(false);
  const externalClose = new Subject<any>();
  let countCollection: CountCollectionModel<ILookupItem>;
  const selectedItemId = 2;
  const mockFilter: IFilter = { filterOperation: FilterOperation.Contains, name: 'test', value: 1 };
  const items: ILookupItem[] = [
    { id: 1, description: 'd1' },
    { id: 2, description: 'd2' },
    { id: 3, description: 'd3' }
  ];
  const observer: jasmine.Spy = jasmine.createSpy('ModalDetailsService Observer');

  beforeEach(() => {
    countCollection = new CountCollectionModel<ILookupItem>(items, 10);
    mockAdapterItemsSubject = new Subject<CountCollectionModel<ILookupItem>>();
    mockAdapter.itemsObservable$ = mockAdapterItemsSubject;
    mockAdapter.requestItems = (page: number, filters: IFilter[]): any => {};
    items.forEach((x) => x['isSelected'] = false);
    TestBed.configureTestingModule({
      providers: [ModalDetailsService]
    });
    service = TestBed.inject(ModalDetailsService);
  });

  afterEach(() => {
    observer.calls.reset();
  });

  it('calling initialise causes adapter.requestItems to be called', () => {
    // Arrange
    const isMultiSelect = false;
    const autoClose = true;
    spyOn(mockAdapter, 'requestItems').and.callThrough();

    // Act
    service.initialise([[selectedItemId], [], mockAdapter, [mockFilter], isMultiSelect, autoClose], loading, externalClose);

    // Assert
    expect(service).toBeTruthy();
    expect(service.items).toBeUndefined();
    expect(service.modalLoading).toBe(true);
    expect(service.columns).toEqual([]);
    expect(mockAdapter.requestItems).toHaveBeenCalledOnceWith(0, [mockFilter]);
  });

  it('calling initialise and pushing onto items observable causes adapter.requestItems to be called and items to be assigned', () => {
    // Arrange
    const isMultiSelect = false;
    const autoClose = true;
    spyOn(mockAdapter, 'requestItems').and.callThrough();

    // Act
    service.initialise([[selectedItemId], [], mockAdapter, [mockFilter], isMultiSelect, autoClose], loading, externalClose);
    mockAdapterItemsSubject.next(countCollection);

    // Assert
    expect(service).toBeTruthy();
    expect(service.columns).toEqual([]);
    expect(service.items).toEqual(countCollection);
    expect(service.items.results.find((x) => x.id === selectedItemId).isSelected).toBe(true);
    expect(service.isAnItemSelected).toBeTrue();
    expect(service.modalLoading).toBe(false);
    expect(mockAdapter.requestItems).toHaveBeenCalledOnceWith(0, [mockFilter]);
  });

    it('calling initialise and pushing onto items observable causes adapter.requestItems to be called. No selectedItemId = no selected items', () => {
      // Arrange
      const isMultiSelect = false;
      const autoClose = true;
      spyOn(mockAdapter, 'requestItems').and.callThrough();

      // Act
      service.initialise([[], [], mockAdapter, [mockFilter], isMultiSelect, autoClose], loading, externalClose);
      mockAdapterItemsSubject.next(countCollection);

      // Assert
      expect(service).toBeTruthy();
      expect(service.columns).toEqual([]);
      expect(service.items).toEqual(countCollection);
      expect(service.items.results.every((x) => x.isSelected === false)).toBe(true);
      expect(service.isAnItemSelected).toBeFalse();
      expect(service.modalLoading).toBe(false);
      expect(mockAdapter.requestItems).toHaveBeenCalledOnceWith(0, [mockFilter]);
    });

  it('calling onPageChange causes adapter.requestItems to be called', () => {
    // Arrange
    const isMultiSelect = false;
    const autoClose = false;
    service.initialise([[selectedItemId], [], mockAdapter, [mockFilter], isMultiSelect, autoClose], loading, externalClose);
    spyOn(mockAdapter, 'requestItems').and.callThrough();

    // Act
    service.onPageChange(1);

    // Assert
    expect(mockAdapter.requestItems).toHaveBeenCalledOnceWith(1, [mockFilter]);

    // Act
    service.onPageChange(2);

    // Assert
    expect(mockAdapter.requestItems).toHaveBeenCalledTimes(2);
    expect(mockAdapter.requestItems).toHaveBeenCalledWith(2, [mockFilter]);
  });

  it('calling rowSelected causes externalClose.next to be called with the first rowId', () => {
    // Arrange
    const isMultiSelect = false;
    const autoClose = true;
    service.initialise([[selectedItemId], [], mockAdapter, [mockFilter], isMultiSelect, autoClose], loading, externalClose);
    mockAdapterItemsSubject.next(countCollection);
    const subscription = externalClose.subscribe(observer);

    // Act
    service.rowSelected([3, 6]);

    // Assert
    // Weirdly we expect isSelected: false here as it's really the amcs-grid which changes this
    expect(observer).toHaveBeenCalledOnceWith([{ id: 3, description: 'd3', isSelected: false }]);

    subscription.unsubscribe();
  });

    it('calling rowSelected with autoClose false shouldn\'t call externalClose.next', () => {
      // Arrange
      const isMultiSelect = false;
      const autoClose = false;
      service.initialise([[selectedItemId], [], mockAdapter, [mockFilter], isMultiSelect, autoClose], loading, externalClose);
      mockAdapterItemsSubject.next(countCollection);
      const subscription = externalClose.subscribe(observer);

      // Act
      service.rowSelected([3, 6]);

      // Assert
      expect(observer).toHaveBeenCalledTimes(0);

      subscription.unsubscribe();
    });

    it('calling cancel causes externalClose.next to be called with empty array', () => {
      // Arrange
      const isMultiSelect = false;
      const autoClose = true;
      service.initialise([[selectedItemId], [], mockAdapter, [mockFilter], isMultiSelect, autoClose], loading, externalClose);
      mockAdapterItemsSubject.next(countCollection);
      const subscription = externalClose.subscribe(observer);

      // Act
      service.cancel();

      // Assert
      expect(observer).toHaveBeenCalledOnceWith([]);

      subscription.unsubscribe();
    });
});
