import { alias } from '@coreconfig/api-dto-alias.function';
import { amcsApiUrl, amcsJsonMember, amcsJsonObject, amcsJsonZonedDateMember, ApiBaseModel } from '@coremodels/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsApiUrl('settings/SysUserNotificationSummaries')
@amcsJsonObject()
export class UserNotification extends ApiBaseModel {
  @alias('ModifiedDate')
  @amcsJsonZonedDateMember('ModifiedDate')
  modifiedDate: Date;

  @alias('SysUserName')
  @amcsJsonMember('SysUserName')
  sysUserName: string;

  @alias('Field')
  @amcsJsonMember('Field')
  field: string;

  @alias('NewValue')
  @amcsJsonMember('NewValue')
  newValue: string;

  @alias('CommunicationNo')
  @amcsJsonMember('CommunicationNo')
  communicationNo: string;

  @alias('SysUserAssignedToFullName')
  @amcsJsonMember('SysUserAssignedToFullName')
  sysUserAssignedToFullName: string;

  @alias('FollowupStatusDescription')
  @amcsJsonMember('FollowupStatusDescription')
  followupStatusDescription: string;

  @alias('CommunicationId')
  @amcsJsonMember('CommunicationId')
  communicationId: number;

  @alias('CustomerId')
  @amcsJsonMember('CustomerId')
  customerId: number;

  @alias('CommunicationFollowupId')
  @amcsJsonMember('CommunicationFollowupId')
  communicationFollowupId: number;

  @alias('IsAssignedToGroup')
  @amcsJsonMember('IsAssignedToGroup')
  isAssignedToGroup: boolean;
}
