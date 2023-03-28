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
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { FormControlDisplay } from '@coremodels/forms/form-control-display.enum';
import { NgSelectComponent } from '@ng-select/ng-select';
import { AmcsFormControlBaseComponent } from '@shared-module/forms/amcs-form-control-base';
import { GlossaryPipe } from '@shared-module/pipes/glossary.pipe';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Subscription } from 'rxjs';
import { isTruthy } from '../../../core/helpers/is-truthy.function';
import { AmcsSelectHelper } from '../helpers/amcs-select-helper';

/**
 * @todo Move Multi select functionality to amcs-select-with-search and deprecate this one
 */
@Component({
    selector: 'app-amcs-multi-select-with-search',
    templateUrl: './amcs-multi-select-with-search.component.html',
    styleUrls: ['./amcs-multi-select-with-search.component.scss'],
    providers: [ GlossaryPipe,
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => AmcsMultiSelectWithSearchComponent),
            multi: true
        }
    ],
    encapsulation: ViewEncapsulation.None
})
export class AmcsMultiSelectWithSearchComponent extends AmcsFormControlBaseComponent implements OnInit, AfterViewInit, OnDestroy, OnChanges {
    edited = false;
    selectTranslation: string;
    notFoundText: string;
    value: any;
    FormControlDisplay = FormControlDisplay;

    @Input() items: any[];
    @Input() bindLabel: string;
    @Input() bindValue: string = null;
    @Input() groupBy: string = null;
    @Input() displayMode: FormControlDisplay = FormControlDisplay.Standard;
    @Input() isSecondaryUi = false;
    @Input() loading = false;
    @Input() isDisabled = false;
    @Input() closeOnSelect = false;
    @Input() enableSelectAll = false;
    @Input() customClass: string;
    @Input() isOptional = true;
    @Input() keepOriginalOrder = false;
    @Input() autoFocus = false;
    @Input() appendToBody = true;
    @Input() appendTo = 'body';
    @ViewChild('select') selectElement: NgSelectComponent;
    @Input('options') options: ILookupItem[];
    // eslint-disable-next-line @angular-eslint/no-output-native
    @Output() change = new EventEmitter<any>();
    @Input() dropdownPosition = 'auto';
    @Input() hideSelectionsWhenSelectAll = false;
    @Input() rowTemplate: TemplateRef<any>;
    @Input() isCheckbox = true;

    @Input() toolTipPlacement = 'bottom';

    @Input() csvTextLabel = false;

    allItemsSelected = false;

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

    onTouchedCallback: () => void = () => {};
    onChangeCallback: (_: any) => void = (_: any) => {
        this.change.emit(_);
    };

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
        if (this.customClass != null) {
            const classes = this.customClass.split(' ');
            classes.forEach((element) => {
                this.renderer.addClass(this.selectElement, element);
            });
        }
    }

    ngOnDestroy() {
        this.translationSubscription.unsubscribe();
        super.ngOnDestroy();
    }

    selectAll() {
        this.writeValue(this.items.map((x) => x[this.bindValue]));
    }

    unselectAll() {
        this.writeValue([]);
    }

    writeValueFromView(value: any) {
        this.writeValue(value);
    }

    /** ControlValueAccessor interface */
    writeValue(value: any) {
        this.value = value;
        if (this.value === null) {
            this.writeNullValue();
        } else {
            this.edited = true;
            this.allItemsSelected = this.items.length === this.value.length;
            this.onChangeCallback(this.value);
        }
    }

    checkSelectionValid() {
        if (isTruthy(this.value) && this.value !== NaN && isTruthy(this.items) && !AmcsSelectHelper.areOptionsGrouped(this.items)) {
            if (this.value.some((x) => !this.items.some((y) => y.id === x))) {
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

    groupCompare(a: any, b: any): boolean {
        // If it's the group option (select all) and all is selected then return true
        if (!isTruthy(a[this.bindValue]) && this.allItemsSelected) {
            return true;
            // If it's not the group option and all are selected then return false (we don't want a tons of selected chips showing if all items selected)
        } else if (this.allItemsSelected) {
            return false;
        }
        // else 'all' are not selected so fallback to normal
        return this.defaultCompare(a, b);
    }

    defaultCompare(a: any, b: any): boolean {
        return a[this.bindValue] === b;
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
        this.allItemsSelected = false;
        this.onChangeCallback(null);
    }
}
