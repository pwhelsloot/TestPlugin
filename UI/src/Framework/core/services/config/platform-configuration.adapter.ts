import { Injectable, NgZone, OnDestroy } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import * as AuthActions from '@auth-module/store/auth.actions';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { PlatformConfiguration } from '@core-module/models/config/platform-configuration';
import * as fromApp from '@core-module/store/app.reducers';
import { Store } from '@ngrx/store';
import { AmcsModalService } from '@shared-module/components/amcs-modal/amcs-modal.service';
import { ErrorModalComponent } from '@shared-module/components/layouts/error-modal/error-modal.component';
import { combineLatest, Observable, ReplaySubject } from 'rxjs';
import { filter, map, take, takeUntil } from 'rxjs/operators';
import { BaseService } from '../base.service';
import { ErrorNotificationService } from '../error-notification.service';
import { ReportingBiHelperService } from '../reportingBi/reportingBi-helper';
import { PlatformConfigurationServiceData } from './data/platform-configuration.service.data';
import { IPlatformConfigurationService } from './platform-configuration.service.interface';
import { PlatformTitleService } from './platform-title.service';

@Injectable()
// Deals with base configuration
export abstract class PlatformConfigurationAdapter extends BaseService implements OnDestroy, IPlatformConfigurationService {
  abstract configuration$: Observable<PlatformConfiguration>;

  initialised$ = new ReplaySubject<void>(1);

  abstract urlKey: string;

  fakeConfigCall = true;

  constructor(
    private errorNotificationService: ErrorNotificationService,
    private modalService: AmcsModalService,
    private store: Store<fromApp.AppState>,
    private dataService: PlatformConfigurationServiceData,
    private ngZone: NgZone,
    private reportingBiHelper: ReportingBiHelperService,
    private titleService: PlatformTitleService,
    private router: Router,
    private activatedRoute: ActivatedRoute
    ) {
      super();
    }

    private configuration = new ReplaySubject<PlatformConfiguration>(1);
    private type: new () => any;
    private pageTitleTranslations$: Observable<string[]>;

  setUp<T extends PlatformConfiguration>(type: new () => T, titleTranslations: Observable<string[]>) {
    this.type = type;
    this.pageTitleTranslations$ = titleTranslations;
  }

  initialise() {
    this.getConfiguration();
    if (window.Cypress) {
      this.doCypressLogin();
    }
    this.setUpExago();
    this.setUpErrorModals();
    this.setUpPageTitles();
  }

  ngOnDestroy() {
    super.ngOnDestroy();
    window.platform.namespace.openFromExago = null;
  }

  protected getConfiguration() {
    this.configuration$ = this.configuration.asObservable();
    if (this.fakeConfigCall) {
      this.configuration.next(new this.type());
      this.initialised$.next();
    } else {
      this.dataService
        .getConfiguration(this.urlKey, this.type)
        .pipe(take(1))
        .subscribe((config: PlatformConfiguration) => {
          this.configuration.next(config);
          this.initialised$.next();
        });
    }
  }

  private setUpExago() {
    window.platform = window.platform || {};
    window.platform.namespace = window.platform.namespace || {};
    window.platform.namespace.openFromExago = this.openFromExago.bind(this);
  }

  private openFromExago(parameters) {
    this.ngZone.run(() => this.navigateFromExago(parameters));
  }

  private navigateFromExago(parameters) {
    this.reportingBiHelper.Redirect(parameters);
  }

  private doCypressLogin() {
    // Started sign in process in Cypress login command. Need to finish signing in now that AppComponent exists.
    const authorizedUser = JSON.parse(window.localStorage.authorizedUser);
    this.store.dispatch(new AuthActions.Signin(authorizedUser));
  }

  private setUpErrorModals() {
    this.errorNotificationService.errors$.pipe(takeUntil(this.unsubscribe)).subscribe((error) => {
      // prevent multiple error modals opening on top of one another, where only the top one can be closed.
      if (this.modalService.getErrorModalsCount() === 0) {
        this.pageTitleTranslations$.pipe(take(1)).subscribe((translations) => {
          this.modalService.createModal({
            template: ErrorModalComponent,
            title: translations['user.error'],
            extraData: [error],
            largeSize: false,
            type: 'alert',
            isError: true
          });
        });
      }
    });
  }

  private setUpPageTitles() {
    combineLatest([
      this.router.events.pipe(
        filter((event) => event instanceof NavigationEnd),
        map(() => {
          let child = this.activatedRoute.firstChild;
          while (child.firstChild) {
            child = child.firstChild;
          }
          if (child.snapshot.data['title']) {
            return child.snapshot.data['title'];
          }
          return this.titleService.getTitle();
        })
      ),
      this.pageTitleTranslations$
    ])
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((data) => {
        const titleKey: string = data[0];
        const translations: string[] = data[1];
        const title = translations[titleKey];
        // eslint-disable-next-line @typescript-eslint/no-unused-expressions
        isTruthy(title) ? this.titleService.setTitle(title) : this.titleService.setTitle(translations['title.main.amcsPlatform']);
      });
  }
}
