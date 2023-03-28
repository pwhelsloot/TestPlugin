import { Injectable } from '@angular/core';
import { CountCollectionModel } from '@core-module/models/api/count-collection.model';
import { IFilter } from '@core-module/models/api/filters/iFilter';
import { CostTemplateSelector } from '@core-module/models/cost-template-selector.model';
import { Observable, Subject } from 'rxjs';
import { switchMap, takeUntil } from 'rxjs/operators';
import { BaseService } from '../base.service';
import { CostTemplateSelectorServiceData } from './cost-template-selector.service.data';

/**
 * @deprecated To be deleted
 */
@Injectable()
export class CostTemplateSelectorService extends BaseService {

    static providers = [CostTemplateSelectorService, CostTemplateSelectorServiceData];

    costTemplates$: Observable<CountCollectionModel<CostTemplateSelector>>;

    constructor(private dataService: CostTemplateSelectorServiceData) {
        super();
        this.setupCostTemplateStream();
    }

    private costTemplateRequest: Subject<[number, IFilter[]]> = new Subject<[number, IFilter[]]>();
    private costeTemplateSubject: Subject<CountCollectionModel<CostTemplateSelector>> = new Subject<CountCollectionModel<CostTemplateSelector>>();

    requestCostTemplates(page: number, filters: IFilter[]) {
        this.costTemplateRequest.next([page, filters]);
    }

    private setupCostTemplateStream() {
        this.costTemplates$ = this.costeTemplateSubject.asObservable();
        this.costTemplateRequest.pipe(
            takeUntil(this.unsubscribe),
            switchMap(data => {
                return this.dataService.getCostTemplatess(data[0], data[1]);
            }))
            .subscribe((data: CountCollectionModel<CostTemplateSelector>) => {
                this.costeTemplateSubject.next(data);
            });
    }
}
