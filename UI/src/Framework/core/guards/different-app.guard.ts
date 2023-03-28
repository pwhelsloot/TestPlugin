import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot } from '@angular/router';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { CoreAppRouting } from '@core-module/models/config/core-app-routing';
import { CoreAppMessagingAdapter } from '@core-module/services/config/core-app-messaging.service';
import { CoreAppRoutingService } from '@core-module/services/config/core-app-routing.service';
import { environment } from '@environments/environment';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class DifferentAppGuard implements CanActivate {
  constructor(
    private readonly coreAppRoutingService: CoreAppRoutingService,
    private readonly coreAppMessagingService: CoreAppMessagingAdapter
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    return this.coreAppRoutingService.coreAppRoutes$.pipe(
      map((routes: CoreAppRouting[]) => {
        const targetApp: CoreAppRouting = routes.find((x) => state.url.startsWith(x.sourcePrefix));
        if (this.isDifferentApp(targetApp)) {
          this.coreAppMessagingService.sendAppChangeNavigationRequest(targetApp.sourcePrefix, state.url);
        }
        // Don't navigate if a target app exists (core app will take you to it)
        return !isTruthy(targetApp);
      })
    );
  }

  private isDifferentApp(targetApp: CoreAppRouting) {
    return isTruthy(targetApp) && targetApp.sourcePrefix !== `/${environment.applicationURLPrefix}`;
  }
}
