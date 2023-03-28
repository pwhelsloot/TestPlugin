import { Injectable } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { AuthorisationService } from '@auth-module/services/authorisation.service';
import { AmcsMenuItem } from '@shared-module/models/amcs-menu-item.model';
import { combineLatest, Observable } from 'rxjs';
import { filter, take, takeUntil } from 'rxjs/operators';
import { BaseService } from '../base.service';
import { HeaderItemsServiceAdapter } from './header-items.service.abstract';
import { HeaderRoutingSeviceAdapter } from './header-routing.service.abstract';

/**
 * @deprecated Move to PlatformUI
 */
@Injectable({ providedIn: 'root' })
export class HeaderMenuFormService extends BaseService {
    items$: Observable<AmcsMenuItem[]>;
    activeItem: AmcsMenuItem = null;

    constructor(
        private headerRoutingSevice: HeaderRoutingSeviceAdapter,
        private headerItemsService: HeaderItemsServiceAdapter,
        private router: Router,
        private authorisationService: AuthorisationService
    ) {
        super();

        this.items$ = this.headerItemsService.items$;

        combineLatest([this.router.events, this.headerItemsService.items$])
            .pipe(
                filter((data) => data[0] instanceof NavigationEnd),
                takeUntil(this.unsubscribe)
            )
            .subscribe((data) => {
                const event = data[0] as NavigationEnd;
                const items: AmcsMenuItem[] = data[1];
                // After navigation is complete, select menu item based on the url
                const key = this.headerRoutingSevice.getMenuItemKeyFromUrl(event.url);
                this.setSelectedItemFromKey(items, key);
            });

        // Initial setting of menu items. Sometimes router event has already been and gone by the time this
        // service starts up
        let initialKey = this.headerRoutingSevice.getMenuItemKeyFromUrl(this.router.routerState.snapshot.url);
        this.items$.pipe(take(1)).subscribe((items: AmcsMenuItem[]) => {
            if (!initialKey && this.router.routerState.snapshot.url) {
                initialKey = this.headerRoutingSevice.getMenuItemKeyFromUrl(this.router.routerState.snapshot.url);
            }
            this.setSelectedItemFromKey(items, initialKey);
        });
    }

    onItemClick(item: AmcsMenuItem) {
        this.navigateToItem(item);
    }

    private navigateToItem(item: AmcsMenuItem) {
        this.headerRoutingSevice.navigateToItem(item, this.authorisationService);
    }

    private setSelectedItemFromKey(items: AmcsMenuItem[], key: string) {
        items.forEach((item) => {
            item.isSelected = item.key === key;
            if (item.isSelected) {
                this.activeItem = item;
            }
        });
        if (items.every((x) => !x.isSelected)) {
            this.activeItem = null;
        }
    }
}
