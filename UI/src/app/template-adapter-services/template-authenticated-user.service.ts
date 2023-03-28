import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { TemplateModuleAppRoutes } from '@app/template/template-module-app-routes.constants';
import { AuthorisationService } from '@auth-module/services/authorisation.service';
import { SysUserService } from '@auth-module/services/sysuser.service';
import { PlatformAuthenticatedUserAdapter } from '@core-module/services/config/platform-authenticated-user.adapter';
import { PlatformInitialisationAdapter } from '@core-module/services/config/platform-initialisation.adapter';
import { InstrumentationService } from '@core-module/services/logging/instrumentationService.service';
import { CoreUserPreferencesService } from '@core-module/services/preferences/core-user-preferences.service';
import { UserProfileService } from '@core-module/services/user-profile-service/user-profile.service';
import { combineLatest } from 'rxjs';
import { take, shareReplay, map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class TemplateAuthenticatedUserService extends PlatformAuthenticatedUserAdapter {
  constructor(
    private readonly router: Router,
    sysUserService: SysUserService,
    authorisationService: AuthorisationService,
    profileService: UserProfileService,
    instrumentationService: InstrumentationService,
    platformInitialisationAdapter: PlatformInitialisationAdapter,
    coreUserPreferencesService: CoreUserPreferencesService
  ) {
    super(
      sysUserService,
      authorisationService,
      profileService,
      instrumentationService,
      platformInitialisationAdapter,
      coreUserPreferencesService
    );
  }

  navigateToHomeAfterCoreAppLogin(): void {
    this.router.navigate([TemplateModuleAppRoutes.module, TemplateModuleAppRoutes.dashboard]);
  }

  // Example of how to override the initialisation/authentication adapters. You can choose which underlying services to call
  initialise(sysUserId: number, username: string) {
    this.initialised$ = combineLatest(this.getInitialisationStreams()).pipe(
      take(1),
      map(() => {
        return;
      }),
      shareReplay(1)
    );
    this.instrumentationService.setAuthenticatedUserContext(sysUserId.toString(), username, true);
    this.coreUserPreferencesService.userAuthenticated();
  }

  protected getInitialisationStreams() {
    return [this.platformInitialisationAdapter.initialised$];
  }
}
