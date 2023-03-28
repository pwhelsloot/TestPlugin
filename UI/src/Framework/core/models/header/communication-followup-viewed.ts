import { amcsApiUrl, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
@amcsApiUrl('customer/CommunicationFollowupViewedSaves')
export class CommunicationFollowupViewedSave extends ApiBaseModel {

  @amcsJsonMember('CommunicationFollowupId')
  communicationFollowupId: number;

  @amcsJsonMember('IsViewed')
  isViewed: boolean;
}
