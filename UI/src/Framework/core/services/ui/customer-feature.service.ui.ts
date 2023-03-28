import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

/**
 * @deprecated Move to PlatformUI
 */
@Injectable({ providedIn: 'root' })
export class CustomerFeatureServiceUI {

    siteSelectedId: BehaviorSubject<number> = new BehaviorSubject<number>(null);

    completeAndRestartSiteSelectedId() {
        this.siteSelectedId.complete();
        this.siteSelectedId = new BehaviorSubject<number>(null);
    }
}
