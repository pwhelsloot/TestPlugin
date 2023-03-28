import { Injectable } from '@angular/core';
import { AuthenticationToken } from '@auth-module/models/authentication-token.model';
import { AuthorizedUser } from '@auth-module/models/authorized-user.model';
import * as AuthActions from '@auth-module/store/auth.actions';
import { ApiResourceResponse } from '@core-module/config/api-resource-response.interface';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { PlatformAuthenticatedUserAdapter } from '@core-module/services/config/platform-authenticated-user.adapter';
import { PlatformInitialisationAdapter } from '@core-module/services/config/platform-initialisation.adapter';
import { ApiRequest } from '@coremodels/api/api-request.model';
import { ErpApiService } from '@coreservices/erp-api.service';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { map, mergeMap, switchMap, tap } from 'rxjs/operators';

@Injectable()
export class AuthEffects {
  cacheUserId: number;

  @Effect()
  authInitialise = this.actions$.pipe(
    ofType(AuthActions.TRY_INITIALISE),
    switchMap(() => {
      const apiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
      apiRequest.urlResourcePath = ['authStatus'];
      return this.erpApiService.getRequest<
        ApiResourceResponse,
        AuthenticationToken
      >(apiRequest, this.getAuthResultData, AuthenticationToken);
    }),
    mergeMap((response: AuthenticationToken) => {
      const actions: any[] = [{ type: AuthActions.INITIALISE }];
      this.platformInitialisationAdapter.authStatusFinished.next();
      if (response.authResult === 'ok') {
        actions.unshift({
          type: AuthActions.SIGNIN,
          payload: new AuthorizedUser(
            response.username,
            response.companyoutletid,
            response.sysUserId,
            response.isExternalLogoutRequired
          ),
        });
      } else if (response.authResult === 'Unauthorized') {
        actions.unshift({
          type: AuthActions.SIGNOUT,
        });
      }
      return actions;
    })
  );

  @Effect({ dispatch: false })
  authSignin = this.actions$.pipe(
    ofType(AuthActions.SIGNIN),
    tap((action: AuthActions.Signin) => {
      this.platformAuthenticatedUserAdapter.initialise(
        action.payload.sysUserId,
        action.payload.username
      );
    })
  );

  @Effect()
  authSignout = this.actions$.pipe(
    ofType(AuthActions.TRY_SIGNOUT),
    map((action: AuthActions.TrySignout) => {
      const returnURL = isTruthy(action.returnURL) ? `?returnUrl=${action.returnURL}` : '';
      window.top.location.href = window.coreServiceRoot + '/logout.html' + returnURL;
      return { type: AuthActions.SIGNOUT };
    })
  );

  constructor(
    private actions$: Actions,
    private erpApiService: ErpApiService,
    private platformAuthenticatedUserAdapter: PlatformAuthenticatedUserAdapter,
    private platformInitialisationAdapter: PlatformInitialisationAdapter
  ) {}

  private getAuthResultData(response: ApiResourceResponse) {
    const data = response;
    return data;
  }
}
