import { Injectable } from '@angular/core';
import { ClassBuilder } from '@core-module/helpers/dto/class-builder';
import { MyProfileData } from '@core-module/models/external-dependencies/profile/my-profile-data.model';
import { ApiResourceResponse } from '@coreconfig/api-resource-response.interface';
import { ApiRequest } from '@coremodels/api/api-request.model';
import { ErpApiService } from '@coreservices/erp-api.service';

/**
 * @deprecated Move to ALL UI apps (which actually use MyProfile)
 */
@Injectable({ providedIn: 'root' })
export class UserProfileServiceData {

  constructor(
    private erpApiService: ErpApiService) { }

  doMyProfileSearch(sysUserId: number) {
    const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
    apiRequest.urlResourcePath = ['Settings', 'Sysuserprofiles', sysUserId];

    return this.erpApiService.getRequest<ApiResourceResponse, MyProfileData>(
      apiRequest, this.getMyProfileSearchMap.bind(this), MyProfileData);
  }

  save(data: MyProfileData) {
    const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
    apiRequest.urlResourcePath = ['Settings', 'Sysuserprofiles'];
    return this.erpApiService.postSimpleRequest(apiRequest, data);
  }

  private getMyProfileSearchMap(response: ApiResourceResponse) {
    const responsePayload = response.resource;
    const result: MyProfileData = ClassBuilder.buildFromApiResourceResponse<MyProfileData>(
      responsePayload,
      MyProfileData
    );

    return result;
  }
}
