import { Injectable } from '@angular/core';
import * as fromAuth from '@auth-module/store/auth.reducers';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import * as fromApp from '@core-module/store/app.reducers';
import { UserCommunicationCount } from '@coremodels/header/user-communication-count.model';
import { UserCommunication } from '@coremodels/header/user-communication.model';
import { BaseService } from '@coreservices/base.service';
import { Store } from '@ngrx/store';
import { Observable, of, ReplaySubject, Subject, timer } from 'rxjs';
import { switchMap, take, takeUntil, withLatestFrom } from 'rxjs/operators';
import { ApplicationConfigurationStore } from '../config/application-configuration.store';
import { UserCommunicationServiceData } from './data/user-communication.service.data';

/**
 * @deprecated Move to PlatformUI
 */
@Injectable()
export class UserCommunicationService extends BaseService {

    communicationsCount$: Observable<number>;
    communications$: Observable<UserCommunication[]>;

    constructor(
        private dataService: UserCommunicationServiceData,
        private store: Store<fromApp.AppState>,
        private applicationCofigStore: ApplicationConfigurationStore) {
        super();

        this.authState$ = this.store.select('auth');
        this.setupTimer();
        this.setupCommunicationCountStream();
        this.setupCommunicationsStream();
    }

    private authState$: Observable<fromAuth.State>;
    private communicationsCountSubject: ReplaySubject<number> = new ReplaySubject<number>(1);
    private communicationsSubject: ReplaySubject<UserCommunication[]> = new ReplaySubject<UserCommunication[]>(1);

    private communicationsRequest: Subject<any> = new Subject<any>();
    private communicationCountRequest: Subject<any> = new Subject<any>();

    requestCommunications() {
        this.communicationsRequest.next(null);
    }
    refreshCommunicationCount() {
        this.communicationCountRequest.next(null);
    }

    private setupCommunicationsStream() {
        this.communications$ = this.communicationsSubject.asObservable();

        this.communicationsRequest
            .pipe(takeUntil(this.unsubscribe), switchMap(() => this.dataService.getCommunications()))
            .subscribe((data: UserCommunication[]) => {
                this.communicationsSubject.next(data);
            });
    }

    private setupCommunicationCountStream() {
        this.communicationsCount$ = this.communicationsCountSubject.asObservable();
        this.communicationCountRequest
        .pipe(
            takeUntil(this.unsubscribe),
            withLatestFrom(this.authState$),
            switchMap(data => {
                const sysUserId = data[1].sysUserId;
                if (isTruthy(sysUserId) && sysUserId > 0) {
                    return this.dataService.getCommunicationCount(sysUserId);
                } else {
                    const userCommunicationCount = new UserCommunicationCount();
                    userCommunicationCount.communicationsCount = 0;
                    return of(userCommunicationCount);
                }
            })
        ).subscribe((data: UserCommunicationCount) => {
            this.communicationsCountSubject.next(data.communicationsCount);
        });
    }

    private setupTimer() {
        this.communicationsCount$ = this.communicationsCountSubject.asObservable();

        this.applicationCofigStore.configuration$.pipe(take(1)).subscribe(config => {
            return timer(0, config.communicationCountPollingIntervalSeconds * 1000)
                .pipe(
                    takeUntil(this.unsubscribe),
                    withLatestFrom(this.authState$),
                    switchMap(data => {
                        const sysUserId = data[1].sysUserId;
                        if (isTruthy(sysUserId) && sysUserId > 0) {
                            return this.dataService.getCommunicationCount(sysUserId);
                        } else {
                            const userCommunicationCount = new UserCommunicationCount();
                            userCommunicationCount.communicationsCount = 0;
                            return of(userCommunicationCount);
                        }
                    })
                ).subscribe((data: UserCommunicationCount) => {
                    this.communicationsCountSubject.next(data.communicationsCount);
                });
        });
    }
}
