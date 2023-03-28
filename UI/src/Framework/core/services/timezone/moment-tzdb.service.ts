import { Injectable } from '@angular/core';
import { MomentTZDBZone } from '@core-module/models/timezone/moment-tzdb-zone.model';
import { BaseService } from '@coreservices/base.service';
import * as moment from 'moment-timezone/moment-timezone-utils';
import { EMPTY, interval, Observable, ReplaySubject, Subject } from 'rxjs';
import { catchError, switchMap, take, takeUntil } from 'rxjs/operators';
import { ApplicationConfiguration } from '../../models/application-configuration.model';
import { MomentTZDB } from '../../models/timezone/moment-tzdb.model';
import { ApplicationConfigurationStore } from '../config/application-configuration.store';
import { ErrorNotificationService } from '../error-notification.service';
import { MomentTZDBServiceData } from './data/moment-tzdb.service.data';

@Injectable({ providedIn: 'root' })
export class MomentTZDBService extends BaseService {
    initialised$: Observable<boolean>;

    constructor(
        private store: ApplicationConfigurationStore,
        private dataService: MomentTZDBServiceData,
        private errorNotificationService: ErrorNotificationService
    ) {
        super();
        this.initialised$ = this.initialisedSubject.asObservable();
    }

    private requestSubject: Subject<number>;
    private initialisedSubject = new ReplaySubject<boolean>(1);
    private currentTZDBVersion: string;
    private canonicalIdMap: string[] = [];

    initialise() {
        // Do first call immediately
        this.dataService
            .getMomentTimeZoneDatabase()
            .pipe(take(1))
            .subscribe(
                (momentTZDB: MomentTZDB) => {
                    // the load function expects a packed TZDB.
                    // we do not pack the TZDB on the server because the pack function depends on specific version of moment.js.
                    // therefore we pack the TZDB here before loading.

                    const packedTZDB = this.pack(momentTZDB);
                    moment.tz.load(packedTZDB);
                    this.loadCanonicalIdMap(momentTZDB);
                    this.currentTZDBVersion = packedTZDB.version;
                    this.initialisedSubject.next(true);

                    this.setUpInterval();

                    this.store.configuration$.pipe(take(1)).subscribe((config: ApplicationConfiguration) => {
                        this.requestSubject.next(config.pollingIntervalSeconds * 1000);
                    });
                },
                () => {
                    // we want the initial call to succeed - otherwise moment is left with its default TZDB.
                    this.errorNotificationService.notifyError('Failed to initialise Moment timezone database');
                }
            );
    }

    getBrowserTimeZoneId() {
        const momentGuess = moment.tz.guess();
        return this.canonicalIdMap[momentGuess];
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
                            return this.dataService.getMomentTimeZoneDatabase();
                        }),
                        catchError(() => {
                            // ignore errors in subsequent calls in the same way as we do with application configuration.
                            // just queue up another attempt.
                            this.store.configuration$.pipe(take(1)).subscribe((config: ApplicationConfiguration) => {
                                this.requestSubject.next(config.pollingIntervalSeconds * 1000);
                            });
                            return EMPTY;
                        })
                    );
                })
            )
            .subscribe((momentTZDB: MomentTZDB) => {
                if (momentTZDB.version !== this.currentTZDBVersion) {
                    // the load function expects a packed TZDB.
                    // we do not pack the TZDB on the server because the pack function depends on specific version of moment.js.
                    // therefore we pack the TZDB here before loading.

                    const packedTZDB = this.pack(momentTZDB);
                    moment.tz.load(packedTZDB);
                    this.currentTZDBVersion = packedTZDB.version;

                    this.store.configuration$.pipe(take(1)).subscribe((config: ApplicationConfiguration) => {
                        this.requestSubject.next(config.pollingIntervalSeconds * 1000);
                    });
                }
            });
    }

    private pack(unpackedTZDB: MomentTZDB): { version: string; links: string[]; zones: string[] } {
        const packedTZDB = {
            version: unpackedTZDB.version,
            links: unpackedTZDB.links,
            zones: []
        };

        unpackedTZDB.zones.forEach((zone) => {
            const unpackedZone: moment.UnpackedZone = zone as moment.UnpackedZone;
            const packedZone = moment.tz.pack(unpackedZone);
            packedTZDB.zones.push(packedZone);
        });

        return packedTZDB;
    }

    private loadCanonicalIdMap(tzdb: MomentTZDB) {
        tzdb.zones.forEach((zone: MomentTZDBZone) => {
            this.canonicalIdMap[zone.name] = zone.canonicalId;
        });
    }
}
