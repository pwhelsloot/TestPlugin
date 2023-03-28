import { Injectable } from '@angular/core';
import { AuthorisationClaim } from '@auth-module/models/authorisation-claim.model';
import { ClassBuilder } from '@core-module/helpers/dto/class-builder';
import { ApiResourceResponse } from '@coreconfig/api-resource-response.interface';
import { ApiOptionsEnum } from '@coremodels/api/api-options.enum';
import { ApiRequest } from '@coremodels/api/api-request.model';
import { ErpApiService } from '@coreservices/erp-api.service';
import { BehaviorSubject, Observable, ReplaySubject } from 'rxjs';
import { filter, map, take } from 'rxjs/operators';
import { SysUserService } from './sysuser.service';

@Injectable({ providedIn: 'root' })
export class AuthorisationService {
  authorisationClaims: Observable<AuthorisationClaim[]>;
  staffTypeId: number;
  additionalStaffTypeIds: number[];
  initalised$ = new ReplaySubject(1);

  constructor(private erpApiService: ErpApiService, private sysUserService: SysUserService) {
    this.innerAuthorisationClaims = new BehaviorSubject<AuthorisationClaim[]>([]);
    this.authorisationClaims = this.innerAuthorisationClaims.asObservable();

    /**
      * @todo Move all this to SysUserService
      */
    this.sysUserService.sysUserStaffTypes$.pipe(take(1)).subscribe((data) => {
      this.staffTypeId = data.primaryStaffTypeId;
      this.additionalStaffTypeIds = data.additionalStaffTypeIds;
      this.initalised$.next();
    });
  }
  private innerAuthorisationClaims: BehaviorSubject<AuthorisationClaim[]>;

  hasAuthorisation(claimName: string): boolean {
    const claims: AuthorisationClaim[] = this.innerAuthorisationClaims.getValue();
    if (claims != null) {
      const foundClaim: AuthorisationClaim = claims.find((x) => x.description === claimName);
      return foundClaim != null && foundClaim.hasAccess;
    } else {
      return false;
    }
  }

  hasStaffType(staffType: number): boolean {
    if (this.additionalStaffTypeIds && this.additionalStaffTypeIds.length > 0) {
      return this.additionalStaffTypeIds.filter((x) => x === staffType).length > 0;
    } else {
      return false;
    }
  }

  hasAuthorisation$(claimName: string): Observable<boolean> {
    return this.innerAuthorisationClaims.pipe(
      filter((x) => x.length > 0),
      map((claims) => {
        const foundClaim: AuthorisationClaim = claims.find((x) => x.description === claimName);
        return foundClaim != null && foundClaim.hasAccess;
      })
    );
  }

  setAuthorisationClaims(): void {
    const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
    apiRequest.apiOptions = ApiOptionsEnum.core;
    apiRequest.urlResourcePath = ['authentication/authorisationClaims'];
    this.erpApiService
      .getRequestHandleError<ApiResourceResponse>(apiRequest, this.getAuthorisationMap, true)
      .subscribe((result: AuthorisationClaim[]) => {
        this.innerAuthorisationClaims.next(result);
      });
  }

  private getAuthorisationMap(response: ApiResourceResponse) {
    const data = response;
    const claims: AuthorisationClaim[] = [];
    for (const result of data['resource']) {
      const claim: AuthorisationClaim = ClassBuilder.buildFromApiResourceResponse<AuthorisationClaim>(result, AuthorisationClaim);
      claims.push(claim);
    }
    return claims;
  }
}
