/* eslint-disable amcs-ts-plugin/api-request-ban */
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { fakeAsync, TestBed } from '@angular/core/testing';
import { ApiBaseModelDecoratorKeys } from '@core-module/models/api/api-base-model-decorator-keys.model';
import { IApiRequestGetOptions } from '@core-module/models/api/api-request-get-options.interface';
import 'reflect-metadata';
import { of } from 'rxjs';
import { ApiRequest } from '../../models/api/api-request.model';
import { DummyApiResponseModel } from '../testing/api-data-service/dummy-response-model';
import { EnhancedErpApiService } from './../enhanced-erp-api.service';
import { DummyApiRequestModel } from './../testing/api-data-service/dummy-request-model';
import { ApiBaseDataService } from './api-base-data.service';
import { ApiBusinessService } from './api-business.service';

describe('ApiBusinessService', () => {
  let businessService: ApiBusinessService;
  let dataService: ApiBaseDataService<DummyApiRequestModel>;
  let enhancedErpApiService: EnhancedErpApiService;
  let httpMock: HttpTestingController;
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
        ApiBusinessService,
        {
          provide: EnhancedErpApiService,
          useFactory: enhancedErpApiServiceMock,
        },
      ],
    });
    businessService = TestBed.inject(ApiBusinessService);
    enhancedErpApiService = TestBed.inject(EnhancedErpApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    //make sure there are no outstanding requests
    httpMock.verify();
  });

  describe('', () => {
    beforeEach(() => {
      dataService = new ApiBaseDataService<DummyApiRequestModel>(enhancedErpApiService, DummyApiRequestModel);
      spyOn(businessService, 'createDataService').and.returnValue(dataService);
    });

    it('get', () => {
      const request = new ApiRequest();
      const expectedModel = new DummyApiRequestModel();
      request.urlResourcePath = [expectedUrl];
      spyOn(enhancedErpApiService, 'get').and.returnValue(of(expectedModel));
      spyOn(dataService, 'get').and.callThrough();

      businessService.get([], DummyApiRequestModel).subscribe((response) => {
        expect(response).toEqual(expectedModel);
      });
      expect(dataService.get).toHaveBeenCalledOnceWith([], undefined);
    });

    it('getWithOptions', () => {
      const options: IApiRequestGetOptions = { searchTerms: ['test'], includeCount: true, includeDeleted: true, page: 1 };
      const request = new ApiRequest(options);
      const expectedModel = new DummyApiRequestModel();
      request.urlResourcePath = [expectedUrl];
      spyOn(enhancedErpApiService, 'get').and.returnValue(of(expectedModel));
      spyOn(dataService, 'get').and.callThrough();

      businessService.get([], DummyApiRequestModel, options).subscribe((response) => {
        expect(response).toEqual(expectedModel);
      });
      expect(dataService.get).toHaveBeenCalledOnceWith([], options);
    });

    it('getAsync', fakeAsync(async () => {
      const request = new ApiRequest();
      const expectedModel = new DummyApiRequestModel();
      request.urlResourcePath = [expectedUrl];
      spyOn(enhancedErpApiService, 'get').and.returnValue(of(expectedModel));
      spyOn(dataService, 'getAsync').and.callThrough();

      const response = await businessService.getAsync([], DummyApiRequestModel);

      expect(response).toEqual(expectedModel);

      expect(dataService.getAsync).toHaveBeenCalledOnceWith([], undefined);
    }));

    it('getAsyncWithOptions', fakeAsync(async () => {
      const options: IApiRequestGetOptions = {
        searchTerms: ['test'],
        max: 10,
        includeCount: true,
        includeDeleted: true,
        page: 1,
      };
      const request = new ApiRequest(options);
      const expectedModel = new DummyApiRequestModel();
      request.urlResourcePath = [expectedUrl];
      spyOn(enhancedErpApiService, 'get').and.returnValue(of(expectedModel));
      spyOn(dataService, 'getAsync').and.callThrough();

      const response = await businessService.getAsync([], DummyApiRequestModel, options);

      expect(response).toEqual(expectedModel);

      expect(dataService.getAsync).toHaveBeenCalledOnceWith([], options);
    }));

    it('getById', () => {
      const id = 2;
      const request = new ApiRequest();
      const expectedModel = new DummyApiRequestModel();
      request.urlResourcePath = [expectedUrl, id];
      spyOn(enhancedErpApiService, 'get').and.returnValue(of(expectedModel));
      spyOn(dataService, 'getById').and.callThrough();

      businessService.getById(id, DummyApiRequestModel).subscribe((response) => {
        expect(response).toEqual(expectedModel);
      });
      expect(dataService.getById).toHaveBeenCalledOnceWith(id);
    });

    it('getByIdAsync', fakeAsync(async () => {
      const id = 2;
      const request = new ApiRequest();
      const expectedModel = new DummyApiRequestModel();
      request.urlResourcePath = [expectedUrl];
      spyOn(enhancedErpApiService, 'get').and.returnValue(of(expectedModel));
      spyOn(dataService, 'getByIdAsync').and.callThrough();

      const response = await businessService.getByIdAsync(id, DummyApiRequestModel);

      expect(response).toEqual(expectedModel);

      expect(dataService.getByIdAsync).toHaveBeenCalledOnceWith(id);
    }));

    it('getArray', () => {
      const request = new ApiRequest();
      const expectedModel = new Array<DummyApiRequestModel>();
      request.urlResourcePath = [expectedUrl];
      spyOn(enhancedErpApiService, 'getArray').and.returnValue(of(expectedModel));
      spyOn(dataService, 'getArray').and.callThrough();

      businessService.getArray([], DummyApiRequestModel).subscribe((response) => {
        expect(response).toEqual(expectedModel);
      });

      expect(dataService.getArray).toHaveBeenCalledOnceWith([], undefined);
    });

    it('getArrayWithOptions', () => {
      const options: IApiRequestGetOptions = { includeDeleted: true };
      const request = new ApiRequest(options);
      const expectedModel = new Array<DummyApiRequestModel>();
      request.urlResourcePath = [expectedUrl];
      spyOn(enhancedErpApiService, 'getArray').and.returnValue(of(expectedModel));
      spyOn(dataService, 'getArray').and.callThrough();

      businessService.getArray([], DummyApiRequestModel, options).subscribe((response) => {
        expect(response).toEqual(expectedModel);
      });

      expect(dataService.getArray).toHaveBeenCalledOnceWith([], options);
    });

    it('getArrayAsync', fakeAsync(async () => {
      const request = new ApiRequest();
      const expectedModel = new Array<DummyApiRequestModel>();
      request.urlResourcePath = [expectedUrl];
      spyOn(enhancedErpApiService, 'getArray').and.returnValue(of(expectedModel));
      spyOn(dataService, 'getArrayAsync').and.callThrough();

      const response = await businessService.getArrayAsync([], DummyApiRequestModel);

      expect(response).toEqual(expectedModel);
      expect(dataService.getArrayAsync).toHaveBeenCalledOnceWith([], undefined);
    }));

    it('getArrayAsyncWithOptions', fakeAsync(async () => {
      const options: IApiRequestGetOptions = {
        searchTerms: ['test'],
        max: 10,
        includeCount: true,
        includeDeleted: true,
        page: 1,
      };
      const request = new ApiRequest(options);
      const expectedModel = new Array<DummyApiRequestModel>();
      request.urlResourcePath = [expectedUrl];
      spyOn(enhancedErpApiService, 'getArray').and.returnValue(of(expectedModel));
      spyOn(dataService, 'getArrayAsync').and.callThrough();

      const response = await businessService.getArrayAsync([], DummyApiRequestModel, options);

      expect(response).toEqual(expectedModel);
      expect(dataService.getArrayAsync).toHaveBeenCalledOnceWith([], options);
    }));

    it('save', () => {
      const request = new ApiRequest();
      const expectedReturn = 4;
      const expectedModel = new DummyApiRequestModel();
      const expectedSuccessMessage = 'yepItWorked';
      request.urlResourcePath = [expectedUrl];
      spyOn(enhancedErpApiService, 'save').and.returnValue(of(expectedReturn));
      spyOn(dataService, 'save').and.callThrough();

      businessService.save(expectedModel, expectedSuccessMessage, DummyApiRequestModel).subscribe((response) => {
        expect(response).toEqual(expectedReturn);
      });

      expect(dataService.save).toHaveBeenCalledOnceWith(expectedModel, expectedSuccessMessage);
    });

    it('saveAsync', fakeAsync(async () => {
      const request = new ApiRequest();
      const expectedReturn = 4;
      const expectedModel = new DummyApiRequestModel();
      const expectedSuccessMessage = 'yepItWorked';
      request.urlResourcePath = [expectedUrl];
      spyOn(enhancedErpApiService, 'save').and.returnValue(of(expectedReturn));
      spyOn(dataService, 'saveAsync').and.callThrough();

      const response = await businessService.saveAsync(expectedModel, expectedSuccessMessage, DummyApiRequestModel);

      expect(response).toEqual(expectedReturn);
      expect(dataService.saveAsync).toHaveBeenCalledOnceWith(expectedModel, expectedSuccessMessage);
    }));

    it('delete', () => {
      const request = new ApiRequest();
      const expectedReturn = true;
      const idToDelete = 990;
      const expectedSuccessMessage = 'yepItWorked';
      request.urlResourcePath = [expectedUrl];
      spyOn(enhancedErpApiService, 'delete').and.returnValue(of(expectedReturn));
      spyOn(dataService, 'delete').and.callThrough();

      businessService.delete(idToDelete, expectedSuccessMessage, DummyApiRequestModel).subscribe((response) => {
        expect(response).toEqual(expectedReturn);
      });

      expect(dataService.delete).toHaveBeenCalledOnceWith(idToDelete, expectedSuccessMessage);
    });

    it('deleteAsync', fakeAsync(async () => {
      const request = new ApiRequest();
      const expectedReturn = true;
      const idToDelete = 990;
      const expectedSuccessMessage = 'yepItWorked';
      request.urlResourcePath = [expectedUrl];
      spyOn(enhancedErpApiService, 'delete').and.returnValue(of(expectedReturn));
      spyOn(dataService, 'delete').and.callThrough();

      const response = await businessService.deleteAsync(idToDelete, expectedSuccessMessage, DummyApiRequestModel);

      expect(response).toEqual(expectedReturn);
      expect(dataService.delete).toHaveBeenCalledOnceWith(idToDelete, expectedSuccessMessage);
    }));
  });
  describe('', () => {
    beforeEach(() => {
      dataService = new ApiBaseDataService<DummyApiRequestModel, DummyApiResponseModel>(
        enhancedErpApiService,
        DummyApiRequestModel,
        DummyApiResponseModel
      );
      spyOn(businessService, 'createDataService').and.returnValue(dataService);
    });
    it('postMessage', () => {
      const request = new ApiRequest();
      const expectedRequestModel = new DummyApiRequestModel();
      const expectedResponseModel = new DummyApiResponseModel();
      request.urlResourcePath = [expectedUrl];
      spyOn(enhancedErpApiService, 'postMessage').and.returnValue(of(expectedResponseModel));
      spyOn(dataService, 'postMessage').and.callThrough();

      businessService.postMessage(expectedRequestModel, DummyApiRequestModel, DummyApiResponseModel).subscribe((response) => {
        expect(response).toEqual(expectedResponseModel);
      });

      expect(dataService.postMessage).toHaveBeenCalledOnceWith(expectedRequestModel);
    });

    it('postMessageAsync', fakeAsync(async () => {
      const request = new ApiRequest();
      const expectedRequestModel = new DummyApiRequestModel();
      const expectedResponseModel = new DummyApiResponseModel();
      request.urlResourcePath = [expectedUrl];
      spyOn(enhancedErpApiService, 'postMessage').and.returnValue(of(expectedResponseModel));
      spyOn(dataService, 'postMessageAsync').and.callThrough();

      const response = await businessService.postMessageAsync(expectedRequestModel, DummyApiRequestModel, DummyApiResponseModel);

      expect(response).toEqual(expectedResponseModel);
      expect(dataService.postMessageAsync).toHaveBeenCalledOnceWith(expectedRequestModel);
    }));
  });
});
