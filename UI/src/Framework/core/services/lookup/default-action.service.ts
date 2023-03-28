import { Injectable, OnDestroy } from '@angular/core';
import { CountCollectionModel } from '@core-module/models/api/count-collection.model';
import { IFilter } from '@core-module/models/api/filters/iFilter';
import { DefaultAction } from '@core-module/models/default-action.model';
import { ModalGridSelectorServiceAdapter } from '@core-module/services/forms/modal-grid-selector.adapter';
import { Observable, Subject } from 'rxjs';
import { switchMap, takeUntil } from 'rxjs/operators';
import { DefaultActionServiceData } from './default-action.service.data';

/**
 * @deprecated To be deleted
 */
@Injectable()
export class DefaultActionService extends ModalGridSelectorServiceAdapter implements OnDestroy {

    static providers = [DefaultActionService, DefaultActionServiceData];

    defaultActions$: Observable<CountCollectionModel<DefaultAction>>;

    // IModalAPIService implementation
    itemsObservable$: Observable<CountCollectionModel<any>>;
    unsubscribe = new Subject();

    constructor(private dataService: DefaultActionServiceData) {
        super();
        this.setupDefaultActionStream();
    }

    private defaultActionsRequest: Subject<[number, IFilter[]]> = new Subject<[number, IFilter[]]>();
    private defaultActionsSubject: Subject<CountCollectionModel<DefaultAction>> = new Subject<CountCollectionModel<DefaultAction>>();

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }
    requestItems(page: number) {
        this.defaultActionsRequest.next([page, null]);
    }

    requestDefaultActions(page: number, filters: IFilter[]) {
        this.defaultActionsRequest.next([page, filters]);
    }

    private setupDefaultActionStream() {
        this.defaultActions$ = this.defaultActionsSubject.asObservable();
        this.itemsObservable$ = this.defaultActionsSubject.asObservable();
        this.defaultActionsRequest.pipe(
            takeUntil(this.unsubscribe),
            switchMap(data => {
                return this.dataService.getDefaultActions(data[0], data[1]);
            }))
            .subscribe((data: CountCollectionModel<DefaultAction>) => {
                this.defaultActionsSubject.next(data);
            });
    }
}
