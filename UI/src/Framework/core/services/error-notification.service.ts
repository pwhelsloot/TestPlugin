import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ErrorNotificationService {
  errors$: Observable<string>;

  constructor() {
    this.errors = new Subject<string>();
    this.errors$ = this.errors.asObservable();
  }

  private errors: Subject<string>;

  notifyError(error: string) {
    this.errors.next(error);
  }
}
