import { Injectable } from '@angular/core';
import { ClassBuilder } from '@core-module/helpers/dto/class-builder';
import { ApiResourceResponse } from '@coreconfig/api-resource-response.interface';
import { ApiOptionsEnum } from '@coremodels/api/api-options.enum';
import { ApiRequest } from '@coremodels/api/api-request.model';
import { UserCommunicationCount } from '@coremodels/header/user-communication-count.model';
import { UserCommunication } from '@coremodels/header/user-communication.model';
import { ErpApiService } from '../../erp-api.service';
import { UserCommunicationFormatter } from '../user-communication-formatter.service';

/**
 * @deprecated Move to PlatformUI
 */
@Injectable()
export class UserCommunicationServiceData {
    constructor(private erpApiService: ErpApiService) {}

    getCommunications() {
        const apiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['settings', 'SysUserCommunicationSummaries'];
        apiRequest.apiOptions = ApiOptionsEnum.core;
        apiRequest.max = 3;

        return this.erpApiService.getRequest<ApiResourceResponse, UserCommunication>(
            apiRequest,
            this.communicationsMap,
            [] as UserCommunication[]
        );
    }

    getCommunicationCount(userId: number) {
        const apiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['settings', 'SysUserCommunicationCounts', userId];
        apiRequest.apiOptions = ApiOptionsEnum.core;

        return this.erpApiService.getRequest<ApiResourceResponse, UserCommunicationCount>(
            apiRequest,
            this.communicationCountMap,
            UserCommunicationCount
        );
    }

    private communicationsMap(response: ApiResourceResponse) {
        const results: UserCommunication[] = ClassBuilder.buildFromApiResourceResults<UserCommunication>(
            response.resource,
            UserCommunication
        );
        UserCommunicationFormatter.formatUserCommunication(results);
        return results;
    }

    private communicationCountMap(response: ApiResourceResponse) {
        const result: UserCommunicationCount = ClassBuilder.buildFromApiResourceResponse<UserCommunicationCount>(
            response.resource,
            UserCommunicationCount
        );
        return result;
    }
}
