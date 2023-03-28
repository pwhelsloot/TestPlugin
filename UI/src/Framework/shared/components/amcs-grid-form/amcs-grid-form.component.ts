import { Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, Renderer2, TemplateRef } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { ResizeableColumnHelper } from '@core-module/helpers/resizeable-column.helper';
import { AmcsGridFormValidationService } from '@coreservices/forms/amcs-grid-form-validation.service';
import { AmcsGridFormService } from '@coreservices/forms/amcs-grid-form.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { AmcsDropdownIconEnum } from '../amcs-dropdown/amcs-dropdown-icon-enum.model';
import { AmcsGridFormColumn } from './amcs-grid-form-column';
import { AmcsGridFormConfig } from './amcs-grid-form-config.model';
import { AmcsGridFormRow } from './amcs-grid-form-row.model';

@Component({
    selector: 'app-amcs-grid-form',
    templateUrl: './amcs-grid-form.component.html',
    styleUrls: ['./amcs-grid-form.component.scss'],
    providers: [AmcsGridFormService]
})
export class AmcsGridFormComponent extends AutomationLocatorDirective implements OnInit, OnDestroy {
    loading: boolean;
    disabled: boolean;
    styles = { display: 'flex', width: '100%' };
    AmcsDropdownIconEnum = AmcsDropdownIconEnum;

    @Input() loading$: BehaviorSubject<boolean>;
    @Input() rows: AmcsGridFormRow[];
    @Input() formTemplate: TemplateRef<any>;
    @Input() columns: AmcsGridFormColumn[];
    @Input() config: AmcsGridFormConfig;
    @Input() createForm: (rowId: number) => AmcsFormGroup;
    @Input() settingsMenu: TemplateRef<any>;
    @Input() resizable = true;
    @Input() canEnableCheckboxSelectionCallback = false;

    @Output() onManualFocusError = new EventEmitter<any>();
    @Output() onCheckboxSelection = new EventEmitter<any>();

    resizeHelper: ResizeableColumnHelper = null;

    constructor(
        protected elRef: ElementRef, protected renderer: Renderer2,
        public service: AmcsGridFormService, private validationService: AmcsGridFormValidationService) {
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
                    this.service.canEnableCheckboxSelectionCallback = this.canEnableCheckboxSelectionCallback;
                    this.service.onManualFocusError = this.onManualFocusError;
                    this.service.onCheckboxSelection = this.onCheckboxSelection;
                    this.service.setColumnSizes(this.columns);
                    this.resizeHelper = new ResizeableColumnHelper(this.columns);
                    this.service.setDisabledState();
                    this.validationService.validForms$ = Observable.create((function(observer) {
                        observer.next(this.service.getForms());
                    }).bind(this));
                    this.validationService.readySubject.next();
                }, 0);
            }
        });
    }

    ngOnDestroy() {
        this.loadingSubscription.unsubscribe();
        if (isTruthy(this.resizeHelper)) {
            this.resizeHelper.destroy();
        }
    }
}
