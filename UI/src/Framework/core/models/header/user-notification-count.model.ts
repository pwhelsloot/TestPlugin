import { alias } from '@coreconfig/api-dto-alias.function';
import { amcsApiUrl, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@coremodels/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsApiUrl('settings/SysUserNotificationCounts')
@amcsJsonObject()
export class UserNotificationCount extends ApiBaseModel {

  @alias('NotificationsCount')
  @amcsJsonMember('NotificationsCount')
  notificationsCount: number;
}
