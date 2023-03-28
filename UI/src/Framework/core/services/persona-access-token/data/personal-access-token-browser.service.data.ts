import { Injectable } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { UserLookup } from '@core-module/models/lookups/user-lookup.model';
import { PersonalAccessToken } from '@core-module/models/personal-access-token/personal-access-token.model';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { ApiRequest } from '@coremodels/api/api-request.model';
import { LocalizedDatePipe } from '@translate/localized-datePipe';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class PersonalAccessTokenBrowserServiceData {

    constructor(private enhancedErpApiService: EnhancedErpApiService, private datePipe: LocalizedDatePipe) {
    }

    getPersonalAccessTokens() {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['settings/SysUserPrivateKeys'];
        return this.enhancedErpApiService.getArray<PersonalAccessToken>(apiRequest, PersonalAccessToken).pipe(
            map((tokens: PersonalAccessToken[]) => {
                tokens.forEach(token => {
                    token.creationDate = isTruthy(token.creationDate) ? this.datePipe.transform(token.creationDate, 'shortDate') : token.creationDate;
                    token.expire = isTruthy(token.expire) ? this.datePipe.transform(token.expire, 'shortDate') : token.expire;

                });
                return tokens;
            }));
    }

    savePersonalAccessToken(successMessage: string, personalAccessToken: PersonalAccessToken): Observable<PersonalAccessToken> {
        const apiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['settings/sysUserPrivateKeysSave'];
        return this.enhancedErpApiService.postMessage<PersonalAccessToken, PersonalAccessToken>(apiRequest, personalAccessToken, PersonalAccessToken, PersonalAccessToken, successMessage);
    }

    deletePersonalAccessToken(successMessage: string, sysUserPrivateKeyId: number) {
        const apiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['settings/SysUserPrivateKeys', sysUserPrivateKeyId];
        return this.enhancedErpApiService.delete(apiRequest, successMessage);
    }

    getUsers() {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['settings/SysUsers'];
        return this.enhancedErpApiService.getArray<UserLookup>(apiRequest, UserLookup);
    }
}
