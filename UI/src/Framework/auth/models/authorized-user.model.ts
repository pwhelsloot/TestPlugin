export class AuthorizedUser {
  constructor(
    public username: string,
    public companyoutletid: number,
    public sysUserId: number,
    public isExternalLogoutRequired: boolean
  ) {}
}
