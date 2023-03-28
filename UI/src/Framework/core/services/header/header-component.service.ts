import { Injectable, OnDestroy } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import * as AuthActions from '@auth-module/store/auth.actions';
import { CoreAppRoutes } from '@core-module/config/routes/core-app-routes.constants';
import * as fromApp from '@core-module/store/app.reducers';
import { ApiRequest } from '@coremodels/api/api-request.model';
import { ApplicationConfiguration } from '@coremodels/application-configuration.model';
import { ApplicationConfigurationStore } from '@coreservices/config/application-configuration.store';
import { ErpApiService } from '@coreservices/erp-api.service';
import { GlobalSearchServiceUI } from '@coreservices/global-search/global-search.service.ui';
import { InstrumentationService } from '@coreservices/logging/instrumentationService.service';
import { LoggingVerbs } from '@coreservices/logging/loggingVerbs.model';
import { TrackingEventType } from '@coreservices/logging/tracking-event-type.enum';
import { environment } from '@environments/environment';
import { Store } from '@ngrx/store';
import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { Subscription } from 'rxjs';
import { HeaderMenuFormService } from './header-menu-form.service';
import { HeaderService } from './header.service';

/**
 * @deprecated Move to PlatformUI
 */
@Injectable({ providedIn: 'root' })
export class HeaderComponentService implements OnDestroy {

    sysUserId: number;
    userName: string;
    shortUserName: string;
    searchTerm: string;
    searchForm: AmcsFormGroup;
    isOpen: boolean;
    isCommunicationsOpen: boolean;
    isNotificationsOpen: boolean;
    showAboutLink = false;
    clickOnceExtensionURLFirefox: string;
    clickOnceExtensionURLChrome: string;
    showDevtools = false;

    constructor(
        public headerService: HeaderService,
        public headerMenuService: HeaderMenuFormService,
        public globalSearchServiceUI: GlobalSearchServiceUI,
        private router: Router,
        private erpApiService: ErpApiService,
        private store: Store<fromApp.AppState>,
        private formBuilder: FormBuilder,
        private instrumentationService: InstrumentationService,
        private applicationConfigService: ApplicationConfigurationStore
    ) {

        this.isOpen = false;
        this.searchForm = new AmcsFormGroup(this.formBuilder.group({
            searchTerm: [this.searchTerm, [Validators.required, Validators.minLength(3)]]
        }));
        const authStore = this.store.select('auth');

        this.userNameSubscription = authStore.subscribe(s => {
          this.userName = s.username;
          this.shortUserName = s.username === null || s.username.length <= 8 ? s.username : s.username.slice(0, 6) + '..';
        });

        this.configSubscription = this.applicationConfigService.configuration$.subscribe((config: ApplicationConfiguration) => {
            this.showDevtools = config.buttonConfiguration.showHeaderDevtools;
            this.showAboutLink = config.buttonConfiguration.showHeaderERPAbout;
            this.clickOnceExtensionURLFirefox = config.clickOnceExtensionURLFirefox;
            this.clickOnceExtensionURLChrome = config.clickOnceExtensionURLChrome;
        });
    }

    private userNameSubscription: Subscription;
    private configSubscription: Subscription;

    goToDashboard() {
        this.headerService.isSearchedClicked.next(false);
        this.router.navigate([CoreAppRoutes.userModule, CoreAppRoutes.customerServiceDashboard]);
    }

    goToReportsExpanded() {
        this.headerService.isSearchedClicked.next(false);
        this.router.navigateByUrl(CoreAppRoutes.customerModule + '/1/' + CoreAppRoutes.report);
    }

    goHome() {
        this.headerService.isSearchedClicked.next(false);
        this.router.navigate([CoreAppRoutes.homeModule + '/' + CoreAppRoutes.search]);
    }

    goToDevHome() {
        this.isOpen = false;
        this.headerService.isSearchedClicked.next(false);
        this.router.navigate([CoreAppRoutes.homeModule + '/' + CoreAppRoutes.devHome]);
    }

    goToMyProfile() {
        this.isOpen = false;
        this.headerService.isSearchedClicked.next(false);
        this.router.navigate([CoreAppRoutes.homeModule + '/' + CoreAppRoutes.myProfile]);
    }

    logOut() {
        this.isOpen = false;
        this.headerService.isSearchedClicked.next(false);
        this.headerService.logOutSelected.emit(true);
        this.store.dispatch(new AuthActions.TrySignout());
    }

    clearInput(event: MouseEvent) {
        if (event.screenY !== 0 && event.screenX !== 0) {
            this.searchForm.formGroup.reset();
            this.searchTerm = null;
        }
    }

    expandProfileMenu() {
        this.isOpen = !this.isOpen;
    }

    expandCommunicationsMenu() {
        this.isCommunicationsOpen = !this.isCommunicationsOpen;
    }

    expandNotificationsMenu() {
        this.isNotificationsOpen = !this.isNotificationsOpen;
    }

    displaySearchBar() {
        this.headerService.isSearchedClicked.next(!this.headerService.isSearchedClicked.getValue());
        if (this.headerService.isSearchedClicked) {
            this.globalSearchServiceUI.focusRequested.next(true);
        }
    }

    launchDesktopClient() {
        this.instrumentationService.trackEvent(LoggingVerbs.EventLaunchDesktop, null, null, TrackingEventType.Desktop);

        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['authentication/elemosToken'];
        apiRequest.postData = '';

        const getElemosTokenPostResult =
            (response: { Token: { value: string } }) => {
                const baseURL = environment.elemosClientURL;
                const token = response.Token;
                const queryParamPrefix = baseURL.indexOf('?') > -1 ? '&' : '?';
                const url = baseURL + queryParamPrefix + 'token=' + token;
                this.instrumentationService.trackEvent(LoggingVerbs.EventLaunchDesktopTokenisedCall, { launchUrl: url }, null, TrackingEventType.Desktop);
                location.replace(url);

            };
        this.erpApiService.postRequest<{ Token: { value: string } }>(apiRequest, getElemosTokenPostResult).subscribe();
    }

    ngOnDestroy() {
        this.userNameSubscription.unsubscribe();
        this.configSubscription.unsubscribe();
    }
}
