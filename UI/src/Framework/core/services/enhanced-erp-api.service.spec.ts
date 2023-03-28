import {
    HttpClientTestingModule, HttpTestingController
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ApiRequest } from '@coremodels/api/api-request.model';
import { ErrorNotificationService } from '@coreservices/error-notification.service';
import { environment } from '@environments/environment';
import { FakeApiModel } from '@testing/fake-api-model';
import { ApiResourceResponse } from '../config/api-resource-response.interface';
import { ErrorInterceptor } from '../http/error.interceptor';
import { CountCollectionModel } from '../models/api/count-collection.model';
import { AmcsNotificationService } from './amcs-notification.service';
import { EnhancedErpApiService } from './enhanced-erp-api.service';
import { ErpSaveService } from './erp-save.service';

describe('Service: EnhancedErpApiService', () => {
  let service: EnhancedErpApiService;
  let httpMock: HttpTestingController;
  let errorNotificationService: ErrorNotificationService;
  let erpSaveService: ErpSaveService;
  let successNotificationService: AmcsNotificationService;

  beforeEach(() => {
    const errorNotificationServiceMock = () => ({
      notifyError: errors => ({})
    });
    const amcsNotificationServiceMock = () => ({
      showNotification: successMessage => ({})
    });
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        ErrorInterceptor,
        EnhancedErpApiService,
        {
          provide: ErrorNotificationService,
          useFactory: errorNotificationServiceMock
        },
        {
          provide: AmcsNotificationService,
          useFactory: amcsNotificationServiceMock
        },
        ErpSaveService
      ]
    });
    erpSaveService = TestBed.inject(ErpSaveService);
    service = TestBed.inject(EnhancedErpApiService);
    httpMock = TestBed.inject(HttpTestingController);
    successNotificationService = TestBed.inject(AmcsNotificationService);
    errorNotificationService = TestBed.inject(
      ErrorNotificationService
    );
    window.coreServiceRoot = 'root';
    environment.applicationApiPrefix = 'prefix';
  });

  afterEach(() => {
    //make sure there are no outstanding requests
    httpMock.verify();
  });

  describe('#getRawResponse', () => {
    it('throws on HttpError 401 and triggers errorNotificationService', () => {
      TestThrowsHttpError(errorNotificationService, service, httpMock, 401);
    });
    it('throws on HttpError 404 and triggers errorNotificationService', () => {
      TestThrowsHttpError(errorNotificationService, service, httpMock, 404);
    });
    it('throws on HttpError 500 and triggers errorNotificationService', () => {
      TestThrowsHttpError(errorNotificationService, service, httpMock, 500);
    });

    it('returns response on Get 200', () => {
      const expectedBaseUrl = 'root/prefix';
      const apiUrl = 'something';
      const payload = { data: 'payload' };
      const apiRequest = createApiRequest(apiUrl, payload);

      service.getRawResponse(apiRequest).subscribe((response) => {
        expect(response).toEqual(payload);
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'GET' });

      expect(req.request.method).toEqual('GET');

      req.flush(payload);
    });
  });

  describe('#postMessage', () => {
    it('returns null on error and triggers errorNotificationService', () => {
      const { apiRequest, expectedBaseUrl, apiUrl, errorMessageResponse } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();
      const request = new FakeApiModel();

      service.postMessage<FakeApiModel, FakeApiModel>(apiRequest, request, FakeApiModel, FakeApiModel).subscribe((response) => {
        expect(response).toEqual(null);
      });

      httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'POST' })
        .flush(errorMessageResponse, { status: 400, statusText: '' });

      expect(errorNotificationService.notifyError).toHaveBeenCalled();
      expect(errorNotificationService.notifyError).toHaveBeenCalledTimes(1);
    });

    it('returns null class instance on empty response', () => {
      const { apiRequest, expectedBaseUrl, apiUrl } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();
      const request = new FakeApiModel();

      service.postMessage<FakeApiModel, FakeApiModel>(apiRequest, request, FakeApiModel, FakeApiModel).subscribe((response) => {
        expect(errorNotificationService.notifyError).not.toHaveBeenCalled();
        expect(response).toEqual(null);
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'POST' });

      req.flush(null);
    });

    it('returns null class instance on errors', () => {
      const { apiRequest, expectedBaseUrl, apiUrl, errorMessageResponse } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();
      const request = new FakeApiModel();

      service.postMessage<FakeApiModel, FakeApiModel>(apiRequest, request, FakeApiModel, FakeApiModel).subscribe((response) => {
        expect(errorNotificationService.notifyError).toHaveBeenCalledOnceWith(errorMessageResponse.message);
        expect(response).toEqual(null);
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'POST' });

      req.flush({ resource: 'something', errors: errorMessageResponse.message } as ApiResourceResponse);
    });

    it('parse response with multiple resources on success', () => {
      const { apiRequest, expectedBaseUrl, apiUrl } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();
      const request = new FakeApiModel();

      service.postMessage<FakeApiModel, FakeApiModel>(apiRequest, request, FakeApiModel, FakeApiModel).subscribe((response) => {
        expect(response).toEqual(new FakeApiModel().parse(response, FakeApiModel));
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'POST' });

      req.flush({ resource: [{ id: 'someId', description: 'someDescription' }, { id: 'someId2', description: 'someDescription2' }] } as ApiResourceResponse);
    });
  });

  describe('#postUrl', () => {
    it('returns empty on error and triggers errorNotificationService', () => {
      const { apiRequest, expectedBaseUrl, apiUrl, errorMessageResponse } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.postUrl<FakeApiModel>(apiRequest, FakeApiModel).subscribe((response) => {
        expect(response).toEqual(new FakeApiModel());
      });

      httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'POST' })
        .flush(errorMessageResponse, { status: 400, statusText: '' });

      expect(errorNotificationService.notifyError).toHaveBeenCalled();
      expect(errorNotificationService.notifyError).toHaveBeenCalledTimes(1);
    });

    it('returns null class instance on empty response', () => {
      const { apiRequest, expectedBaseUrl, apiUrl } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.postUrl<FakeApiModel>(apiRequest, FakeApiModel).subscribe((response) => {
        expect(errorNotificationService.notifyError).not.toHaveBeenCalled();
        expect(response).toEqual(null);
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'POST' });

      req.flush(null);
    });

    it('returns null class instance on errors', () => {
      const { apiRequest, expectedBaseUrl, apiUrl, errorMessageResponse } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.postUrl<FakeApiModel>(apiRequest, FakeApiModel).subscribe((response) => {
        expect(errorNotificationService.notifyError).toHaveBeenCalledOnceWith(errorMessageResponse.message);
        expect(response).toEqual(null);
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'POST' });

      req.flush({ resource: 'something', errors: errorMessageResponse.message } as ApiResourceResponse);
    });

    it('parse response with multiple resources on success', () => {
      const { apiRequest, expectedBaseUrl, apiUrl } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.postUrl<FakeApiModel>(apiRequest, FakeApiModel).subscribe((response) => {
        expect(response).toEqual(new FakeApiModel().parse(response, FakeApiModel));
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'POST' });

      req.flush({ resource: [{ id: 'someId', description: 'someDescription' }, { id: 'someId2', description: 'someDescription2' }] } as ApiResourceResponse);
    });
  });

  describe('#delete', () => {
    it('returns null on error and triggers errorNotificationService', () => {
      const { apiRequest, expectedBaseUrl, apiUrl, errorMessageResponse } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.delete(apiRequest).subscribe((response) => {
        expect(response).toEqual(null);
      });

      httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'DELETE' })
        .flush(errorMessageResponse, { status: 400, statusText: '' });

      expect(errorNotificationService.notifyError).toHaveBeenCalled();
      expect(errorNotificationService.notifyError).toHaveBeenCalledTimes(1);
    });

    it('returns null class instance on empty response', () => {
      const { apiRequest, expectedBaseUrl, apiUrl } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.delete(apiRequest).subscribe((response) => {
        expect(errorNotificationService.notifyError).not.toHaveBeenCalled();
        expect(response).toEqual(null);
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'DELETE' });

      req.flush(null);
    });

    it('returns null class instance on errors', () => {
      const { apiRequest, expectedBaseUrl, apiUrl, errorMessageResponse } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.delete(apiRequest).subscribe((response) => {
        expect(errorNotificationService.notifyError).toHaveBeenCalledOnceWith(errorMessageResponse.message);
        expect(response).toBeFalse();
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'DELETE' });

      req.flush({ resource: 'something', errors: errorMessageResponse.message } as ApiResourceResponse);
    });

    it('parse response on success', () => {
      const { apiRequest, expectedBaseUrl, apiUrl } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.delete(apiRequest).subscribe((response) => {
        expect(response).toBeTrue();
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'DELETE' });

      req.flush({ resource: [{ id: 'someId', description: 'someDescription' }, { id: 'someId2', description: 'someDescription2' }] } as ApiResourceResponse);
    });
  });

  describe('#getArray', () => {
    it('returns empty on error and triggers errorNotificationService', () => {
      const { apiRequest, expectedBaseUrl, apiUrl, errorMessageResponse } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.getArray<FakeApiModel>(apiRequest, FakeApiModel).subscribe((response) => {
        expect(response).toEqual(new Array<FakeApiModel>());
      });
      throwHttpError('GET', httpMock, `${expectedBaseUrl}/${apiUrl}`, errorMessageResponse.message, 400);

      expect(errorNotificationService.notifyError).toHaveBeenCalled();
      expect(errorNotificationService.notifyError).toHaveBeenCalledTimes(1);
    });

    it('returns empty class instance on empty response', () => {
      const { apiRequest, expectedBaseUrl, apiUrl } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.getArray<FakeApiModel>(apiRequest, FakeApiModel).subscribe((response) => {
        expect(errorNotificationService.notifyError).not.toHaveBeenCalled();
        expect(response).toEqual(new Array<FakeApiModel>());
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'GET' });

      req.flush(null);
    });
    it('returns empty class instance on errors', () => {
      const { apiRequest, expectedBaseUrl, apiUrl, errorMessageResponse } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.getArray<FakeApiModel>(apiRequest, FakeApiModel).subscribe((response) => {
        expect(errorNotificationService.notifyError).toHaveBeenCalledOnceWith(errorMessageResponse.message);
        expect(response).toEqual(new Array<FakeApiModel>());
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'GET' });

      req.flush({ resource: 'something', errors: errorMessageResponse.message } as ApiResourceResponse);
    });
    it('parse response with multiple resources on success', () => {
      const { apiRequest, expectedBaseUrl, apiUrl } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.getArray<FakeApiModel>(apiRequest, FakeApiModel).subscribe((response) => {
        expect(response).toEqual(new FakeApiModel().parseArray(response, FakeApiModel));
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'GET' });

      req.flush({ resource: [{ id: 'someId', description: 'someDescription' }, { id: 'someId2', description: 'someDescription2' }] } as ApiResourceResponse);
    });
  });

  describe('#getArrayWithCount', () => {
    it('returns empty on error and triggers errorNotificationService', () => {
      const { apiRequest, expectedBaseUrl, apiUrl, errorMessageResponse } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.getArrayWithCount<FakeApiModel>(apiRequest, FakeApiModel).subscribe((response) => {
        expect(errorNotificationService.notifyError).toHaveBeenCalled();
        expect(response).toEqual(new CountCollectionModel<FakeApiModel>([], 0));
        expect(response.count).toEqual(0);
        expect(response.results).toEqual([]);
      });

      throwHttpError('GET', httpMock, `${expectedBaseUrl}/${apiUrl}`, errorMessageResponse.message, 400);

      expect(errorNotificationService.notifyError).toHaveBeenCalled();
      expect(errorNotificationService.notifyError).toHaveBeenCalledTimes(1);
    });

    it('returns empty class instance on empty response', () => {
      const { apiRequest, expectedBaseUrl, apiUrl } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.getArrayWithCount<FakeApiModel>(apiRequest, FakeApiModel).subscribe((response) => {
        expect(errorNotificationService.notifyError).not.toHaveBeenCalled();
        expect(response).toEqual(new CountCollectionModel<FakeApiModel>([], 0));
        expect(response.count).toEqual(0);
        expect(response.results).toEqual([]);
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'GET' });

      req.flush(null);
    });
    it('returns empty class instance on errors', () => {
      const { apiRequest, expectedBaseUrl, apiUrl, errorMessageResponse } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.getArrayWithCount<FakeApiModel>(apiRequest, FakeApiModel).subscribe((response) => {
        expect(errorNotificationService.notifyError).toHaveBeenCalledOnceWith(errorMessageResponse.message);
        expect(response).toEqual(new CountCollectionModel<FakeApiModel>([], 0));
        expect(response.count).toEqual(0);
        expect(response.results).toEqual([]);
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'GET' });

      req.flush({ resource: 'something', errors: errorMessageResponse.message } as ApiResourceResponse);
    });

    it('parse response with multiple resources on success', () => {
      const { apiRequest, expectedBaseUrl, apiUrl, errorMessageResponse } = setup('root/prefix', 'something', 'something went wrong');

      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.getArrayWithCount<FakeApiModel>(apiRequest, FakeApiModel).subscribe((response) => {
        expect(response).toEqual(new CountCollectionModel<FakeApiModel>(new FakeApiModel().parseArray(response, FakeApiModel), 0));
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'GET' });

      req.flush({ resource: [{ id: 'someId', description: 'someDescription' }, { id: 'someId2', description: 'someDescription2' }] } as ApiResourceResponse);
    });
  });

  describe('#save', () => {

    it('returns null on error and triggers errorNotificationService', () => {
      const { apiRequest, expectedBaseUrl, apiUrl, errorMessageResponse } = setup('root/prefix', 'something', 'something went wrong');
      const request = new FakeApiModel();
      spyOn(errorNotificationService, 'notifyError').and.callThrough();
      spyOn(erpSaveService, 'addRequest').and.callThrough();
      spyOn(erpSaveService, 'removeRequest').and.callThrough();

      const req = service.save<FakeApiModel>(apiRequest, request, FakeApiModel).subscribe((response) => {
        expect(response).toBeNull();
      });
      throwHttpError('POST', httpMock, `${expectedBaseUrl}/${apiUrl}`, errorMessageResponse.message, 400);

      expect(errorNotificationService.notifyError).toHaveBeenCalled();
      expect(errorNotificationService.notifyError).toHaveBeenCalledTimes(1);
      expect(erpSaveService.addRequest).toHaveBeenCalledOnceWith(apiRequest);
      expect(erpSaveService.removeRequest).toHaveBeenCalledOnceWith(apiRequest);
      expect(req.closed).toBeTrue();
    });

    it('returns null on empty response and triggers errorNotificationService', () => {
      const { apiRequest, expectedBaseUrl, apiUrl } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();
      spyOn(erpSaveService, 'addRequest').and.callThrough();
      spyOn(erpSaveService, 'removeRequest').and.callThrough();

      const request = service.save<FakeApiModel>(apiRequest, new FakeApiModel(), FakeApiModel).subscribe((response) => {
        expect(response).toBeNull();
      });
      const fakeRequest = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'POST' });
      fakeRequest.flush(null);

      expect(errorNotificationService.notifyError).not.toHaveBeenCalled();
      expect(erpSaveService.addRequest).toHaveBeenCalledOnceWith(apiRequest);
      expect(erpSaveService.removeRequest).toHaveBeenCalledOnceWith(apiRequest);
      expect(request.closed).toBeTrue();
    });

    it('returns null on errors', () => {
      const { apiRequest, expectedBaseUrl, apiUrl, errorMessageResponse } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();
      spyOn(erpSaveService, 'addRequest').and.callThrough();
      spyOn(erpSaveService, 'removeRequest').and.callThrough();

      const request = service.save<FakeApiModel>(apiRequest, new FakeApiModel(), FakeApiModel).subscribe((response) => {
        expect(response).toBeNull();
      });
      const fakeRequest = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'POST' });
      fakeRequest.flush({ resource: 'something', errors: errorMessageResponse.message } as ApiResourceResponse);

      expect(errorNotificationService.notifyError).toHaveBeenCalled();
      expect(errorNotificationService.notifyError).toHaveBeenCalledTimes(1);
      expect(erpSaveService.addRequest).toHaveBeenCalledOnceWith(apiRequest);
      expect(erpSaveService.removeRequest).toHaveBeenCalledOnceWith(apiRequest);
      expect(request.closed).toBeTrue();
    });

    it('parse response on success triggers success notification', () => {
      const { apiRequest, expectedBaseUrl, apiUrl, errorMessageResponse } = setup('root/prefix', 'something', 'something went wrong');
      const expectedResource = 10;
      const expectedSuccessMessage = 'Yep it worked';
      spyOn(errorNotificationService, 'notifyError').and.callThrough();
      spyOn(erpSaveService, 'addRequest').and.callThrough();
      spyOn(erpSaveService, 'removeRequest').and.callThrough();
      spyOn(successNotificationService, 'showNotification').and.callThrough();

      const request = service.save<FakeApiModel>(apiRequest, new FakeApiModel(), FakeApiModel, expectedSuccessMessage).subscribe((response) => {
        expect(response).toEqual(expectedResource);
      });
      const fakeRequest = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'POST' });
      fakeRequest.flush({ resource: expectedResource } as ApiResourceResponse);

      expect(successNotificationService.showNotification).toHaveBeenCalledOnceWith(expectedSuccessMessage);
      expect(erpSaveService.addRequest).toHaveBeenCalledOnceWith(apiRequest);
      expect(erpSaveService.removeRequest).toHaveBeenCalledOnceWith(apiRequest);
      expect(request.closed).toBeTrue();
    });
  });

  describe('#get', () => {
    it('returns null on error and triggers errorNotificationService', () => {
      const { apiRequest, expectedBaseUrl, apiUrl, errorMessageResponse } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.delete(apiRequest).subscribe((response) => {
        expect(response).toEqual(null);
      });

      httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'DELETE' })
        .flush(errorMessageResponse, { status: 400, statusText: '' });

      expect(errorNotificationService.notifyError).toHaveBeenCalled();
      expect(errorNotificationService.notifyError).toHaveBeenCalledTimes(1);
    });

    it('returns null class instance on empty response', () => {
      const { apiRequest, expectedBaseUrl, apiUrl } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.delete(apiRequest).subscribe((response) => {
        expect(errorNotificationService.notifyError).not.toHaveBeenCalled();
        expect(response).toEqual(null);
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'DELETE' });

      req.flush(null);
    });

    it('returns null class instance on errors', () => {
      const { apiRequest, expectedBaseUrl, apiUrl, errorMessageResponse } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.delete(apiRequest).subscribe((response) => {
        expect(errorNotificationService.notifyError).toHaveBeenCalledOnceWith(errorMessageResponse.message);
        expect(response).toBeFalse();
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'DELETE' });

      req.flush({ resource: 'something', errors: errorMessageResponse.message } as ApiResourceResponse);
    });

    it('parse response on success', () => {
      const { apiRequest, expectedBaseUrl, apiUrl } = setup('root/prefix', 'something', 'something went wrong');
      spyOn(errorNotificationService, 'notifyError').and.callThrough();

      service.delete(apiRequest).subscribe((response) => {
        expect(response).toBeTrue();
      });

      const req = httpMock
        .expectOne({ url: `${expectedBaseUrl}/${apiUrl}/`, method: 'DELETE' });

      req.flush({ resource: [{ id: 'someId', description: 'someDescription' }, { id: 'someId2', description: 'someDescription2' }] } as ApiResourceResponse);
    });
  });
  //saveArray
});

