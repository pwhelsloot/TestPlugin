import { Injectable } from '@angular/core';
import { IFilter } from '@core-module/models/api/filters/iFilter';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { ApiBaseDataService } from '../../service-structure/api-base-data.service';
import { DummyApiRequestModel } from './dummy-request-model';
import { DummyApiResponseModel } from './dummy-response-model';

/**
 * Example implementation of the ApiBaseDataService
 *
 * @export
 * @class DummyApiDataService
 * @extends {ApiBaseDataService<DummyApiRequestModel, DummyApiResponseModel>}
 */
@Injectable()
export class DummyApiDataService extends ApiBaseDataService<DummyApiRequestModel, DummyApiResponseModel> {
  constructor(enhancedErpApiService: EnhancedErpApiService) {
    super(enhancedErpApiService, DummyApiRequestModel, DummyApiResponseModel);
  }

  get() {
    const filters = new Array<IFilter>();
    return super.get(filters);
  }

  myCustomGetArray() {
    const filters = new Array<IFilter>();
    return super.getArray(filters);
  }

  getArrayWithCount() {
    const filters = new Array<IFilter>();
    return super.getArrayWithCount(filters);
  }

  myCustomSave() {
    const model = new DummyApiRequestModel();
    return super.save(model, 'YEP saved');
  }

  delete() {
    return super.delete(100, 'YEP Deleted');
  }

  postMessage() {
    return super.postMessage(new DummyApiRequestModel());
  }
}
