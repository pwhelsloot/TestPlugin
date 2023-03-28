import { Injectable } from '@angular/core';
import { AuthorisationService } from '@auth-module/services/authorisation.service';
import { SysUserService } from '@auth-module/services/sysuser.service';
import { PlatformAuthenticatedUserAdapter } from '@core-module/services/config/platform-authenticated-user.adapter';
import { PlatformInitialisationAdapter } from '@core-module/services/config/platform-initialisation.adapter';
import { InstrumentationService } from '@core-module/services/logging/instrumentationService.service';
import { CoreUserPreferencesService } from '@core-module/services/preferences/core-user-preferences.service';
import { UserProfileService } from '@core-module/services/user-profile-service/user-profile.service';

@Injectable()
export class StorybookAuthenticatedUserService extends PlatformAuthenticatedUserAdapter {

  constructor(
    protected readonly sysUserService: SysUserService,
    protected readonly authorisationService: AuthorisationService,
    protected readonly profileService: UserProfileService,
    protected readonly instrumentationService: InstrumentationService,
    protected readonly platformInitAdapter: PlatformInitialisationAdapter,
    protected readonly coreUserPreferencesService: CoreUserPreferencesService,
  ) {
    super(sysUserService, authorisationService, profileService, instrumentationService, platformInitAdapter, coreUserPreferencesService);
    this.initialise();
  }

  navigateToHomeAfterCoreAppLogin(): void {
    throw new Error('Method not supported.');
  }

  initialise() {
    this.coreUserPreferencesService.userAuthenticated();
  }
}
