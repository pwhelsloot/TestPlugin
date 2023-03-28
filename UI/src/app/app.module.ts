import { ExtensibilityModule } from '@amcs-extensibility/host';
import { APP_BASE_HREF, registerLocaleData } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import localedeDE from '@angular/common/locales/de';
import localeenAU from '@angular/common/locales/en-AU';
import localenGB from '@angular/common/locales/en-GB';
import localeesMX from '@angular/common/locales/es-MX';
import localeesUs from '@angular/common/locales/es-US';
import localeFR from '@angular/common/locales/fr';
import localeNB from '@angular/common/locales/nb';
import localeNL from '@angular/common/locales/nl';
import { ErrorHandler, LOCALE_ID, NgModule } from '@angular/core';
import { HammerModule, HAMMER_GESTURE_CONFIG, Title } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { Router } from '@angular/router';
import { GlobalErrorHandler } from '@app/global-error-handler';
import { TemplateAuthenticatedUserService } from '@app/template-adapter-services/template-authenticated-user.service';
import { TemplateConfigurationService } from '@app/template-adapter-services/template-configuration-service';
import { TemplateInitialisationService } from '@app/template-adapter-services/template-initialisation-service';
import { TemplateMessagingService } from '@app/template-adapter-services/template-messaging.service';
import { TemplateSharedModule } from '@app/template-shared/template-shared.module';
import { AuthorisationService } from '@auth-module/services/authorisation.service';
import { CoreModule } from '@core-module/core.module';
import { CoreAppMessagingAdapter } from '@core-module/services/config/core-app-messaging.service';
import { CoreAppRoutingService } from '@core-module/services/config/core-app-routing.service';
import { PlatformAuthenticatedUserAdapter } from '@core-module/services/config/platform-authenticated-user.adapter';
import { PlatformConfigurationAdapter } from '@core-module/services/config/platform-configuration.adapter';
import { PlatformInitialisationAdapter } from '@core-module/services/config/platform-initialisation.adapter';
import { HeaderItemsServiceAdapter } from '@core-module/services/header/header-items.service.abstract';
import { HeaderRoutingSeviceAdapter } from '@core-module/services/header/header-routing.service.abstract';
import { CoreTranslationsService } from '@core-module/services/translation/core-translation.service';
import { ApplicationConfigurationStore } from '@coreservices/config/application-configuration.store';
import { environment } from '@environments/environment';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { HammerConfig } from '@shared-module/components/amcs-swipe-options/hammer-config.service';
import { SharedModule } from '@shared-module/shared.module';
import { AppLocaleService } from '@translate/app-locale.service';
import { MultiTranslateHttpLoader } from '@translate/multi-translation-file-loader';
import { ResizableModule } from 'angular-resizable-element';
import { TemplateAppTranslationsService } from 'translate/template-app-translations.service';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { createHeaderItemsAdapter, createHeaderRoutingAdapter } from './template-adapter-services/template-service-adapters';

registerLocaleData(localeFR);
registerLocaleData(localenGB);
registerLocaleData(localeNB);
registerLocaleData(localeesMX);
registerLocaleData(localeesUs);
registerLocaleData(localeenAU);
registerLocaleData(localeNL);
registerLocaleData(localedeDE);

export function createTranslateLoader(http: HttpClient) {
  return new MultiTranslateHttpLoader(http, [
    { prefix: './Framework/assets/i18n/shared/', suffix: '.json' },
    { prefix: './assets/i18n/app/', suffix: '.json' },
    { prefix: './Framework/assets/i18n/core/', suffix: '.json' },
  ]);
}

@NgModule({
  declarations: [AppComponent],
  imports: [
    ExtensibilityModule.forRoot(environment.applicationURLPrefix),
    CoreModule.forRoot(
      {
        provide: CoreAppMessagingAdapter,
        useExisting: TemplateMessagingService,
      },
      {
        provide: PlatformConfigurationAdapter,
        useExisting: TemplateConfigurationService,
      },
      {
        provide: PlatformInitialisationAdapter,
        useExisting: TemplateInitialisationService,
      },
      {
        provide: PlatformAuthenticatedUserAdapter,
        useExisting: TemplateAuthenticatedUserService,
      },
      {
        provide: HeaderRoutingSeviceAdapter,
        useFactory: createHeaderRoutingAdapter,
        deps: [Router, CoreTranslationsService, AuthorisationService, CoreAppRoutingService],
      },
      {
        provide: HeaderItemsServiceAdapter,
        useFactory: createHeaderItemsAdapter,
        deps: [CoreTranslationsService, AuthorisationService, ApplicationConfigurationStore, CoreAppRoutingService],
      }
    ),
    HttpClientModule,
    SharedModule,
    TemplateSharedModule,
    AppRoutingModule,
    ResizableModule,
    BrowserAnimationsModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient],
      },
      useDefaultLang: false,
      isolate: true,
    }),
    HammerModule,
  ],
  exports: [CoreModule],
  providers: [
    TemplateAppTranslationsService,
    {
      provide: ErrorHandler,
      useClass: GlobalErrorHandler,
    },
    {
      provide: LOCALE_ID,
      deps: [AppLocaleService],
      useFactory: (appLocaleService) => appLocaleService.getLocale(),
    },
    { provide: APP_BASE_HREF, useValue: '/' },
    Title,
    { provide: HAMMER_GESTURE_CONFIG, useClass: HammerConfig },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
