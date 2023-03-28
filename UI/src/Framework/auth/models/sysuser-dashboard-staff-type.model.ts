import { alias } from '@coreconfig/api-dto-alias.function';

/**
 * @deprecated Move to PlatformUI + ScaleUI
 */
export class SysUserDashboardStaffType {

  @alias('SysUserId')
  sysUserId: number;

  @alias('PrimaryStaffTypeId')
  primaryStaffTypeId: number;

  @alias('AdditionalStaffTypeIds')
  additionalStaffTypeIds: number[];
}
