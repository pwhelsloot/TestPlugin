import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

/**
 * @deprecated Move to PlatformUI
 */
export enum TransportJobsViewModeEnum {
    TwoColumn = 1,
    Inline = 2,
    Mobile = 3
}

/**
 * @deprecated Move to PlatformUI
 */
@Injectable({ providedIn: 'root' })
export class CustomerOperationsServiceUI {
    transportJobsViewMode$: Observable<TransportJobsViewModeEnum>;

    constructor() {
        this.transportJobsViewMode = new BehaviorSubject<TransportJobsViewModeEnum>(TransportJobsViewModeEnum.Inline);
        this.transportJobsViewMode$ = this.transportJobsViewMode.asObservable();
    }

    private previousViewMode: TransportJobsViewModeEnum;
    private transportJobsViewMode: BehaviorSubject<TransportJobsViewModeEnum>;

    changeTransportJobsViewModeFromPreference(viewMode: TransportJobsViewModeEnum) {
        // Preferences might have an invalid option (we might be on mobile device with inline set or vice versa)
        if (this.transportJobsViewMode.getValue() === TransportJobsViewModeEnum.Mobile && viewMode !== TransportJobsViewModeEnum.Mobile) {
            this.previousViewMode = viewMode;
        } else if (this.transportJobsViewMode.getValue() !== TransportJobsViewModeEnum.Mobile && viewMode === TransportJobsViewModeEnum.Mobile) {
            this.previousViewMode = viewMode;
        } else {
            this.previousViewMode = this.transportJobsViewMode.getValue();
            this.transportJobsViewMode.next(viewMode);
        }
    }

    changeTransportJobsViewMode(viewMode: TransportJobsViewModeEnum) {
        this.previousViewMode = this.transportJobsViewMode.getValue();
        this.transportJobsViewMode.next(viewMode);
    }

    returnToPreviousViewMode(isMobile: boolean) {
        if (isMobile) {
            this.changeTransportJobsViewMode(TransportJobsViewModeEnum.Mobile);
        } else {
            const currentViewMode = this.transportJobsViewMode.getValue();
            // We don't want to go to mobile view if the screen isn't in mobile mode!
            if (this.previousViewMode !== TransportJobsViewModeEnum.Mobile) {
                this.transportJobsViewMode.next(this.previousViewMode);
                this.previousViewMode = currentViewMode;
            }
        }
    }
}
