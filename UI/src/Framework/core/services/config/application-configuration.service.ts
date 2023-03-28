import { Injectable } from '@angular/core';
import { ApplicationConfiguration } from '@coremodels/application-configuration.model';
import { BaseService } from '@coreservices/base.service';
import { ApplicationConfigurationStore } from '@coreservices/config/application-configuration.store';
import { ApplicationConfigurationFormatter } from '@coreservices/config/data/application-configuration.formatter';
import { ApplicationConfigurationServiceData } from '@coreservices/config/data/application-configuration.service.data';
import { take, catchError, switchMap, takeUntil } from 'rxjs/operators';
import { interval, EMPTY, Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ApplicationConfigurationService extends BaseService {

    constructor(
        private store: ApplicationConfigurationStore,
        private dataService: ApplicationConfigurationServiceData) {
        super();
    }

    private requestSubject: Subject<number>;

    initialise() {
        // Do first call immediately
        this.dataService.getConfiguration().pipe(take(1)).subscribe((config: ApplicationConfiguration) => {
            this.store.changeStore(config);
            this.setUpInterval();
            this.requestSubject.next(config.pollingIntervalSeconds * 1000);
        }, () => { // on error default
            this.store.changeStore(ApplicationConfigurationFormatter.default(new ApplicationConfiguration()));
        });
    }

    private setUpInterval() {
        this.requestSubject = new Subject<number>();
        // Subject to handle subsequent calls using an interval determined by config
        this.requestSubject
            .pipe(
                takeUntil(this.unsubscribe),
                switchMap((intervalVal: number) => {
                    return interval(intervalVal).pipe(
                        takeUntil(this.unsubscribe),
                        switchMap(() => {
                            return this.dataService.getConfiguration();
                        }),
                        catchError(() => { // errors in subsequent calls can be ignored as we know we'll have a default.
                            this.store.configuration$.pipe(take(1)).subscribe((config: ApplicationConfiguration) => {
                                this.requestSubject.next(config.pollingIntervalSeconds * 1000);
                            });
                            return EMPTY;
                        })
                    );
                })
            ).subscribe((config: ApplicationConfiguration) => {
                this.store.changeStore(config);
                this.requestSubject.next(config.pollingIntervalSeconds * 1000);
            });
    }
}
