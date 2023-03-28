import { Injectable, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import * as AuthActions from '@auth-module/store/auth.actions';
import { CoreAppRouting } from '@core-module/models/config/core-app-routing';
import * as fromApp from '@core-module/store/app.reducers';
import { Store } from '@ngrx/store';
import { combineLatest, Observable, Subject } from 'rxjs';
import { filter, map, shareReplay, take, takeUntil } from 'rxjs/operators';
import { BaseService } from '../base.service';
import { InstrumentationService } from '../logging/instrumentationService.service';
import { LoggingVerbs } from '../logging/loggingVerbs.model';
import { PreviousRouteService } from '../previous-route.service';
import { RouteContextService } from '../route-context.service';
import { ApplicationConfigurationService } from './application-configuration.service';
import { ApplicationConfigurationStore } from './application-configuration.store';
import { CoreAppMessagingAdapter } from './core-app-messaging.service';
import { CoreAppRoutingConfigHelper } from './core-app-routing-config-helper';
import { CoreAppRoutingService } from './core-app-routing.service';
import { PlatformConfigurationAdapter } from './platform-configuration.adapter';
import { IPlatformInitialisationService } from './platform-initialisation.service.interface';

@Injectable()
// Deals with initialsation needed for app to run
export abstract class PlatformInitialisationAdapter extends BaseService implements OnDestroy, IPlatformInitialisationService {
  initialised$: Observable<void>;
  authStatusFinished = new Subject();

  constructor(
    protected appConfigStore: ApplicationConfigurationStore,
    protected previousRouteService: PreviousRouteService,
    protected store: Store<fromApp.AppState>,
    protected instrumentationService: InstrumentationService,
    protected configService: ApplicationConfigurationService,
    protected routeContextService: RouteContextService,
    protected platformConfigurationAdapter: PlatformConfigurationAdapter,
    protected coreAppMessagingService: CoreAppMessagingAdapter,
    protected coreAppRoutingService: CoreAppRoutingService,
    protected router: Router
  ) {
    super();
  }

  protected getInitialisationStreams() {
    return [this.platformConfigurationAdapter.initialised$, this.appConfigStore.initialsed$];
  }

  protected initialise() {
    this.initialised$ = combineLatest(this.getInitialisationStreams()).pipe(
      take(1),
      map(() => {
        this.store.dispatch(new AuthActions.TryInitialise());
        return;
      }),
      shareReplay(1)
    );
    this.instrumentationService.startTrackTimedEvent(LoggingVerbs.AppLoading);
    this.platformConfigurationAdapter.initialise();
    // TODO - Deprecate below in favour of above call (dont need two config services)
    this.configService.initialise();
    this.previousRouteService.initialise();
    this.routeContextService.initialise();
    this.coreAppMessagingService.initialise();
    this.coreAppRoutingService.coreAppRoutes$
      .pipe(
        takeUntil(this.unsubscribe),
        filter((routesForNav) => routesForNav.length > 0)
      )
      .subscribe((routesForNav: CoreAppRouting[]) => {
        CoreAppRoutingConfigHelper.updateRouterConfig(this.router, routesForNav);
      });
    this.instrumentationService.stopTrackTimedEvent(LoggingVerbs.AppLoading);
  }
}
