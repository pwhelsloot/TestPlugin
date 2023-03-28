import { EventEmitter, Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';

/**
 * @deprecated Move to PlatformUI
 */
@Injectable({ providedIn: 'root' })
export class HeaderService {

    headerMenuButtonClicked: Subject<boolean> = new Subject<boolean>();

    isSearchedClicked: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

    logOutSelected = new EventEmitter<boolean>();
}
