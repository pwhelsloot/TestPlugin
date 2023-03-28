import { Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, Renderer2, TemplateRef } from '@angular/core';
import { AmcsDataInputGridValidationService } from '@coreservices/forms/amcs-data-input-grid-validation.service';
import { AmcsDataInputGridService } from '@coreservices/forms/amcs-data-input-grid.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { AmcsDataInputGridColumn } from './amcs-data-input-grid-column';
import { AmcsDataInputGridColumnType } from './amcs-data-input-grid-column-type.enum';
import { AmcsDataInputGridConfig } from './amcs-data-input-grid-config.model';
import { AmcsDataInputGridRow } from './amcs-data-input-grid-row.model';

/**
 * @deprecated Use AmcsGridFormSimple instead
 */
@Component({
    selector: 'app-amcs-data-input-grid',
    templateUrl: './amcs-data-input-grid.component.html',
    styleUrls: ['./amcs-data-input-grid.component.scss'],
    providers: [AmcsDataInputGridService]
})
export class AmcsDataInputGridComponent extends AutomationLocatorDirective implements OnInit, OnDestroy {
    loading: boolean;
    disabled: boolean;
    styles = { display: 'flex', width: '100%' };

    @Input() loading$: BehaviorSubject<boolean>;
    @Input() rows: AmcsDataInputGridRow[];
    @Input() formTemplate: TemplateRef<any>;
    @Input() columns: AmcsDataInputGridColumn[];
    @Input() config: AmcsDataInputGridConfig;
    @Input() checkBoxSizePercent = '2';
    @Input() createForm: (rowId: number) => AmcsFormGroup;
    @Input('noDataTemplate') noDataTemplate: TemplateRef<any>;

    @Output() onRowSelected = new EventEmitter<AmcsDataInputGridRow>();
    @Output() onManualFocusError = new EventEmitter<any>();

    ColumnType = AmcsDataInputGridColumnType;

    constructor(
        protected elRef: ElementRef, protected renderer: Renderer2,
        public service: AmcsDataInputGridService, private validationService: AmcsDataInputGridValidationService) {
        super(elRef, renderer);
    }

    private loadingSubscription: Subscription;

    ngOnInit() {
        this.loadingSubscription = this.loading$.subscribe((loading: boolean) => {
            this.loading = loading;
            if (!this.loading) {
                // Need timeout here to ensure we get inputs
                setTimeout(() => {
                    this.service.rows = this.rows;
                    this.service.createForm = this.createForm;
                    this.service.config = this.config;
                    this.service.onManualFocusError = this.onManualFocusError;
                    if (this.columns) {
                        this.service.setColumnSizes(this.columns, this.checkBoxSizePercent);
                        this.service.setDisabledState();
                    }
                    this.validationService.validForms$ = Observable.create((function(observer) {
                        observer.next(this.service.getForms());
                    }).bind(this));
                    this.validationService.readySubject.next();
                }, 0);
            }
        });
    }

    onSelectRow(row, $event) {
        this.service.rowSelected(row, $event);
        this.onRowSelected.emit(row);
    }

    ngOnDestroy() {
        this.loadingSubscription.unsubscribe();
    }
}
