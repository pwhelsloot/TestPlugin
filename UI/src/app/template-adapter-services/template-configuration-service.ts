import { Injectable, NgZone } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PlatformConfiguration } from '@core-module/models/config/platform-configuration';
import { PlatformConfigurationServiceData } from '@core-module/services/config/data/platform-configuration.service.data';
import { PlatformConfigurationAdapter } from '@core-module/services/config/platform-configuration.adapter';
import { PlatformTitleService } from '@core-module/services/config/platform-title.service';
import { ErrorNotificationService } from '@core-module/services/error-notification.service';
import { ReportingBiHelperService } from '@core-module/services/reportingBi/reportingBi-helper';
import * as fromApp from '@core-module/store/app.reducers';
import { Store } from '@ngrx/store';
import { AmcsModalService } from '@shared-module/components/amcs-modal/amcs-modal.service';
import { Observable } from 'rxjs';
import { TemplateAppTranslationsService } from 'translate/template-app-translations.service';

@Injectable({ providedIn: 'root' })
export class TemplateConfigurationService extends PlatformConfigurationAdapter {
  urlKey: string;
  configuration$: Observable<PlatformConfiguration>;
  fakeConfigCall = true;

  constructor(
    errorNotificationService: ErrorNotificationService,
    modalService: AmcsModalService,
    store: Store<fromApp.AppState>,
    ngZone: NgZone,
    reportingBiHelperService: ReportingBiHelperService,
    dataService: PlatformConfigurationServiceData,
    titleService: PlatformTitleService,
    router: Router,
    activatedRoute: ActivatedRoute,
    templateAppTranslations: TemplateAppTranslationsService
  ) {
    super(
      errorNotificationService,
      modalService,
      store,
      dataService,
      ngZone,
      reportingBiHelperService,
      titleService,
      router,
      activatedRoute
    );
    this.setUp(PlatformConfiguration, templateAppTranslations.translations);
  }
}
