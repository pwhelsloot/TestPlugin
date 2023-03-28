import { Injectable } from '@angular/core';
import { NavigationEnd, NavigationExtras, Router } from '@angular/router';
import { CoreAppRoutes } from '@core-module/config/routes/core-app-routes.constants';
import { BaseService } from '@coreservices/base.service';
import { takeUntil } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class PreviousRouteService extends BaseService {

    constructor(private router: Router) {
        super();
        // Initalises properties from storage
        if (sessionStorage && sessionStorage.getItem('previousurls') != null) {
            this.previousUrls = JSON.parse(sessionStorage.getItem('previousurls'));
            this.index = +sessionStorage.getItem('previousurlindex');
        }
    }

    private previousUrls: string[] = new Array<string>(5);
    private index = 0;

    initialise() {
        this.router.events.pipe(takeUntil(this.unsubscribe)).subscribe(event => {
            if (event instanceof NavigationEnd) {
                // Don't want to track the not-found page
                if (event.url !== '/' + CoreAppRoutes.notFound && event.urlAfterRedirects !== '/' + CoreAppRoutes.notFound
                    && this.previousUrlIsNotSame(event.url)) {
                    this.previousUrls[this.index] = event.url;
                    this.nextIndex();
                    this.saveToStorage();
                }
            }
        });
    }

    getPreviousUrlString(): string {
        return this.getUrlStringAt(2);
    }

    getUrlAt(stepsBack: number): string {
        if (stepsBack < 1 || stepsBack > 5) {
            throw new Error('Steps back must be between 1 and 5.');
        }
        while (stepsBack > 0) {
            this.previousIndex();
            stepsBack--;
        }
        const url = this.previousUrls[this.index];
        if (url !== undefined) {
            return url;
        } else {
            return null;
        }
    }

    // Usually used if API call errored then route has been added but was actually invalid.
    removeCurrentUrl() {
        this.previousIndex();
        this.previousUrls[this.index] = null;
        this.saveToStorage();
    }

    navigationToPreviousUrl(commands: any[], extras?: NavigationExtras, urlAt: number = 2) {
        const previousUrl = this.getUrlAt(urlAt);
        if (previousUrl !== null) {
            this.router.navigateByUrl(previousUrl);
        } else {
            this.router.navigate(commands, extras);
        }
    }

    private getUrlStringAt(stepsBack: number) {
        const origIndex = this.index;
        if (stepsBack < 1 || stepsBack > 5) {
            throw new Error('Steps back must be between 1 and 5.');
        }
        while (stepsBack > 0) {
            this.previousIndex();
            stepsBack--;
        }
        const url = this.previousUrls[this.index];
        this.index = origIndex;
        if (url) {
            return url;
        } else {
            return null;
        }
    }

    // This logic just ensures we keep within the array size 5
    private previousIndex() {
        if (this.index === 0) {
            this.index = 4;
        } else {
            this.index--;
        }
    }

    private nextIndex() {
        if (this.index === 4) {
            this.index = 0;
        } else {
            this.index++;
        }
    }

    private saveToStorage() {
        if (sessionStorage) {
            sessionStorage.setItem('previousurls', JSON.stringify(this.previousUrls));
            sessionStorage.setItem('previousurlindex', this.index.toString());
        }
    }

    // This can sometimes happen if user reloads the app in the same session, we never want two duplicates
    // side by side.
    private previousUrlIsNotSame(currentUrl: string): boolean {
        this.previousIndex();
        const isSame = this.previousUrls[this.index] === currentUrl;
        this.nextIndex();
        return !isSame;
    }
}
