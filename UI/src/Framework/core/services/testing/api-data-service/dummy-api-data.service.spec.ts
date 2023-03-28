import { TestBed } from '@angular/core/testing';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { ApiBaseDataService } from './../../service-structure/api-base-data.service';
import { DummyApiDataService } from './dummy-api-data.service';

describe('DummyApiDataService', () => {
  let service: DummyApiDataService;

  beforeEach(() => {
    const enhancedErpApiServiceStub = () => ({});
    TestBed.configureTestingModule({
      providers: [
        DummyApiDataService,
        {
          provide: EnhancedErpApiService,
          useFactory: enhancedErpApiServiceStub,
        },
      ],
    });
    service = TestBed.inject(DummyApiDataService);
  });

  it('can load instance', () => {
    expect(service).toBeTruthy();
  });

  it('get', () => {
    spyOn(ApiBaseDataService.prototype, 'get');
    service.get();
    expect(ApiBaseDataService.prototype.get).toHaveBeenCalled();
  });

  it('myCustomGetArray', () => {
    spyOn(ApiBaseDataService.prototype, 'getArray');
    service.myCustomGetArray();
    expect(ApiBaseDataService.prototype.getArray).toHaveBeenCalled();
  });

  it('getArrayWithCount', () => {
    spyOn(ApiBaseDataService.prototype, 'getArrayWithCount');
    service.getArrayWithCount();
    expect(ApiBaseDataService.prototype.getArrayWithCount).toHaveBeenCalled();
  });

  it('myCustomSave', () => {
    spyOn(ApiBaseDataService.prototype, 'save');
    service.myCustomSave();
    expect(ApiBaseDataService.prototype.save).toHaveBeenCalled();
  });

  it('delete', () => {
    spyOn(ApiBaseDataService.prototype, 'delete');
    service.delete();
    expect(ApiBaseDataService.prototype.delete).toHaveBeenCalled();
  });

  it('postMessage', () => {
    spyOn(ApiBaseDataService.prototype, 'postMessage');
    service.postMessage();
    expect(ApiBaseDataService.prototype.postMessage).toHaveBeenCalled();
  });
});
