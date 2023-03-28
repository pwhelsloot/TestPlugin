import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppLoadingService } from '@core-module/services/config/app-loading.service';
import { PlatformAuthenticatedUserAdapter } from '@core-module/services/config/platform-authenticated-user.adapter';
import { CoreTranslationsService } from '@core-module/services/translation/core-translation.service';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.scss'],
})
// RDM - This is the landing page for each app that the core app will route us to after login.
// Once login is initialised we'll navigate to the correct page for the application
export class IndexComponent implements OnInit {
  initialisingMessage: string;
  appInitialised = false;

  constructor(
    private authenticatedUserAdapter: PlatformAuthenticatedUserAdapter,
    private translationsService: CoreTranslationsService,
    private activatedRoute: ActivatedRoute,
    private appLoadingService: AppLoadingService
  ) {}

  ngOnInit() {
    this.translationsService.translations.pipe(take(1)).subscribe((translations: string[]) => {
      this.initialisingMessage = translations['app.initialising'];
    });
    this.authenticatedUserAdapter.initialised$.pipe(take(1)).subscribe(() => {
      this.appLoadingService.set(true);
      this.activatedRoute.queryParams.pipe(take(1)).subscribe((params) => {
        if (!params['ext']) {
          this.authenticatedUserAdapter.navigateToHomeAfterCoreAppLogin();
        }
      });
    });
  }
}
