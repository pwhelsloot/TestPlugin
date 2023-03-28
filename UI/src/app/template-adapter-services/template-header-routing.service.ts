import { Injectable } from '@angular/core';
import { HeaderRoutingSeviceAdapter } from '@core-module/services/header/header-routing.service.abstract';
import { AmcsMenuItem } from '@shared-module/models/amcs-menu-item.model';
import { TemplateHeaderItemKeys } from './template-header-item-keys.model';
import { TemplateModuleAppRoutes } from '@app/template/template-module-app-routes.constants';

@Injectable()
export class TemplateHeaderRoutingService extends HeaderRoutingSeviceAdapter {
  // this controls the routes for the menu items
  navigateToItem(item: AmcsMenuItem) {
    switch (item.key) {
      case TemplateHeaderItemKeys.template:
        this.router.navigate([TemplateModuleAppRoutes.module, TemplateModuleAppRoutes.dashboard]);
        break;

      default:
        this.tryCrossAppNavigation(item.key);
        break;
    }
  }

  protected parseContextFromUrl(url: string[]): void {
    // to get context from the URL
  }

  protected getNodeNameFromFeatureName(featureName: string): string {
    switch (featureName) {
      default:
        return TemplateHeaderItemKeys.template;
    }
  }
}
