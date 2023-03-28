import {
    AfterViewInit,
    Component,
    ElementRef,
    EventEmitter,
    forwardRef,
    Input,
    OnChanges,
    OnDestroy,
    OnInit,
    Optional,
    Output,
    Renderer2,
    SimpleChanges,
    TemplateRef,
    ViewChild,
    ViewEncapsulation
} from '@angular/core';
import { ControlContainer, NG_VALUE_ACCESSOR } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { FormControlDisplay } from '@coremodels/forms/form-control-display.enum';
import { NgSelectComponent } from '@ng-select/ng-select';
import { AmcsFormControlBaseComponent } from '@shared-module/forms/amcs-form-control-base';
import { GlossaryPipe } from '@shared-module/pipes/glossary.pipe';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Subject, Subscription } from 'rxjs';
import { AmcsSelectHelper } from '../helpers/amcs-select-helper';

/**
 * @todo Consolidate with amcs-multi-select-with-search
 */
@Component({
    selector: 'app-amcs-select-with-search',
    templateUrl: './amcs-select-with-search.component.html',
    styleUrls: ['./amcs-select-with-search.component.scss'],
    providers: [ GlossaryPipe,
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => AmcsSelectWithSearchComponent),
            multi: true
        }
    ],
    encapsulation: ViewEncapsulation.None
})
export class AmcsSelectWithSearchComponent extends AmcsFormControlBaseComponent implements OnInit, AfterViewInit, OnDestroy, OnChanges {
    edited = false;
    selectTranslation: string;
    notFoundText: string;
    value: any;
    FormControlDisplay = FormControlDisplay;
    isInitialCheck = true;

    @Input() items: any[];
    @Input() loading = false;
    @Input() bindLabel: string;
    @Input() bindValue: string = null;
    @Input() groupBy: string = null;
    @Input() isDisabled = false;
    @Input() customClass: string;
    @Input() isOptional = true;
    @Input() keepOriginalOrder = false;
    @Input() autoFocus = false;
    @Input() appendToBody = true;
    @Input() appendTo = 'body';
    @Input() dropdownPosition = 'auto';
    @Input() displayMode: FormControlDisplay = FormControlDisplay.Standard;
    @Input() isSecondaryUi = false;
    @ViewChild('select') selectElement: NgSelectComponent;
    // eslint-disable-next-line @angular-eslint/no-output-native
    @Output('change') change = new EventEmitter<any>();
    @Input('rowTemplate') rowTemplate: TemplateRef<any>;
    @Input() previewChanges: Subject<any>;
    @Input() hasWarning = false;

    constructor(
        @Optional() readonly controlContainer: ControlContainer,
        protected readonly elRef: ElementRef,
        protected readonly renderer: Renderer2,
        private readonly appTranslationsService: SharedTranslationsService,
        private readonly glossaryPipe: GlossaryPipe
        ) {
        super(controlContainer, elRef, renderer);
    }

    private translationSubscription: Subscription;

    onTouchedCallback: () => void = () => { };
    onChangeCallback: (_: any) => void = (_: any) => { };

    ngOnInit() {
        this.translationSubscription = this.appTranslationsService.translations.subscribe((translations: string[]) => {
            this.selectTranslation = translations['searchselect.select'];
            this.notFoundText = translations['selectTypeahead.noResultsFound'];
        });
    }

    ngOnChanges(change: SimpleChanges) {
        if (change && change['items'] && !this.keepOriginalOrder) {
            AmcsSelectHelper.doSort(this.items);
            this.checkSelectionValid();
        }
    }

    ngAfterViewInit() {
        if (this.selectElement.classes === null) {
            this.selectElement.classes = 'custom';
        }
        if (this.customClass != null) {
            this.selectElement.classes = this.selectElement.classes + ' ' + this.customClass;
        }
        if (this.autoFocus && isTruthy(this.selectElement)) {
            this.selectElement.focus();
        }
    }

    ngOnDestroy() {
        this.translationSubscription.unsubscribe();
        super.ngOnDestroy();
    }

    writeValueFromView(value: any) {
        if (isTruthy(this.previewChanges)) {
            this.previewChanges.next(value);
        } else {
            this.writeValue(value);
        }
    }

    /** ControlValueAccessor interface */
    writeValue(value: any) {
        this.value = value;
        if (this.value === null) {
            this.writeNullValue();
        } else {
            this.edited = true;
            this.onChangeCallback(this.value);
        }
        if (this.isInitialCheck) {
            // Only the first time, do this to validate on initialization
            this.isInitialCheck = false;
            this.checkSelectionValid();
        }
    }

    checkSelectionValid() {
        if (
            this.value !== null &&
            this.value !== undefined &&
            this.items !== null &&
            this.items !== undefined &&
            !AmcsSelectHelper.areOptionsGrouped(this.items)
        ) {
            let idProperty: string = this.bindValue;
            if (!isTruthy(idProperty)) {
                idProperty = 'id';
            }
            if (!this.items.some((x) => x[idProperty] === this.value)) {
                // This might be affecting the form it's linked to so put onto different thread
                setTimeout(() => {
                    this.writeValue(null);
                }, 0);
            }
        }
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

    onChange(e: any) {
        this.change.emit(e);
    }

    searchFn(term: string, item: any): boolean {
        term = term.toLowerCase();
        const label = this.glossaryPipe.transform(item[this.bindLabel]);
        if(label) {
            return label.toLowerCase().includes(term);
        } else {
            return false;
        }
    }

    /** End ControlValueAccessor interface */

    private writeNullValue() {
        this.edited = false;
        this.onChangeCallback(null);
    }
}
