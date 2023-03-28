import { Injectable } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { CoreUserPreferenceKeys } from '@core-module/models/preferences/core-user-preference-keys.model';
import { CoreUserPreference } from '@core-module/models/preferences/core-user-preference.model';
import { CoreAppMessagingAdapter } from '@core-module/services/config/core-app-messaging.service';
import { CoreUserPreferencesDataAccess } from '@core-module/services/preferences/data/core-user-preferences-data.access';
import { environment } from '@environments/environment';
import { merge, Observable, ReplaySubject } from 'rxjs';
import { map, switchMap, take } from 'rxjs/operators';
import { BaseService } from '../base.service';
import { ContainerAppMessagingHandlerService } from '../config/messaging/messaging-handler.service';

@Injectable({ providedIn: 'root' })
export class CoreUserPreferencesService extends BaseService {
  constructor(
    private dataAccess: CoreUserPreferencesDataAccess,
    private coreAppMessagingAdapter: CoreAppMessagingAdapter,
    private containerAppMessagingHandlerService: ContainerAppMessagingHandlerService
  ) {
    super();
  }

  private readonly userAuthenticatedSubject = new ReplaySubject<void>(1);

  userAuthenticated() {
    this.userAuthenticatedSubject.next();
  }

  get<T>(key: string, defaultValue?: T): Observable<T> {
    if (defaultValue === undefined) {
      defaultValue = null;
    }
    const formattedKey: string = this.buildKey(key);
    // Core preferences make the api call but also listens for core-ui preference changed messages (other apps changing core prefs)
    if (this.isCorePreference(key)) {
      return merge(this.apiGet<T>(key, defaultValue), this.containerAppMessagingHandlerService.coreUserPreferenceChanged<T>(key));
    } else {
      return this.apiGet<T>(formattedKey, defaultValue);
    }
  }

  save<T>(key: string, value: T): void {
    const formattedKey: string = this.buildKey(key);
    if (this.isCorePreference(key)) {
      this.coreAppMessagingAdapter.sendCorePreferenceChangeNotification(formattedKey, JSON.stringify(value));
    }
    this.userAuthenticatedSubject
      .pipe(
        switchMap(() => {
          return this.dataAccess.save(formattedKey, JSON.stringify(value));
        }),
        take(1)
      )
      .subscribe(() => {});
  }

  private apiGet<T>(key: string, defaultValue: T): Observable<T> {
    return this.userAuthenticatedSubject.pipe(
      switchMap(() => {
        return this.dataAccess.get(key);
      }),
      map((preference: CoreUserPreference) => {
        if (isTruthy(preference) && isTruthy(preference.value)) {
          return JSON.parse(preference.value);
        } else {
          return defaultValue;
        }
      })
    );
  }

  private buildKey(key: string): string {
    if (!this.isCorePreference(key)) {
      return `${environment.applicationURLPrefix}-${key}`;
    }
    return key;
  }

  private isCorePreference(key: string): boolean {
    return CoreUserPreferenceKeys.hasOwnProperty(key);
  }
}
