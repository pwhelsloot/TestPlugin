/* eslint-disable amcs-ts-plugin/api-request-ban */
/* eslint-disable max-classes-per-file */
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ApiBaseModelDecoratorKeys } from '@core-module/models/api/api-base-model-decorator-keys.model';
import { IApiRequestGetOptions } from '@core-module/models/api/api-request-get-options.interface';
import { CountCollectionModel } from '@core-module/models/api/count-collection.model';
import 'reflect-metadata';
import { of } from 'rxjs';
import { ApiRequest } from '../../models/api/api-request.model';
import { EnhancedErpApiService } from '../enhanced-erp-api.service';
import { DummyApiRequestModel } from '../testing/api-data-service/dummy-request-model';
import { DummyApiResponseModel } from '../testing/api-data-service/dummy-response-model';
import { ApiBaseDataService } from './api-base-data.service';
import { ApiFilters } from './api-filter-builder';

describe('ApiBaseDataService', () => {
  let service: ApiBaseDataService<DummyApiRequestModel, DummyApiResponseModel>;
  let enhancedErpApiService: EnhancedErpApiService;
  let expectedUrl = Reflect.getMetadata(ApiBaseModelDecoratorKeys.apiUrl, DummyApiRequestModel);
  beforeEach(() => {
    const enhancedErpApiServiceMock = () => ({
      get: (apiRequest, responseType) => ({}),
      getArray: (apiRequest, responseType) => ({}),
      getArrayWithCount: (apiRequest, responseType) => ({}),
      save: (apiRequest, entity, responseType, successMessage) => ({}),
      delete: (apiRequest, successMessage) => ({}),
      postMessage: (apiRequest, payload, responseType, postResponseType) => ({}),
    });
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        {
          provide: EnhancedErpApiService,
          useFactory: enhancedErpApiServiceMock,
        },
      ],
    });
    enhancedErpApiService = TestBed.inject(EnhancedErpApiService);
    service = new ApiBaseDataService<DummyApiRequestModel, DummyApiResponseModel>(
      enhancedErpApiService,
      DummyApiRequestModel,
      DummyApiResponseModel
    );
  });

  it('get', () => {
    const request = new ApiRequest();
    const expectedModel = new DummyApiRequestModel();
    request.urlResourcePath = [expectedUrl];
    spyOn(enhancedErpApiService, 'get').and.returnValue(of(expectedModel));
    const filters = new ApiFilters().equal('SomeProp', 'MyValue').build();
    request.filters = filters;
    service.get(filters).subscribe((response) => {
      expect(response).toEqual(expectedModel);
    });
    expect(enhancedErpApiService.get).toHaveBeenCalledOnceWith(request, DummyApiRequestModel);
  });

  it('getWithOptions', () => {
    const options: IApiRequestGetOptions = { searchTerms: ['test'], max: 10, includeCount: true, includeDeleted: true, page: 1 };
    const request = new ApiRequest(options);
    const expectedModel = new DummyApiRequestModel();
    request.urlResourcePath = [expectedUrl];
    spyOn(enhancedErpApiService, 'get').and.returnValue(of(expectedModel));
    const filters = new ApiFilters().equal('SomeProp', 'MyValue').build();
    request.filters = filters;
    service.get(filters, options).subscribe((response) => {
      expect(response).toEqual(expectedModel);
    });
    expect(enhancedErpApiService.get).toHaveBeenCalledOnceWith(request, DummyApiRequestModel);
  });

  it('getById', () => {
    const id = 2;
    const request = new ApiRequest();
    const expectedModel = new DummyApiRequestModel();
    request.urlResourcePath = [expectedUrl, id];
    spyOn(enhancedErpApiService, 'get').and.returnValue(of(expectedModel));
    service.getById(id).subscribe((response) => {
      expect(response).toEqual(expectedModel);
    });
    expect(enhancedErpApiService.get).toHaveBeenCalledOnceWith(request, DummyApiRequestModel);
  });

  it('getArray', () => {
    const request = new ApiRequest();
    const expectedModel = new Array<DummyApiRequestModel>();
    request.urlResourcePath = [expectedUrl];
    const filters = new ApiFilters().equal('SomeProp', 'MyValue').greaterThan('some', 'value').build();
    request.filters = filters;
    spyOn(enhancedErpApiService, 'getArray').and.returnValue(of(expectedModel));
    service.getArray(filters).subscribe((response) => {
      expect(response).toEqual(expectedModel);
    });
    expect(enhancedErpApiService.getArray).toHaveBeenCalledOnceWith(request, DummyApiRequestModel);
  });

  it('getArrayWithOptions', () => {
    const options: IApiRequestGetOptions = { searchTerms: ['test'], max: 10, page: 1 };
    const request = new ApiRequest(options);
    const expectedModel = new Array<DummyApiRequestModel>();
    request.urlResourcePath = [expectedUrl];
    const filters = new ApiFilters().equal('SomeProp', 'MyValue').greaterThan('some', 'value').build();
    request.filters = filters;
    spyOn(enhancedErpApiService, 'getArray').and.returnValue(of(expectedModel));
    service.getArray(filters, options).subscribe((response) => {
      expect(response).toEqual(expectedModel);
    });
    expect(enhancedErpApiService.getArray).toHaveBeenCalledOnceWith(request, DummyApiRequestModel);
  });

  it('getArrayWithCount', () => {
    const request = new ApiRequest();
    request.includeCount = true;
    const expectedValues = new Array<DummyApiRequestModel>();
    const expectedModel = new CountCollectionModel<DummyApiRequestModel>(expectedValues, 4);
    request.urlResourcePath = [expectedUrl];
    const filters = new ApiFilters().equal('SomeProp', 'MyValue').greaterThan('some', 'value').build();
    request.filters = filters;
    spyOn(enhancedErpApiService, 'getArrayWithCount').and.returnValue(of(expectedModel));
    service.getArrayWithCount(filters).subscribe((response) => {
      expect(response).toEqual(expectedModel);
    });
    expect(enhancedErpApiService.getArrayWithCount).toHaveBeenCalledOnceWith(request, DummyApiRequestModel);
  });

  it('getArrayWithCountAndOptions', () => {
    const options: IApiRequestGetOptions = { max: 10, includeCount: true, includeDeleted: true };
    const request = new ApiRequest(options);
    const expectedValues = new Array<DummyApiRequestModel>();
    const expectedModel = new CountCollectionModel<DummyApiRequestModel>(expectedValues, 4);
    request.urlResourcePath = [expectedUrl];
    const filters = new ApiFilters().equal('SomeProp', 'MyValue').greaterThan('some', 'value').build();
    request.filters = filters;
    spyOn(enhancedErpApiService, 'getArrayWithCount').and.returnValue(of(expectedModel));
    service.getArrayWithCount(filters, options).subscribe((response) => {
      expect(response).toEqual(expectedModel);
    });
    expect(enhancedErpApiService.getArrayWithCount).toHaveBeenCalledOnceWith(request, DummyApiRequestModel);
  });

  describe('getArrayWithCount includeCount default is true', () => {
    it('defaults to true when omitted', () => {
      const requestOptions: IApiRequestGetOptions = { max: 10, includeCount: undefined, includeDeleted: true };
      const expectedOptions: IApiRequestGetOptions = { max: 10, includeCount: true, includeDeleted: true };
      const request = new ApiRequest(expectedOptions);
      const expectedValues = new Array<DummyApiRequestModel>();
      const expectedModel = new CountCollectionModel<DummyApiRequestModel>(expectedValues, 4);
      request.urlResourcePath = [expectedUrl];
      const filters = new ApiFilters().equal('SomeProp', 'MyValue').greaterThan('some', 'value').build();
      request.filters = filters;
      spyOn(enhancedErpApiService, 'getArrayWithCount').and.returnValue(of(expectedModel));
      service.getArrayWithCount(filters, requestOptions).subscribe((response) => {
        expect(response).toEqual(expectedModel);
      });
      expect(enhancedErpApiService.getArrayWithCount).toHaveBeenCalledOnceWith(request, DummyApiRequestModel);
    });

    it('does not get overridden when set to false', () => {
      const requestOptions: IApiRequestGetOptions = { max: 10, includeCount: false, includeDeleted: true };
      const expectedOptions: IApiRequestGetOptions = { max: 10, includeCount: false, includeDeleted: true };
      const request = new ApiRequest(expectedOptions);
      const expectedValues = new Array<DummyApiRequestModel>();
      const expectedModel = new CountCollectionModel<DummyApiRequestModel>(expectedValues, 4);
      request.urlResourcePath = [expectedUrl];
      const filters = new ApiFilters().equal('SomeProp', 'MyValue').greaterThan('some', 'value').build();
      request.filters = filters;
      spyOn(enhancedErpApiService, 'getArrayWithCount').and.returnValue(of(expectedModel));
      service.getArrayWithCount(filters, requestOptions).subscribe((response) => {
        expect(response).toEqual(expectedModel);
      });
      expect(enhancedErpApiService.getArrayWithCount).toHaveBeenCalledOnceWith(request, DummyApiRequestModel);
    });
  });

  it('save', () => {
    const request = new ApiRequest();
    const expectedReturn = 4;
    const expectedModel = new DummyApiRequestModel();
    const expectedSuccessMessage = 'yepItWorked';
    request.urlResourcePath = [expectedUrl];

    spyOn(enhancedErpApiService, 'save').and.returnValue(of(expectedReturn));
    service.save(expectedModel, expectedSuccessMessage).subscribe((response) => {
      expect(response).toEqual(expectedReturn);
    });
    expect(enhancedErpApiService.save).toHaveBeenCalledOnceWith(request, expectedModel, DummyApiRequestModel, expectedSuccessMessage);
  });

  it('delete', () => {
    const request = new ApiRequest();
    const expectedReturn = true;
    const idToDelete = 990;
    const expectedSuccessMessage = 'yepItWorked';
    request.urlResourcePath = [expectedUrl, idToDelete];

    spyOn(enhancedErpApiService, 'delete').and.returnValue(of(expectedReturn));
    service.delete(idToDelete, expectedSuccessMessage).subscribe((response) => {
      expect(response).toEqual(expectedReturn);
    });
    expect(enhancedErpApiService.delete).toHaveBeenCalledOnceWith(request, expectedSuccessMessage);
  });

  it('postMessage', () => {
    const request = new ApiRequest();
    const expectedRequestModel = new DummyApiRequestModel();
    const expectedResponseModel = new DummyApiResponseModel();
    request.urlResourcePath = [expectedUrl];

    spyOn(enhancedErpApiService, 'postMessage').and.returnValue(of(expectedResponseModel));
    service.postMessage(expectedRequestModel).subscribe((response) => {
      expect(response).toEqual(expectedResponseModel);
    });
    expect(enhancedErpApiService.postMessage).toHaveBeenCalledOnceWith(
      request,
      expectedRequestModel,
      DummyApiRequestModel,
      DummyApiResponseModel
    );
  });
});
