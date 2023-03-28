import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, Renderer2, ViewChild } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { CountCollectionModel } from '@core-module/models/api/count-collection.model';
import { FilterOperation } from '@core-module/models/api/filters/filter-operation.enum';
import { IFilter } from '@core-module/models/api/filters/iFilter';
import { DefaultAction } from '@core-module/models/default-action.model';
import { DefaultActionService } from '@core-module/services/lookup/default-action.service';
import { ComponentFilterProperty } from '@shared-module/components/amcs-component-filter/component-filter-property.model';
import { ComponentFilterTypeEnum } from '@shared-module/components/amcs-component-filter/component-filter-type.enum';
import { ComponentFilter } from '@shared-module/components/amcs-component-filter/component-filter.model';
import { GridColumnConfig } from '@shared-module/components/amcs-grid/grid-column-config';
import { AmcsModalChildComponent } from '@shared-module/components/amcs-modal/amcs-modal-child-component.interface';
import { AmcsSelectorGridComponent } from '@shared-module/components/amcs-selector-grid/amcs-selector-grid.component';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { ComponentFilterStorageService } from '@shared-module/services/amcs-component-filter/component-filter-storage.service';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { BehaviorSubject, Subject, Subscription } from 'rxjs';
import { withLatestFrom } from 'rxjs/operators';

/**
 * @deprecated Move to PlatformUI
 */
@Component({
    selector: 'app-default-action-details',
    templateUrl: './default-action-details.component.html',
    styleUrls: ['./default-action-details.component.scss'],
    providers: [DefaultActionService.providers, ComponentFilterStorageService]
})
export class DefaultActionDetailsComponent
    extends AutomationLocatorDirective
    implements OnInit, OnDestroy, AfterViewInit, AmcsModalChildComponent
{
    @ViewChild('selectorGrid') selectorGrid: AmcsSelectorGridComponent;

    hasFilterComponent = false;
    filterProperties: ComponentFilterProperty[];
    extraData: any;
    loading: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);
    externalClose: Subject<DefaultAction> = new Subject<DefaultAction>();
    selectedAction: DefaultAction;
    filters: IFilter[];

    defaultActions: CountCollectionModel<DefaultAction>;
    columns: GridColumnConfig[];

    currentPage: number;

    constructor(
        protected elRef: ElementRef,
        protected renderer: Renderer2,
        private actionService: DefaultActionService,
        private translationService: SharedTranslationsService
    ) {
        super(elRef, renderer);
    }

    private defaultActionSubscription: Subscription;

    ngOnInit() {
        this.selectedAction = this.extraData[0] as DefaultAction;
        this.filters = this.extraData[1] as IFilter[];
        this.compName = this.extraData[2];
        this.hasFilterComponent = this.extraData[3];

        this.actionService.requestDefaultActions(0, this.filters);
        this.defaultActionSubscription = this.actionService.defaultActions$
            .pipe(withLatestFrom(this.translationService.translations))
            .subscribe((data) => {
                const defaultActions = data[0];
                const translations = data[1];
                this.defaultActions = defaultActions;
                this.columns = DefaultAction.getGridColumns(translations);
                if (this.hasFilterComponent) {
                    this.filterProperties = DefaultAction.getFilterProperties(translations);
                }
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
        this.defaultActionSubscription.unsubscribe();
    }

    onPageChange(page: number) {
        this.currentPage = page;
        this.actionService.requestDefaultActions(page, this.filters);
    }

    rowSelected(action: DefaultAction[]) {
        this.externalClose.next(action[0]);
    }

    filterChanged(componentFilters: ComponentFilter[]) {
        this.filters = [];
        componentFilters.forEach((filter) => {
            let filterOperation: FilterOperation;
            switch (filter.filterTypeId) {
                case ComponentFilterTypeEnum.equal:
                    filterOperation = FilterOperation.Equal;
                    break;
                case ComponentFilterTypeEnum.notEqual:
                    filterOperation = FilterOperation.NotEqual;
                    break;
                case ComponentFilterTypeEnum.startsWith:
                    filterOperation = FilterOperation.StartsWith;
                    break;
                case ComponentFilterTypeEnum.endsWith:
                    filterOperation = FilterOperation.EndsWith;
                    break;
                case ComponentFilterTypeEnum.contains:
                    filterOperation = FilterOperation.Contains;
                    break;
            }
            this.filters.push({
                filterOperation,
                name: filter.propertyKey,
                value: filter.filterTextValue
            });
        });
        this.actionService.requestDefaultActions(this.currentPage, this.filters);
    }

    private setSelectedRow(selectedAction: DefaultAction) {
        let id: number;

        for (const action of this.defaultActions.results) {
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
