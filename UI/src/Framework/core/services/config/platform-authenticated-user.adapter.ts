import { Injectable } from '@angular/core';
import { AuthorisationService } from '@auth-module/services/authorisation.service';
import { SysUserService } from '@auth-module/services/sysuser.service';
import { CoreUserPreferencesService } from '@core-module/services/preferences/core-user-preferences.service';
import { combineLatest, Observable } from 'rxjs';
import { map, shareReplay, take } from 'rxjs/operators';
import { BaseService } from '../base.service';
import { InstrumentationService } from '../logging/instrumentationService.service';
import { UserProfileService } from '../user-profile-service/user-profile.service';
import { IPlatformAuthenticatedserService } from './platform-authenticated-user.service.interface';
import { PlatformInitialisationAdapter } from './platform-initialisation.adapter';

@Injectable()
// Deals with initialsation needed after user is logged in
export abstract class PlatformAuthenticatedUserAdapter extends BaseService implements IPlatformAuthenticatedserService {
  initialised$: Observable<void>;

  constructor(
    protected readonly sysUserService: SysUserService,
    protected readonly authorisationService: AuthorisationService,
    protected readonly profileService: UserProfileService,
    protected readonly instrumentationService: InstrumentationService,
    protected readonly platformInitialisationAdapter: PlatformInitialisationAdapter,
    protected readonly coreUserPreferencesService: CoreUserPreferencesService
  ) {
    super();
  }

  abstract navigateToHomeAfterCoreAppLogin(): void;

  initialise(sysUserId: number, username: string) {
    this.initialised$ = combineLatest(this.getInitialisationStreams()).pipe(
      take(1),
      map(() => {
        return;
      }),
      shareReplay(1)
    );
    this.authorisationService.setAuthorisationClaims();
    this.instrumentationService.setAuthenticatedUserContext(sysUserId.toString(), username, true);
    this.profileService.requestProfileData(sysUserId);
    this.sysUserService.getSysUserStaffType(sysUserId);
    this.coreUserPreferencesService.userAuthenticated();
  }

  destroy() {
    this.instrumentationService.clearAuthenticatedUserContext();
  }

  protected getInitialisationStreams() {
    return [this.sysUserService.initalised$, this.platformInitialisationAdapter.initialised$];
  }
}
