import { Injectable } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { ApiRequest } from '@core-module/models/api/api-request.model';
import { IFilter } from '@core-module/models/api/filters/iFilter';
import { CostTemplateSelector } from '@core-module/models/cost-template-selector.model';
import { EnhancedErpApiService } from '../enhanced-erp-api.service';

/**
 * @deprecated To be deleted
 */
@Injectable()
export class CostTemplateSelectorServiceData {

  constructor(private erpApiService: EnhancedErpApiService) {
  }

  getCostTemplatess(page: number, filters: IFilter[]) {
    const apiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
    apiRequest.urlResourcePath = ['materialManagement/SupplierCostTemplateSelectorLookups'];
    apiRequest.max = 10;
    apiRequest.includeCount = true;
    apiRequest.page = page;
    if (isTruthy(filters)) {
      apiRequest.filters = filters;
    }

    return this.erpApiService.getArrayWithCount<CostTemplateSelector>(apiRequest, CostTemplateSelector);
  }
}
