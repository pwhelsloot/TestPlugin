import { Observable } from 'rxjs';
import { AmcsMenuItem } from '@shared-module/models/amcs-menu-item.model';

export interface IHeaderItemsService {

    items$: Observable<AmcsMenuItem[]>;
}
