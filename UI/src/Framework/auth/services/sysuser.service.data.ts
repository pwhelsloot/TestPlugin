import { Injectable } from '@angular/core';
import { ClassBuilder } from '@core-module/helpers/dto/class-builder';
import { ApiResourceResponse } from '@coreconfig/api-resource-response.interface';
import { ApiRequest } from '@coremodels/api/api-request.model';
import { FilterOperation } from '@coremodels/api/filters/filter-operation.enum';
import { ErpApiService } from '@coreservices/erp-api.service';
import { environment } from '@environments/environment';
import { SysUserDashboardStaffType } from '../models/sysuser-dashboard-staff-type.model';

/**
 * @deprecated Move to PlatformUI + ScaleUI (TDM / IMM 'fake' this so will break until they remove reference)
 */
@Injectable({ providedIn: 'root' })
export class SysUserServiceData {
  constructor(private erpApiService: ErpApiService) {}

  getSysUserDashboardStaffType(sysUserId: number) {
    const apiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban

    if (environment.applicationType === 'scale') {
      apiRequest.urlResourcePath = ['settings/dashboardStaffTypes'];
    } else {
      apiRequest.urlResourcePath = ['user/dashboardStaffTypes'];
    }

    apiRequest.filters = [
      {
        filterOperation: FilterOperation.Equal,
        name: 'SysUserId',
        value: sysUserId,
      },
    ];

    return this.erpApiService.getRequest<ApiResourceResponse, SysUserDashboardStaffType>(
      apiRequest,
      this.getDashboardStaffTypeMap.bind(this),
      SysUserDashboardStaffType
    );
  }

  private getDashboardStaffTypeMap(response: ApiResourceResponse): SysUserDashboardStaffType {
    return ClassBuilder.buildFromApiResourceResponse(response.resource[0], SysUserDashboardStaffType);
  }
}
