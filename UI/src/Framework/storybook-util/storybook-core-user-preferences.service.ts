import { Injectable } from '@angular/core';
import { CoreUserPreferencesService } from '@core-module/services/preferences/core-user-preferences.service';
import { Observable, of } from 'rxjs';


@Injectable()
export class StorybookCoreUserPreferencesService extends CoreUserPreferencesService {
  get<T>(key: string, defaultValue?: T): Observable<T> {
    return of(defaultValue);
  }

  save<T>(key: string, value: T): void {
    return;
  }
}
