import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

/**
 * @deprecated Move to PlatformUI + ScaleUI
 */
@Injectable({ providedIn: 'root' })
export class GlobalSearchServiceUI {

    focusRequested: Subject<boolean> = new Subject<boolean>();
    showGo: Subject<boolean> = new Subject<boolean>();

}
