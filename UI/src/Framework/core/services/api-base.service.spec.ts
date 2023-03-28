import { HttpClientTestingModule } from '@angular/common/http/testing';
import { fakeAsync, TestBed } from '@angular/core/testing';
import { CountCollectionModel } from '@core-module/models/api/count-collection.model';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { ApiFilters } from '@core-module/services/service-structure/api-filter-builder';
import { DummyApiRequestModel } from '@core-module/services/testing/api-data-service/dummy-request-model';
import { DummyApiResponseModel } from '@core-module/services/testing/api-data-service/dummy-response-model';
import { DummyEditorDataService } from '@core-module/services/testing/api-editor-data-service/dummy-editor-data.service';
import { of, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

describe('ApiBaseService', () => {
  let service: DummyEditorDataService;
  const observer: jasmine.Spy = jasmine.createSpy('ApiBaseService Observer');
  const destroy: Subject<void> = new Subject();
  const expectedGet = new DummyApiRequestModel();
  const expectedGetArray = new Array<DummyApiRequestModel>();
  const expectedGetCount = new CountCollectionModel<DummyApiRequestModel>([], 0);
  const expectedSave = 1;
  const expectedDelete = true;
  const expectedPost = new DummyApiResponseModel();

  beforeEach(() => {
    const enhancedErpApiServiceMock = () => ({
      get: (apiRequest, responseType) => {
        return of(expectedGet);
      },
      getArray: (apiRequest, responseType) => {
        return of(expectedGetArray);
      },
      getArrayWithCount: (apiRequest, responseType) => {
        return of(expectedGetCount);
      },
      save: (apiRequest, entity, responseType, successMessage) => {
        return of(expectedSave);
      },
      delete: (apiRequest, successMessage) => {
        return of(expectedDelete);
      },
      postMessage: (apiRequest, payload, responseType, postResponseType) => {
        return of(expectedPost);
      },
    });
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        DummyEditorDataService,
        {
          provide: EnhancedErpApiService,
          useFactory: enhancedErpApiServiceMock,
        },
      ],
    });
    service = TestBed.inject(DummyEditorDataService);
  });

  afterEach(() => {
    destroy.next();
    observer.calls.reset();
  });

  it('get', () => {
    // Arrange
    service.getResult$.pipe(takeUntil(destroy)).subscribe(observer);
    const filters = new ApiFilters().equal('SomeProp', 'MyValue').build();

    // Act
    service.get(filters);

    // Assert
    expect(observer).toHaveBeenCalledOnceWith(expectedGet);
  });

  it('getAsync', fakeAsync(async () => {
    // Arrange
    service.getResult$.pipe(takeUntil(destroy)).subscribe(observer);
    const filters = new ApiFilters().equal('SomeProp', 'MyValue').build();

    // Act
    const response = await service.getAsync(filters);

    // Assert
    expect(observer).toHaveBeenCalledTimes(0);
    expect(response).toEqual(expectedGet);
  }));

  it('getArray', () => {
    // Arrange
    service.getArrayResult$.pipe(takeUntil(destroy)).subscribe(observer);
    const filters = new ApiFilters().equal('SomeProp1', 'MyValue1').build();

    // Act
    service.getArray(filters);

    // Assert
    expect(observer).toHaveBeenCalledOnceWith(expectedGetArray);
  });

  it('getArrayAsync', fakeAsync(async () => {
    // Arrange
    service.getArrayResult$.pipe(takeUntil(destroy)).subscribe(observer);
    const filters = new ApiFilters().equal('SomeProp1', 'MyValue1').build();

    // Act
    const response = await service.getArrayAsync(filters);

    // Assert
    expect(observer).toHaveBeenCalledTimes(0);
    expect(response).toEqual(expectedGetArray);
  }));

  it('getArrayWithCount', () => {
    // Arrange
    service.getArrayWithCount$.pipe(takeUntil(destroy)).subscribe(observer);
    const filters = new ApiFilters().equal('SomeProp2', 'MyValue2').build();

    // Act
    service.getArrayWithCount(filters);

    // Assert
    expect(observer).toHaveBeenCalledOnceWith(expectedGetCount);
  });

  it('getArrayWithCountAsync', fakeAsync(async () => {
    // Arrange
    service.getArrayWithCount$.pipe(takeUntil(destroy)).subscribe(observer);
    const filters = new ApiFilters().equal('SomeProp2', 'MyValue2').build();

    // Act
    const response = await service.getArrayWithCountAsync(filters);

    // Assert
    expect(observer).toHaveBeenCalledTimes(0);
    expect(response).toEqual(expectedGetCount);
  }));

  it('save', () => {
    // Arrange / Act
    service.save(new DummyApiRequestModel(), 'save successful').pipe(takeUntil(destroy)).subscribe(observer);

    // Assert
    expect(observer).toHaveBeenCalledOnceWith(expectedSave);
  });

  it('saveAsync', fakeAsync(async () => {
     // Arrange / Act
    const response = await service.saveAsync(new DummyApiRequestModel(), 'save successful');

    // Assert
    expect(response).toEqual(expectedSave);
  }));

  it('delete', () => {
    // Arrange / Act
    service.delete(1, 'delete successful').pipe(takeUntil(destroy)).subscribe(observer);

    // Assert
    expect(observer).toHaveBeenCalledOnceWith(expectedDelete);
  });

  it('deleteAsync', fakeAsync(async () => {
     // Arrange / Act
    const response = await service.deleteAsync(1, 'delete successful');

    // Assert
    expect(response).toEqual(expectedDelete);
  }));

  it('postMessage', () => {
    // Arrange
    service.postMessageResult$.pipe(takeUntil(destroy)).subscribe(observer);

    // Act
    service.postMessage(new DummyApiRequestModel());

    // Assert
    expect(observer).toHaveBeenCalledOnceWith(expectedPost);
  });

  it('postMessageAsync', fakeAsync(async () => {
     // Arrange / Act
    const response = await service.postMessageAsync(new DummyApiRequestModel());

    // Assert
    expect(response).toEqual(expectedPost);
  }));
});
