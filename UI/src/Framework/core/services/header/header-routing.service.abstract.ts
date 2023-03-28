import { EventEmitter, Injectable } from '@angular/core';
import { NavigationEnd, Router, RouterEvent } from '@angular/router';
import { AuthorisationService } from '@auth-module/services/authorisation.service';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { CoreAppRouting } from '@core-module/models/config/core-app-routing';
import { CoreTranslationsService } from '@core-module/services/translation/core-translation.service';
import { BaseService } from '@coreservices/base.service';
import { environment } from '@environments/environment';
import { AmcsMenuItem } from '@shared-module/models/amcs-menu-item.model';
import { filter, take, takeUntil } from 'rxjs/operators';
import { CoreAppRoutingService } from '../config/core-app-routing.service';
import { IHeaderRoutingService } from './header-routing.interface';

@Injectable()
export abstract class HeaderRoutingSeviceAdapter extends BaseService implements IHeaderRoutingService {

    // the active nodes name
    activeNodeName: EventEmitter<string> = new EventEmitter<string>();

    constructor(
        protected router: Router,
        coreTranslations: CoreTranslationsService,
        protected authorisationService: AuthorisationService,
        private coreAppRoutingService: CoreAppRoutingService) {
        super();
        this.router.events.pipe(takeUntil(this.unsubscribe),
            filter(event => (event instanceof NavigationEnd)))
            .subscribe((navigation: RouterEvent) => {
                this.activeNodeName.emit(this.findActiveNodeFromUrl(navigation.url));
                const url = navigation.url.split('/');
                this.parseContextFromUrl(url);
            });

        coreTranslations.translations.pipe(takeUntil(this.unsubscribe)).subscribe((translations: string[]) => {
            this.translations = translations;
        });
    }

    protected translations: string[];

    // Navigates to a selected node
    abstract navigateToItem(node: AmcsMenuItem, authorisationService: AuthorisationService): void;

    // Gets the menu item for a URL
    getMenuItemKeyFromUrl(url: string): string {
        let key: string = null;

        if (isTruthy(url)) {
            url = url.replace(environment.applicationURLPrefix + '/', '');
            const splUrl = url.split('/');
            if (splUrl.length >= 2) {
                key = splUrl[1];
            }
        }
        return key;
    }

    tryCrossAppNavigation(prefix: string) {
        this.coreAppRoutingService.coreAppRoutes$.pipe(take(1)).subscribe((routes: CoreAppRouting[]) => {
              if (routes.find(x => x.sourcePrefix === prefix)) {
                    this.router.navigate([prefix]);
                }
            });
    }

    // Optional - Gives you a chance to parse extra context out of url header
    protected abstract parseContextFromUrl(url: string[]): void;

    // Returns the node name for the url feature we are under
    protected abstract getNodeNameFromFeatureName(featureName: string): string;

    // Parses the feature name (first string in url) from the active url.
    private findActiveNodeFromUrl(url: string): string {
        url = url.substring(1, url.length); // takes out first '/'
        let index = url.indexOf('/'); // find position of second '/'
        // there might not be a second '/' if not just use whole string
        if (index === -1) {
            index = url.length;
        }
        const featureName = url.substring(0, index); // pull feature name out
        return this.getNodeNameFromFeatureName(featureName);
    }
}
