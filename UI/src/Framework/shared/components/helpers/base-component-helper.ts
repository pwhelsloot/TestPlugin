import { Router, ActivatedRoute, NavigationStart, NavigationEnd, ResolveEnd, ActivatedRouteSnapshot } from '@angular/router';
import { InstrumentationService } from '@core-module/services/logging/instrumentationService.service';
import { filter, take } from 'rxjs/operators';
import { Subscription } from 'rxjs';

/**
 * @deprecated Marked for removal, please use the @aiComponent decorator instead.
 */
export class BaseComponentHelper {
    startTime: number;

    constructor(public router: Router, public activatedRoute: ActivatedRoute, public instrumentationService: InstrumentationService, public name: string) {

        // Manually retrieve the monitoring service from the injector
        // so that constructor has no dependencies that must be passed in from child
        // const injector = ReflectiveInjector.resolveAndCreate([InstrumentationService]);

        // this.instrumentationService = injector.get(InstrumentationService);

        this.navStartEvent = this.router.events
            .pipe(
                filter(event => event instanceof NavigationStart),
                take(1)
            )
            .subscribe((event: NavigationStart) => {
                // this.name is not available until nav end has fired
                this.instrumentationService.startTrackPage(event.url);
            });

        this.navEndEvent = this.router.events
            .pipe(
                filter(event => event instanceof NavigationEnd),
                take(1)
            )
            .subscribe((event: NavigationEnd) => {
                // this.name is not available until nav end has fired
                this.instrumentationService.stopTrackPage(this.name, event.url);
                window.scrollTo(0, 0);
            });

        this.routerSubscription = this.router.events
            .pipe(filter(event => event instanceof ResolveEnd))
            .subscribe((event: ResolveEnd) => {
                const activatedComponent = this.getActivatedComponent(event.state.root);
                if (activatedComponent) {
                    this.instrumentationService.setUrlComponentName(event.url, activatedComponent.name);
                }
            });
    }

    private navStartEvent: Subscription;
    private navEndEvent: Subscription;
    private routerSubscription: Subscription;

    destroy() {
        this.navStartEvent.unsubscribe();
        this.navEndEvent.unsubscribe();
        this.routerSubscription.unsubscribe();
    }

    private getActivatedComponent(snapshot: ActivatedRouteSnapshot): any {

        if (snapshot.firstChild) {
            return this.getActivatedComponent(snapshot.firstChild);
        }

        return snapshot.component;
    }
}
