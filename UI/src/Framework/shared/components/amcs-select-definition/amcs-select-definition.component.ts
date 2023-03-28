import { AfterViewInit, Component, ElementRef, forwardRef, Input, OnChanges, OnDestroy, OnInit, Renderer2, SimpleChanges, ViewChild } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { ILookupItem } from '@coremodels/lookups/lookup-item.interface';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Subscription } from 'rxjs';
import { AmcsSelectHelper } from '../helpers/amcs-select-helper';

/**
 * @deprecated Marked to be moved to settings module in PlatformUI
 */
@Component({
    selector: 'app-amcs-select-definition',
    templateUrl: './amcs-select-definition.component.html',
    styleUrls: ['./amcs-select-definition.component.scss'],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => AmcsSelectDefinitionComponent),
            multi: true
        }
    ]
})
export class AmcsSelectDefinitionComponent extends AutomationLocatorDirective implements OnInit, AfterViewInit, OnDestroy, OnChanges {
    edited = false;
    selectTranslation: string;
    value: any;

    @Input('isDisabled') isDisabled: boolean;
    @Input('isSecondaryUi') isSecondaryUi = false;
    @Input('customClass') customClass: string;
    @Input('hasError') hasError = false;
    @Input('isMandatory') isMandatory = false;
    @Input('isEditable') isEditable = false;
    @Input('isVisible') isVisible = false;
    @Input('showVisible') showVisible = true;
    @Input('isOptional') isOptional: boolean;
    @Input('options') options: ILookupItem[];
    @Input('optionBindValue') optionBindValue = 'id';
    @Input('optionBindDescription') optionBindDescription = 'description';
    @Input('selectTooltip') selectTooltip: string;
    @ViewChild('select') selectElement: ElementRef;

    constructor(protected elRef: ElementRef, protected renderer: Renderer2,
        private appTranslationsService: SharedTranslationsService) {
        super(elRef, renderer);
    }

    private translationSubscription: Subscription;

    onTouchedCallback: () => void = () => { };
    onChangeCallback: (_: any) => void = (_: any) => { };

    ngOnInit() {
        this.translationSubscription = this.appTranslationsService.translations.subscribe((translations: string[]) => {
            this.selectTranslation = translations['searchselect.select'];
        });
    }

    ngOnChanges(change: SimpleChanges) {
        if (change && change['options']) {
            AmcsSelectHelper.doSort(this.options);
            this.checkSelectionValid();
        }
    }

    ngAfterViewInit() {
        if (this.customClass != null) {
            const classes = this.customClass.split(' ');
            classes.forEach(element => {
                this.renderer.addClass(this.selectElement.nativeElement, element);
            });
        }
    }

    ngOnDestroy() {
        this.translationSubscription.unsubscribe();
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
    }

    checkSelectionValid() {
        if (this.value !== null && this.value !== undefined && this.options !== null && this.options !== undefined) {
            if (!this.options.some(x => x.id === this.value)) {
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

    /** End ControlValueAccessor interface */

    private writeNullValue() {
        this.edited = false;
        this.onChangeCallback(null);
    }
}
