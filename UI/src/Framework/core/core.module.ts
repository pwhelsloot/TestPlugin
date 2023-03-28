import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ModuleWithProviders, NgModule, Provider } from '@angular/core';
import { AuthEffects } from '@auth-module/store/auth.effects';
import { AuthInterceptor } from '@core-module/http/auth.interceptor';
import { ErrorInterceptor } from '@core-module/http/error.interceptor';
import { LanguageInterceptor } from '@core-module/http/language.interceptor';
import { LoggingInterceptor } from '@core-module/http/logging.interceptor';
import { GlossaryService } from '@core-module/services/glossary/glossary.service';
import { AiLoggingServiceWrapper } from '@core-module/services/logging/ai-logging.service.wrapper';
import { reducers } from '@core-module/store/app.reducers';
import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { AmcsModalService } from '@shared-module/components/amcs-modal/amcs-modal.service';
import { SharedModule } from '@shared-module/shared.module';
import { FeatureFlagModule } from './services/feature-flag/feature-flag.module';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    StoreModule.forRoot(reducers),
    EffectsModule.forRoot([AuthEffects]),
    FeatureFlagModule,
  ],
  exports: [],
  providers: [
    AmcsModalService, // RDM - Too scared to mark as providedIn: 'root' as this seems to be provided in a few places, safest to leave it here.
    { provide: HTTP_INTERCEPTORS, useClass: LanguageInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: LoggingInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
  ],
})
export class CoreModule {
  static forRoot(
    coreAppMessagingAdapter: Provider,
    platformConfigurationAdapter: Provider,
    platformInitialisationAdapter: Provider,
    platformAuthenticatedUserAdapter: Provider,
    headerRoutingAdapter: Provider,
    headerItemsAdapter: Provider
  ): ModuleWithProviders<CoreModule> {
    return {
      ngModule: CoreModule,
      providers: [
        coreAppMessagingAdapter,
        platformConfigurationAdapter,
        platformInitialisationAdapter,
        platformAuthenticatedUserAdapter,
        headerRoutingAdapter,
        headerItemsAdapter,
      ],
    };
  }

  // Allows the glossaryService to eagerly load (won't wait for it to be loaded in a GlossaryPipe)
  constructor(readonly glossaryService: GlossaryService, readonly aiLoggingService: AiLoggingServiceWrapper) {}
}
