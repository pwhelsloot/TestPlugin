import {
    AfterViewInit,
    Component,
    ElementRef,
    EventEmitter,
    forwardRef,
    Input,
    OnInit,
    Output,
    Renderer2,
    ViewChild,
    ViewEncapsulation
} from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { IFilter } from '@core-module/models/api/filters/iFilter';
import { CostTemplateSelector } from '@core-module/models/cost-template-selector.model';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { take } from 'rxjs/operators';
import { AmcsModalService } from '../amcs-modal/amcs-modal.service';
import { CostTemplateDetailsComponent } from './cost-template-details/cost-template-details.component';

/**
 * @deprecated Use the AmcsModalSelectorComponent instead
 */
@Component({
    selector: 'app-amcs-cost-template-selector',
    templateUrl: './amcs-cost-template-selector.component.html',
    styleUrls: ['./amcs-cost-template-selector.component.scss'],
    encapsulation: ViewEncapsulation.None,
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => AmcsCostTemplateSelectorComponent),
            multi: true
        }
    ]
})
export class AmcsCostTemplateSelectorComponent extends AutomationLocatorDirective implements OnInit, AfterViewInit, ControlValueAccessor {
    edited: boolean;
    FormControlDisplay = FormControlDisplay;

    @Output('costTemplateSelected') costTemplateSelected = new EventEmitter<CostTemplateSelector | boolean>();

    @Input('selectedAction') selectedAction: CostTemplateSelector;
    @Input('customClass') customClass: string;
    @Input('customWrapperClass') customWrapperClass: string;
    @Input('isDisabled') isDisabled: boolean;
    @Input('hasError') hasError: boolean;
    @Input('placeholder') placeholder: string;
    @Input('description') description = '';
    @Input() displayMode: FormControlDisplay = FormControlDisplay.Standard;
    @Input('filters') filters: IFilter[];

    @ViewChild('defaultInput') defaultInput: ElementRef;
    @ViewChild('wrapper') wrapperElement: ElementRef;

    isValueSet = false;

    onTouchedCallback: () => void;
    onChangeCallback: (_: any) => void;

    constructor(
        protected elRef: ElementRef,
        protected renderer: Renderer2,
        private modalService: AmcsModalService,
        private translateService: SharedTranslationsService
    ) {
        super(elRef, renderer);
    }

    ngOnInit() {
        this.edited = false;

        if (this.hasError == null) {
            this.hasError = false;
        }

        if (this.onTouchedCallback == null) {
            this.onTouchedCallback = () => {};
        }

        if (this.onChangeCallback == null) {
            this.onChangeCallback = (_: any) => {};
        }
    }

    ngAfterViewInit() {
        if (this.customClass != null) {
            const classes = this.customClass.split(' ');
            classes.forEach((element) => {
                this.renderer.addClass(this.defaultInput.nativeElement, element);
            });
        }
        if (this.customWrapperClass != null) {
            const classes = this.customWrapperClass.split(' ');
            classes.forEach((element) => {
                this.renderer.addClass(this.wrapperElement.nativeElement, element);
            });
        }
    }

    onFocus() {
        this.translateService.translations.pipe(take(1)).subscribe((translations) => {
            this.modalService
                .createModal({
                    type: 'confirmation',
                    title: translations['costTemplateSelector.title'],
                    template: CostTemplateDetailsComponent,
                    largeSize: true,
                    hideButtons: true,
                    extraData: [this.selectedAction, this.filters, this.compName, this.formControlName]
                })
                .afterClosed()
                .pipe(take(1))
                .subscribe((data: CostTemplateSelector | boolean) => {
                    if (isTruthy(data) && data !== false) {
                        this.onActionSelected(data as CostTemplateSelector);
                    }
                });
        });
    }

    onActionSelected(action: CostTemplateSelector) {
        if (isTruthy(action)) {
            this.description = `${action.service} - ${action.action}`;
            this.writeValue(action.serviceId);
        } else {
            this.writeValue(null);
        }
        this.costTemplateSelected.emit(action);
    }

    writeValue(val: number): void {
        this.onChangeCallback(val);
        this.isValueSet = isTruthy(val);
        if (!isTruthy(val)) {
            this.description = '';
        }
    }

    reset() {
        this.onActionSelected(null);
    }

    registerOnChange(fn: any) {
        this.onChangeCallback = fn;
    }

    registerOnTouched(fn: any) {
        this.onTouchedCallback = fn;
    }

    setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
    }
}
