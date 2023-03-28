import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, Renderer2, ViewChild } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { CountCollectionModel } from '@core-module/models/api/count-collection.model';
import { IFilter } from '@core-module/models/api/filters/iFilter';
import { CostTemplateSelector } from '@core-module/models/cost-template-selector.model';
import { CostTemplateSelectorService } from '@core-module/services/lookup/cost-template-selector.service';
import { GridColumnConfig } from '@shared-module/components/amcs-grid/grid-column-config';
import { AmcsModalChildComponent } from '@shared-module/components/amcs-modal/amcs-modal-child-component.interface';
import { AmcsSelectorGridComponent } from '@shared-module/components/amcs-selector-grid/amcs-selector-grid.component';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { BehaviorSubject, Subject, Subscription } from 'rxjs';
import { withLatestFrom } from 'rxjs/operators';

@Component({
    selector: 'app-cost-template-details',
    templateUrl: './cost-template-details.component.html',
    styleUrls: ['./cost-template-details.component.scss'],
    providers: [CostTemplateSelectorService.providers]
})
export class CostTemplateDetailsComponent
    extends AutomationLocatorDirective
    implements OnInit, OnDestroy, AfterViewInit, AmcsModalChildComponent
{
    @ViewChild('selectorGrid') selectorGrid: AmcsSelectorGridComponent;

    extraData: any;
    loading: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);
    externalClose: Subject<CostTemplateSelector> = new Subject<CostTemplateSelector>();
    selectedAction: CostTemplateSelector;
    filters: IFilter[];

    costTemplates: CountCollectionModel<CostTemplateSelector>;
    columns: GridColumnConfig[];

    constructor(
        protected elRef: ElementRef,
        protected renderer: Renderer2,
        private actionService: CostTemplateSelectorService,
        private translationService: SharedTranslationsService
    ) {
        super(elRef, renderer);
    }

    private costTemplateSubscription: Subscription;

    ngOnInit() {
        this.selectedAction = this.extraData[0] as CostTemplateSelector;
        this.filters = this.extraData[1] as IFilter[];
        this.compName = this.extraData[2];
        this.formControlName = this.extraData[3];

        this.actionService.requestCostTemplates(0, this.filters);
        this.costTemplateSubscription = this.actionService.costTemplates$
            .pipe(withLatestFrom(this.translationService.translations))
            .subscribe((data) => {
                const costTemplates = data[0];
                const translations = data[1];
                this.costTemplates = costTemplates;
                this.columns = CostTemplateSelector.getGridColumns(translations);
                this.loading.next(false);
                setTimeout(() => {
                    if (isTruthy(this.selectorGrid) && isTruthy(this.selectedAction)) {
                        this.setSelectedRow(this.selectedAction);
                    }
                }, 100);
            });
    }

    ngAfterViewInit() {
        setTimeout(() => {
            if (isTruthy(this.selectorGrid) && isTruthy(this.selectedAction)) {
                this.setSelectedRow(this.selectedAction);
            }
        }, 100);
    }

    ngOnDestroy() {
        this.costTemplateSubscription.unsubscribe();
    }

    onPageChange(page: number) {
        this.actionService.requestCostTemplates(page, this.filters);
    }

    rowSelected(action: CostTemplateSelector[]) {
        this.externalClose.next(action[0]);
    }

    private setSelectedRow(selectedAction: CostTemplateSelector) {
        let id: number;

        for (const action of this.costTemplates.results) {
            if (
                action.actionId === selectedAction.actionId &&
                action.materialClassId === selectedAction.materialClassId &&
                action.pricingBasisId === selectedAction.pricingBasisId &&
                action.serviceId === selectedAction.serviceId &&
                action.taxTemplateCollectionId === selectedAction.taxTemplateCollectionId &&
                action.vatId === selectedAction.vatId
            ) {
                id = action.id;
                break;
            }
        }

        if (isTruthy(id)) {
            this.selectorGrid.selectRowWithId(id);
        }
    }
}
