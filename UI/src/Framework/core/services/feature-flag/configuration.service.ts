import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiBaseService } from '../api-base.service';
import { EnhancedErpApiService } from '../enhanced-erp-api.service';
import { ApiConfigRequest } from './api-config.request-model';
import { ApiConfigResponse } from './api-config.response-model';
import { ConfigValue } from './config-value.model';

@Injectable()
export class ConfigurationService extends ApiBaseService<ApiConfigRequest, ApiConfigResponse> {
  configurations$: Observable<ConfigValue[]>;

  constructor(readonly enhancedErpApiService: EnhancedErpApiService) {
    super(enhancedErpApiService, ApiConfigRequest, ApiConfigResponse);
    this.setUpConfigurationStream();
  }

  private setUpConfigurationStream() {
    this.configurations$ = this.postMessageResult$.pipe(
      map((response: ApiConfigResponse) => response.configurationValues),
      map((configurationValues) => {
        return configurationValues.map((config) => {
          return { id: config.configId, value: JSON.parse(config.configValue) } as ConfigValue;
        });
      })
    );
  }
}