function setup(expectedBaseUrl: string, apiUrl: string, errorMessage: string) {
  const errorMessageResponse = {
    message: errorMessage
  };
  const apiRequest = createApiRequest(apiUrl);
  return { apiRequest, expectedBaseUrl, apiUrl, errorMessageResponse };
}

function TestThrowsHttpError(errorNotificationService: ErrorNotificationService, service: EnhancedErpApiService, httpMock: HttpTestingController, errorCode: number) {
  const { apiRequest, expectedBaseUrl, apiUrl, errorMessageResponse } = setup('root/prefix', 'something', 'something went wrong');
  spyOn(errorNotificationService, 'notifyError').and.callThrough();

  service.getRawResponse(apiRequest).subscribe((response) => {
    expect(response).toBeNull();
  });

  throwHttpError('GET', httpMock, `${expectedBaseUrl}/${apiUrl}`, errorMessageResponse.message, errorCode);

  expect(errorNotificationService.notifyError).toHaveBeenCalled();
  expect(errorNotificationService.notifyError).toHaveBeenCalledTimes(1);
}

function createApiRequest(apiUrl: string, payload?: any) {
  const apiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
  apiRequest.urlResourcePath = [apiUrl];
  apiRequest.postData = payload;
  return apiRequest;
}

function throwHttpError(method: string, httpMock: HttpTestingController, expectedUrl: string, errorMessage: string, errorCode: number) {
  httpMock
    .expectOne({ url: `${expectedUrl}/`, method })
    .flush({}, { status: errorCode, statusText: errorMessage });
}
