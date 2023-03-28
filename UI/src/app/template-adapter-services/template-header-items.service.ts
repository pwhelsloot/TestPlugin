import { Injectable } from '@angular/core';
import { AmcsMenuItem } from '@shared-module/models/amcs-menu-item.model';
import { HeaderItemsServiceAdapter } from '@core-module/services/header/header-items.service.abstract';
import { AuthorisationService } from '@auth-module/services/authorisation.service';
import { ApplicationConfiguration } from '@core-module/models/application-configuration.model';
import { TemplateHeaderItemKeys } from './template-header-item-keys.model';

@Injectable()
export class TemplateHeaderItemsService extends HeaderItemsServiceAdapter {
  // To place items on the menu you would add them here:
  protected populateItems(
    translations: string[],
    config: ApplicationConfiguration,
    authorisationService: AuthorisationService
  ): AmcsMenuItem[] {
    const menuItems: AmcsMenuItem[] = [];
    menuItems.push(
      new AmcsMenuItem(TemplateHeaderItemKeys.template, 'Template', 'assets/icons/training.png', 'assets/icons/training-small.png')
    );
    return menuItems;
  }
}
