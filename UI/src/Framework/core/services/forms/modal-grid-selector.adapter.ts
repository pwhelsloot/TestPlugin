import { Observable} from 'rxjs';
import { Injectable } from '@angular/core';
import { IModalAPIService } from '@shared-module/models/modal-api-service.model';
import { CountCollectionModel } from '@core-module/models/api/count-collection.model';
import { BaseService } from '@coreservices/base.service';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { IFilter } from '@core-module/models/api/filters/iFilter';

@Injectable()
export abstract class ModalGridSelectorServiceAdapter extends BaseService implements IModalAPIService {

    abstract itemsObservable$: Observable<CountCollectionModel<ILookupItem>>;
    abstract requestItems(page: number, filters: IFilter[]);
}
