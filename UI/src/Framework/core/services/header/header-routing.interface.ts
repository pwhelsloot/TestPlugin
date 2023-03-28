import { EventEmitter } from '@angular/core';
import { AmcsMenuItem } from '@shared-module/models/amcs-menu-item.model';
import { AuthorisationService } from '@auth-module/services/authorisation.service';

export interface IHeaderRoutingService {

    // the active nodes name
    activeNodeName: EventEmitter<string>;

    // Navigates to a selected node
    navigateToItem(node: AmcsMenuItem, authorisationService: AuthorisationService): void;
}
