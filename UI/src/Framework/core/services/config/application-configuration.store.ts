import { ApplicationConfiguration } from '@coremodels/application-configuration.model';
import { DeveloperButtonsConfiguration } from '@coremodels/developer-buttons-configuration.model';
import { Observable, ReplaySubject } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { InstrumentationService } from '@core-module/services/logging/instrumentationService.service';

@Injectable({ providedIn: 'root' })
export class ApplicationConfigurationStore {

    configuration$: Observable<ApplicationConfiguration>;
    initialsed$: Observable<boolean>;
    profileThumbnail$: Observable<[string, string]>;

    constructor(private applicationInsightsService: InstrumentationService) {
        this.initialsed = new ReplaySubject<boolean>(1);
        this.initialsed$ = this.initialsed.asObservable();

        this.configuration = new ReplaySubject<ApplicationConfiguration>(1);
        this.configuration$ = this.configuration.asObservable();
        this.applicationInsightsService.setUpInsights(
            this.configuration$.pipe(
                map((config: ApplicationConfiguration) => {
                    return [config.appInsights, config.getAppInsightProperties()];
                })
            )
        );
        this.profileThumbnail = new ReplaySubject<[string, string]>(1);
        this.profileThumbnail$ = this.profileThumbnail.asObservable();
    }

    private configuration: ReplaySubject<ApplicationConfiguration>;
    private initialsed: ReplaySubject<boolean>;
    private profileThumbnail: ReplaySubject<[string, string]>;

    changeStore(config: ApplicationConfiguration) {
        this.configuration.next(config);
        this.initialsed.next(true);
    }

    changeDeveloperButtons(buttonConfig: DeveloperButtonsConfiguration) {
        this.configuration$.pipe(take(1)).subscribe((appConfig: ApplicationConfiguration) => {
            appConfig.buttonConfiguration = buttonConfig;
            this.configuration.next(appConfig);
        });
    }

    changeProfileThumbnail(image: string, initials: string) {
        this.profileThumbnail.next([image, initials]);
    }
}
