import { Router } from '@angular/router';
import { AuthorisationService } from '@auth-module/services/authorisation.service';
import { ApplicationConfigurationStore } from '@coreservices/config/application-configuration.store';
import { HeaderRoutingSeviceAdapter } from '@core-module/services/header/header-routing.service.abstract';
import { TemplateHeaderRoutingService } from './template-header-routing.service';
import { TemplateHeaderItemsService } from './template-header-items.service';
import { CoreAppRoutingService } from '@core-module/services/config/core-app-routing.service';
import { CoreTranslationsService } from '@core-module/services/translation/core-translation.service';

/*
The template-service-adapter.ts allows you to provide your own services to control the header routing / header items and sidebar routing / sidebar nodes
via the functions below. This is an example of how to make use of generic components.
This lets you control what you allow users to navigate too and see feature wise.
This is a required file that is needed when setting up a new app as it has to be provided in app.module.ts for the CoreModule.forRoot()
*/

export function createHeaderRoutingAdapter(
  router: Router,
  coreTranslations: CoreTranslationsService,
  authorisationService: AuthorisationService,
  coreAppRoutingService: CoreAppRoutingService
): HeaderRoutingSeviceAdapter {
  return new TemplateHeaderRoutingService(router, coreTranslations, authorisationService, coreAppRoutingService);
}

export function createHeaderItemsAdapter(
  coreTranslations: CoreTranslationsService,
  authorisationService: AuthorisationService,
  configService: ApplicationConfigurationStore,
  coreAppRoutingService: CoreAppRoutingService
) {
  return new TemplateHeaderItemsService(coreTranslations, authorisationService, configService, coreAppRoutingService);
}
