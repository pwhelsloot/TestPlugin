import { Injectable } from '@angular/core';
import { ApiResourceResponse } from '@core-module/config/api-resource-response.interface';
import { ClassBuilder } from '@core-module/helpers/dto/class-builder';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { ApiRequest } from '@core-module/models/api/api-request.model';
import { CountCollectionModel } from '@core-module/models/api/count-collection.model';
import { IFilter } from '@core-module/models/api/filters/iFilter';
import { DefaultAction } from '@core-module/models/default-action.model';
import { ErpApiService } from '../erp-api.service';

/**
 * @deprecated To be deleted
 */
@Injectable()
export class DefaultActionServiceData {

    constructor(private erpApiService: ErpApiService) {
    }

    getDefaultActions(page: number, filters: IFilter[]) {
        const apiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['customer/defaultActions'];
        apiRequest.max = 10;
        apiRequest.includeCount = true;
        apiRequest.page = page;
        if (isTruthy(filters)) {
            // single quote is added to the value on each filter and needs to be removed
            filters.forEach((filter) => {
                if (filter.value && typeof(filter.value) === 'string') {
                    filter.value = filter.value.replace(/[']/g, '');
                }
            });
            apiRequest.filters = filters;
        }
        return this.erpApiService.getRequest<ApiResourceResponse, DefaultAction>(apiRequest, this.getDefaultActionsMap, new CountCollectionModel<DefaultAction>([], 0));
    }

    private getDefaultActionsMap(response: ApiResourceResponse) {
        const results = new CountCollectionModel<DefaultAction>(
            ClassBuilder.buildFromApiResourceResults<DefaultAction>(response.resource, DefaultAction),
            isTruthy(response.extra.count) ? response.extra.count : 0
        );

        return results;
    }
}
