import { Router } from '@angular/router';
import { DifferentAppGuard } from '@core-module/guards/different-app.guard';
import { CoreAppRouting } from '@core-module/models/config/core-app-routing';
import { environment } from '@environments/environment';
import { EmptyRouteComponent } from '@shared-module/empty-route/empty-route.component';

export class CoreAppRoutingConfigHelper {

   static updateRouterConfig(router: Router, coreAppRoutes: CoreAppRouting[]) {
        coreAppRoutes.forEach((coreAppRoute: CoreAppRouting) => {
            let trimmedSourcePrefix: string = coreAppRoute.sourcePrefix;
            if (trimmedSourcePrefix.startsWith('/')) {
                trimmedSourcePrefix = trimmedSourcePrefix.substring(1);
            }
            router.config.unshift(
                {   path: trimmedSourcePrefix,
                    component: EmptyRouteComponent,
                    canActivate: [DifferentAppGuard],
                    children: [
                        { path: '**', component: EmptyRouteComponent, canActivate: [DifferentAppGuard] }
                    ] }
                );
        });

        router.config.unshift(
        {
            path: environment.applicationURLPrefix + '?ext=true',
            component: EmptyRouteComponent,
            pathMatch: 'full',
        });
    }
}
