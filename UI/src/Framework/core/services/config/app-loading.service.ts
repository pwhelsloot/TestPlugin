import { Injectable } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { filter, take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class AppLoadingService {
  private readonly loaded = new ReplaySubject<boolean>(1);

  get(): Observable<boolean> {
    return this.loaded.asObservable();
  }

  loadingComplete(): Observable<boolean> {
    return this.loaded.pipe(
      filter((value) => value),
      take(1)
    );
  }

  set(loaded: boolean) {
    this.loaded.next(loaded);
  }
}
