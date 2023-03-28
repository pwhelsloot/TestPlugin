import { Observable } from 'rxjs';
import { CountCollectionModel } from '@core-module/models/api/count-collection.model';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { IFilter } from '@core-module/models/api/filters/iFilter';

export interface IModalAPIService {
    itemsObservable$: Observable<CountCollectionModel<ILookupItem>>;
    requestItems(page: number, filters: IFilter[]);
}
