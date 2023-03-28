import { Injectable } from '@angular/core';
import { ApiOptionsEnum } from '@core-module/models/api/api-options.enum';
import { ApiRequest } from '@core-module/models/api/api-request.model';
import { PlatformConfiguration } from '@core-module/models/config/platform-configuration';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class PlatformConfigurationServiceData {

    constructor(private erpApiService: EnhancedErpApiService) {
    }

    getConfiguration<T extends PlatformConfiguration>(urlKey: string, type: (new () => T)): Observable<T> {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = [urlKey + 'ApplicationUISettings'];
        apiRequest.apiOptions = ApiOptionsEnum.core;
        return this.erpApiService.getRawResponse(apiRequest).pipe(map((response: any) => {
            return new type().parse(response, type);
        }));
    }
}
