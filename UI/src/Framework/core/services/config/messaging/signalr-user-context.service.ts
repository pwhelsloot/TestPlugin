import {
    AppUserContext1Response
} from '@amcs/platform-communication';
import { Injectable } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class SignalRUserContextService {
  userContext$: Observable<AppUserContext1Response>;

  constructor() {
    this.userContext$ = this.userContextSubject.asObservable();
  }

  private userContextSubject = new ReplaySubject<AppUserContext1Response>(1);

  setUserContext(context: AppUserContext1Response) {
    this.userContextSubject.next(context);
  }
}
