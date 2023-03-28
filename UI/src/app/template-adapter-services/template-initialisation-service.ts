import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ApplicationConfigurationService } from '@core-module/services/config/application-configuration.service';
import { ApplicationConfigurationStore } from '@core-module/services/config/application-configuration.store';
import { CoreAppMessagingAdapter } from '@core-module/services/config/core-app-messaging.service';
import { CoreAppRoutingService } from '@core-module/services/config/core-app-routing.service';
import { PlatformInitialisationAdapter } from '@core-module/services/config/platform-initialisation.adapter';
import { InstrumentationService } from '@core-module/services/logging/instrumentationService.service';
import { PreviousRouteService } from '@core-module/services/previous-route.service';
import { RouteContextService } from '@core-module/services/route-context.service';
import { Store } from '@ngrx/store';
import * as fromApp from '@core-module/store/app.reducers';
import { TemplateConfigurationService } from '@app/template-adapter-services/template-configuration-service';

@Injectable({ providedIn: 'root' })
export class TemplateInitialisationService extends PlatformInitialisationAdapter {
  constructor(
    appConfigStore: ApplicationConfigurationStore,
    previousRouteService: PreviousRouteService,
    store: Store<fromApp.AppState>,
    instrumentationService: InstrumentationService,
    configService: ApplicationConfigurationService,
    routeContextService: RouteContextService,
    coreAppMessagingService: CoreAppMessagingAdapter,
    coreAppRoutingService: CoreAppRoutingService,
    router: Router,
    protected readonly templateConfigurationService: TemplateConfigurationService
  ) {
    super(
      appConfigStore,
      previousRouteService,
      store,
      instrumentationService,
      configService,
      routeContextService,
      templateConfigurationService,
      coreAppMessagingService,
      coreAppRoutingService,
      router
    );
    this.initialise();
  }

  protected getInitialisationStreams() {
    return [this.templateConfigurationService.initialised$];
  }
}
