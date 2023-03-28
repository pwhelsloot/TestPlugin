import { Injectable } from '@angular/core';
import * as fromAuth from '@auth-module/store/auth.reducers';
import { CommunicationFollowupViewedSave } from '@core-module/models/header/communication-followup-viewed';
import * as fromApp from '@core-module/store/app.reducers';
import { isTruthy } from '@corehelpers/is-truthy.function';
import { UserNotificationCount } from '@coremodels/header/user-notification-count.model';
import { UserNotification } from '@coremodels/header/user-notification.model';
import { BaseService } from '@coreservices/base.service';
import { ApplicationConfigurationStore } from '@coreservices/config/application-configuration.store';
import { ApiBusinessService } from '@coreservices/service-structure/api-business.service';
import { ApiFilters } from '@coreservices/service-structure/api-filter-builder';
import { Store } from '@ngrx/store';
import { Observable, of, ReplaySubject, Subject, timer } from 'rxjs';
import { switchMap, take, takeUntil, withLatestFrom } from 'rxjs/operators';


@Injectable()
export class UserNotificationService extends BaseService {

  notificationsCount$: Observable<number>;
  notifications$: Observable<UserNotification[]>;

  constructor(
    private store: Store<fromApp.AppState>,
    private applicationConfigStore: ApplicationConfigurationStore,
    private businessServiceManager: ApiBusinessService
  ) {
    super();
    this.authState$ = this.store.select('auth');
    this.setupTimer();
    this.setupNotificationCountStream();
    this.setupNotificationsStream();
  }

  private authState$: Observable<fromAuth.State>;
  private notificationsCountSubject: ReplaySubject<number> = new ReplaySubject<number>(1);
  private notificationsSubject: ReplaySubject<UserNotification[]> = new ReplaySubject<UserNotification[]>(1);

  private notificationsCountRequest: Subject<any> = new Subject<any>();
  private notificationsRequest: Subject<any> = new Subject<any>();

  requestNotifications() {
    this.notificationsRequest.next(null);
  }

  requestNotificationsCount() {
    this.notificationsCountRequest.next(null);
  }

  saveCommunicationViewed(followupId: number, isViewed: boolean) {
      const model = new CommunicationFollowupViewedSave();
      model.communicationFollowupId = followupId;
      model.isViewed = isViewed;
      return this.businessServiceManager.save(model, null, CommunicationFollowupViewedSave);
  }

  private setupNotificationsStream() {
    this.notifications$ = this.notificationsSubject.asObservable();

    this.notificationsRequest
      .pipe(
        takeUntil(this.unsubscribe),
        switchMap(() => {
           return this.businessServiceManager.getArray(new ApiFilters().build(), UserNotification);
          }
        ))
      .subscribe((data: UserNotification[]) => {
        this.notificationsSubject.next(data);
      });
  }

  private setupNotificationCountStream() {
    this.notificationsCount$ = this.notificationsCountSubject.asObservable();
    this.notificationsCountRequest
      .pipe(
        takeUntil(this.unsubscribe),
        withLatestFrom(this.authState$),
        switchMap(data => {
          const sysUserId = data[1].sysUserId;
          if (isTruthy(sysUserId) && sysUserId > 0) {
            return this.businessServiceManager.get(new ApiFilters().build(), UserNotificationCount);
          } else {
            const userNotificationCount = new UserNotificationCount();
            userNotificationCount.notificationsCount = 0;
            return of(userNotificationCount);
          }
        })
      ).subscribe((data: UserNotificationCount) => {
      this.notificationsCountSubject.next(data.notificationsCount);
    });
  }

  private setupTimer() {
    this.notificationsCount$ = this.notificationsCountSubject.asObservable();
    this.applicationConfigStore.configuration$.pipe(take(1)).subscribe(config => {
      return timer(0, config.communicationCountPollingIntervalSeconds * 1000)
        .pipe(
          takeUntil(this.unsubscribe),
          withLatestFrom(this.authState$),
          switchMap(data => {
            const sysUserId = data[1].sysUserId;
            if (isTruthy(sysUserId) && sysUserId > 0) {
              return this.businessServiceManager.get(new ApiFilters().build(), UserNotificationCount);
            } else {
              const userCommunicationCount = new UserNotificationCount();
              userCommunicationCount.notificationsCount = 0;
              return of(userCommunicationCount);
            }
          })
        ).subscribe((data) => {
          this.notificationsCountSubject.next(data.notificationsCount);
        });
    });
  }
}
