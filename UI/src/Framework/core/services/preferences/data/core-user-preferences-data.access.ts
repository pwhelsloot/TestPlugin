import { Injectable } from '@angular/core';
import { CoreApiRequest } from '@core-module/models/api/core-api-request';
import { CoreUserPreference } from '@core-module/models/preferences/core-user-preference.model';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class CoreUserPreferencesDataAccess {
  constructor(private readonly erpApiService: EnhancedErpApiService) {}

  get(key: string): Observable<CoreUserPreference> {
    const apiRequest = new CoreApiRequest();
    apiRequest.urlResourcePath = ['userPreference'];
    apiRequest.key = key;
    return this.erpApiService.get(apiRequest, CoreUserPreference);
  }

  save(key: string, value: string): Observable<number> {
    const apiRequest = new CoreApiRequest();
    apiRequest.urlResourcePath = ['userPreference'];
    const preference = new CoreUserPreference();
    preference.key = key;
    preference.value = value;
    return this.erpApiService.save(apiRequest, preference, CoreUserPreference);
  }
}
