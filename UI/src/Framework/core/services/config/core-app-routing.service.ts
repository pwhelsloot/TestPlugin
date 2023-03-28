import { PlatformHeaderNavigationItem } from '@amcs/platform-communication';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { CoreAppRoutes } from '@core-module/config/routes/core-app-routes.constants';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { CoreAppModulesEnum } from '@core-module/models/config/core-app-modules.enum';
import { CoreAppRouting } from '@core-module/models/config/core-app-routing';
import { BaseService } from '@core-module/services/base.service';
import { ContainerAppMessagingHandlerService } from '@core-module/services/config/messaging/messaging-handler.service';
import { environment } from '@environments/environment';
import { Observable, ReplaySubject } from 'rxjs';
import { map, shareReplay, take, tap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class CoreAppRoutingService extends BaseService {
  coreAppRoutes$: Observable<CoreAppRouting[]>;
  initalised$: Observable<boolean>;

  constructor(
    private messagingService: ContainerAppMessagingHandlerService,
    private router: Router
  ) {
    super();
    this.setUpObservables();
  }

  private initialised = new ReplaySubject<boolean>(1);

  navigateToModule(moduleName: CoreAppModulesEnum, url: string) {
    this.coreAppRoutes$.pipe(take(1)).subscribe((coreApps) => {
      const coreApp = coreApps.find(
        (x) => this.trimStart(x.sourcePrefix) === this.getSourcePrefixFromModuleName(moduleName)
      );
      if (isTruthy(coreApp)) {
        const fullUrl = coreApp.sourcePrefix + (isTruthy(url) ? url : '');
        this.router.navigateByUrl(fullUrl);
      } else {
        this.router.navigate([CoreAppRoutes.notFound]);
      }
    });
  }

  private setUpObservables() {
    this.initalised$ = this.initialised.asObservable();
    this.coreAppRoutes$ = this.messagingService.headerNavigationItems$.pipe(
      map((menuItems) => {
        return menuItems
          .filter((menuItem) => menuItem.sourcePrefix !== `/${environment.applicationURLPrefix}`)
          .map((menuItem) => this.buildCoreAppRoute(menuItem));
      }),
      tap(() => {
        this.initialised.next(true);
      }),
      shareReplay(1)
    );
  }

  private buildCoreAppRoute(item: PlatformHeaderNavigationItem): CoreAppRouting {
    return {
      navigationText: item.title,
      navigationPrefix: item.navigationPrefix,
      sourcePrefix: item.sourcePrefix,
      iconUrl: item.icon,
    };
  }

  private getSourcePrefixFromModuleName(moduleName: CoreAppModulesEnum) {
    switch (moduleName) {
      case CoreAppModulesEnum.erpUI:
        return 'erp/assets';

      case CoreAppModulesEnum.scaleUI:
        return 'scale/assets';

      case CoreAppModulesEnum.tdmUI:
        return 'tdm/assets';

      case CoreAppModulesEnum.immUI:
        return 'imm/assets';

      default:
        return '';
    }
  }

  private trimStart(prefix: string): string {
    if (prefix?.charAt(0) === '/') {
      return prefix.substr(1);
    }
    return prefix;
  }
}
