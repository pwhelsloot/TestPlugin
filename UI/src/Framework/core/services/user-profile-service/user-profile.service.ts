import { Injectable } from '@angular/core';
import * as fromAuth from '@auth-module/store/auth.reducers';
import { MyProfileData } from '@core-module/models/external-dependencies/profile/my-profile-data.model';
import * as fromApp from '@core-module/store/app.reducers';
import { ApiResourceResponse } from '@coreconfig/api-resource-response.interface';
import { AmcsNotificationService } from '@coreservices/amcs-notification.service';
import { BaseService } from '@coreservices/base.service';
import { ApplicationConfigurationStore } from '@coreservices/config/application-configuration.store';
import { ErrorNotificationService } from '@coreservices/error-notification.service';
import { UserProfileServiceData } from '@coreservices/user-profile.service.data';
import { Store } from '@ngrx/store';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { switchMap, take, takeUntil, withLatestFrom } from 'rxjs/operators';

/**
 * @deprecated Move to ALL UI apps (which actually use MyProfile)
 */
@Injectable({ providedIn: 'root' })
export class UserProfileService extends BaseService {

  myProfile$: Observable<MyProfileData>;
  authState$: Observable<fromAuth.State>;

  constructor(private dataService: UserProfileServiceData, private store: Store<fromApp.AppState>,
    private errorNotificationService: ErrorNotificationService,
    private notificationService: AmcsNotificationService,
    private translationService: SharedTranslationsService,
    private applicationCofigStore: ApplicationConfigurationStore) {
    super();
    this.authState$ = this.store.select('auth');
    this.setUpMyProfileSearchStream();
  }
  private myProfileResults: ReplaySubject<MyProfileData> = new ReplaySubject<MyProfileData>(1);
  private myProfileRequest: Subject<number> = new Subject<number>();

  requestProfileData(id?: number) {
    this.myProfileRequest.next(id);
  }

  save(data: MyProfileData) {
    this.dataService.save(data)
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((response: ApiResourceResponse) => {
        if (response.errors != null) {
          this.errorNotificationService.notifyError(response.errors);

        } else {
          this.translationService.translations.pipe(take(1)).subscribe((translations: string[]) => {
            this.notificationService.showNotification(translations['myProfile.myprofileSavedNotification']);
          });
          this.requestProfileData();
        }
      });
  }

  private setUpMyProfileSearchStream() {
    this.myProfile$ = this.myProfileResults.asObservable();
    this.myProfileRequest
      .pipe(
        withLatestFrom(this.authState$),
        takeUntil(this.unsubscribe),
        switchMap(data => {
          const sysUserId: number = data[0];
          const authState: fromAuth.State = data[1];
          if (authState.initialised && authState.sysUserId != null) {
            return this.dataService.doMyProfileSearch(authState.sysUserId);
          } else if (sysUserId != null) {
            return this.dataService.doMyProfileSearch(sysUserId);
          } else {
            throw new Error('Unhandled user state');
          }
        })
      )
      .subscribe(
        (result: MyProfileData) => {
          this.myProfileResults.next(result);
          this.applicationCofigStore.changeProfileThumbnail(result.thumbnailBase64, result.userInitials);
        }
      );
  }
}
