
import { Injectable, NgZone } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PlatformConfigurationServiceData } from '@core-module/services/config/data/platform-configuration.service.data';
import { PlatformConfigurationAdapter } from '@core-module/services/config/platform-configuration.adapter';
import { PlatformTitleService } from '@core-module/services/config/platform-title.service';
import { ErrorNotificationService } from '@core-module/services/error-notification.service';
import { ReportingBiHelperService } from '@core-module/services/reportingBi/reportingBi-helper';
import * as fromApp from '@core-module/store/app.reducers';
import { Store } from '@ngrx/store';
import { AmcsModalService } from '@shared-module/components/amcs-modal/amcs-modal.service';
import { Observable, of } from 'rxjs';
import { PlatformConfiguration } from '../core/models/config/platform-configuration';
import { FakePlatformConfiguration } from './fake-platform-configuration';

@Injectable()
export class FakePlatformConfigurationService extends PlatformConfigurationAdapter {
  configuration$: Observable<PlatformConfiguration>;
  urlKey = '';

  constructor(
    errorNotificationService: ErrorNotificationService,
    modalService: AmcsModalService,
    store: Store<fromApp.AppState>,
    dataService: PlatformConfigurationServiceData,
    ngZone: NgZone,
    reportingBiHelper: ReportingBiHelperService,
    titleService: PlatformTitleService,
    router: Router,
    activatedRoute: ActivatedRoute
  ) {
    super(errorNotificationService, modalService, store,
      dataService, ngZone, reportingBiHelper, titleService, router, activatedRoute);
    this.setUp(FakePlatformConfiguration, of([]));
  }
}
