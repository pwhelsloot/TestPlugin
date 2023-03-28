import { Injectable } from '@angular/core';
import { ApiBaseService } from '../../api-base.service';
import { DummyApiRequestModel } from '../api-data-service/dummy-request-model';
import { DummyApiResponseModel } from '../api-data-service/dummy-response-model';
import { EnhancedErpApiService } from './../../enhanced-erp-api.service';

@Injectable()
export class DummyEditorDataWithOptionsService extends ApiBaseService<DummyApiRequestModel, DummyApiResponseModel> {
  constructor(enhancedErpApiService: EnhancedErpApiService) {
    super(enhancedErpApiService, DummyApiRequestModel, DummyApiResponseModel, { debounceInterval: 1000 });
  }
}
