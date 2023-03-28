import { Injectable } from '@angular/core';
import { AuthorisationService } from '@auth-module/services/authorisation.service';
import { ApplicationConfiguration } from '@core-module/models/application-configuration.model';
import { CoreAppRouting } from '@core-module/models/config/core-app-routing';
import { CoreTranslationsService } from '@core-module/services/translation/core-translation.service';
import { AmcsMenuItem } from '@shared-module/models/amcs-menu-item.model';
import { combineLatest, Observable, ReplaySubject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';
import { BaseService } from '../base.service';
import { ApplicationConfigurationStore } from '../config/application-configuration.store';
import { CoreAppRoutingService } from '../config/core-app-routing.service';
import { IHeaderItemsService } from './header-items.interface';

@Injectable()
export abstract class HeaderItemsServiceAdapter extends BaseService implements IHeaderItemsService {

    items$: Observable<AmcsMenuItem[]>;

    constructor(coreTranslations: CoreTranslationsService,
        authorisationService: AuthorisationService,
        configService: ApplicationConfigurationStore,
        protected coreAppRoutingService: CoreAppRoutingService) {
        super();
        this.setupItemsStream(coreTranslations, authorisationService, configService);
    }

    protected itemsSubject = new ReplaySubject<AmcsMenuItem[]>(1);

    // Populates the available items. Note this is automatically pushed onto items$.
    protected abstract populateItems(translations: string[], config: ApplicationConfiguration, authorisationService: AuthorisationService): AmcsMenuItem[];

    protected populateCrossAppItems(coreAppRoutes: CoreAppRouting[]): AmcsMenuItem[] {
        return coreAppRoutes.map((route: CoreAppRouting) => {
            return new AmcsMenuItem(route.sourcePrefix, route.navigationText, route.iconUrl);
        });
    }

    // Handles the stream for the items
    protected setupItemsStream(coreTranslations: CoreTranslationsService,
        authorisationService: AuthorisationService,
        configService: ApplicationConfigurationStore): void {
        this.items$ = this.itemsSubject.asObservable();
        combineLatest([authorisationService.authorisationClaims, coreTranslations.translations, configService.configuration$, this.coreAppRoutingService.coreAppRoutes$])
            .pipe(
                filter(x => x[0].length > 0),
                takeUntil(this.unsubscribe))
            .subscribe(data => {
                const translations: string[] = data[1];
                const config: ApplicationConfiguration = data[2];
                const coreAppRoutes: CoreAppRouting[] = data[3];
                const menuItems: AmcsMenuItem[] = this.populateItems(translations, config, authorisationService);
                const coreAppMenuItems = this.populateCrossAppItems(coreAppRoutes);
                this.itemsSubject.next(menuItems.concat(coreAppMenuItems));
            });
    }
}
