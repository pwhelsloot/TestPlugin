import { Injectable } from '@angular/core';
import { ApiRequest } from '@coremodels/api/api-request.model';
import { map } from 'rxjs/operators';
import { MomentTZDB } from '../../../models/timezone/moment-tzdb.model';
import { EnhancedErpApiService } from '../../enhanced-erp-api.service';

@Injectable({ providedIn: 'root' })
export class MomentTZDBServiceData {

    constructor(private erpApiService: EnhancedErpApiService) {
    }

    getMomentTimeZoneDatabase() {
        const request = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        request.urlResourcePath = ['momentTimeZoneDatabase'];

        return this.erpApiService.getRawResponse(request).pipe(map(response => {
            if (response) {
                return new MomentTZDB().parse(response, MomentTZDB);
            } else {
                throw new Error('Failed to initialise Moment timezone database');
            }
        }));
    }
}
