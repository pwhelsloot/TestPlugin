import { alias } from '@core-module/config/api-dto-alias.function';

export class AuthenticationToken {
  @alias('authResult')
  authResult: string;

  @alias('username')
  username: string;

  @alias('companyoutletid')
  companyoutletid: number;

  @alias('sysUserId')
  sysUserId: number;

  @alias('isExternalLogoutRequired')
  isExternalLogoutRequired: boolean;

  @alias('errorMessage')
  errorMessage: string;
}
