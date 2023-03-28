
import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig, MatSnackBarRef } from '@angular/material/snack-bar';
import { ApplicationConfiguration } from '@coremodels/application-configuration.model';
import { BaseService } from '@coreservices/base.service';
import { ApplicationConfigurationStore } from '@coreservices/config/application-configuration.store';
import { takeUntil } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AmcsNotificationService extends BaseService {
  snackBarRef: MatSnackBarRef<any>;
  longDuration: number;
  normalDuration: number;
  shortDuration: number;

  constructor(
    private appConfig: ApplicationConfigurationStore,
    private snackBar: MatSnackBar) {
    super();

    this.appConfig.configuration$
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((config: ApplicationConfiguration) => {
        this.longDuration = config.notificationDurationLong;
        this.normalDuration = config.notificationDurationNormal;
        this.shortDuration = config.notificationDurationShort;
      });
  }

  showLongNotification(message: string, action?: string) {
    this.showNotification(message, action, this.longDuration);
  }

  showRaisedNotification(message: string, action?: string, duration?: number) {
    this.showNotification(message, action, duration, true);
  }

  showNotification(message: string, action?: string, duration?: number, displayRaised?: boolean): void {
    let messageDuration: number = duration;
    if (!messageDuration) {
      messageDuration = this.normalDuration;
    }

    const config = new MatSnackBarConfig();
    config.duration = messageDuration;
    if (action) {
      config.panelClass = ['notification-with-action'];
    } else {
      config.panelClass = ['notification-no-action'];
    }

    if (displayRaised) {
      config.panelClass.push('notification-raised');
    }

    this.snackBarRef = this.snackBar.open(message, action, config);
  }
}
