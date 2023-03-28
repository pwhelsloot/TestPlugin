import { AfterContentInit, AfterViewInit, Component, ElementRef, forwardRef, Input, OnDestroy, Renderer2, ViewChild } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { GlossaryService } from '@core-module/services/glossary/glossary.service';
import { AmcsSelectHelper } from '@shared-module/components/helpers/amcs-select-helper';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Subscription } from 'rxjs';

/**
 * @deprecated Use app-amcs-select-definition instead
 */
@Component({
    selector: 'app-amcs-select-definition-deprecated',
    templateUrl: './amcs-select-definition.component.html',
    styleUrls: ['./amcs-select-definition.component.scss'],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => AmcsSelectDefinitionDeprecatedComponent),
            multi: true
        }
    ]
})
export class AmcsSelectDefinitionDeprecatedComponent extends AutomationLocatorDirective implements AfterContentInit, OnDestroy, ControlValueAccessor, AfterViewInit {
    edited = false;
    value: any;
    selectTranslation: string;

    @Input('isDisabled') isDisabled = false;
    @Input('isSortable') isSortable = true;
    @Input('isMandatory') isMandatory = false;
    @Input('isEditable') isEditable = false;
    @Input('isVisible') isVisible = false;
    @Input('showVisible') showVisible = true;
    @Input('hasError') hasError = false;
    @Input('isOptional') isOptional: boolean;
    @Input('selectTooltip') selectTooltip: string;
    @Input('isNumber') isNumber = false;
    @ViewChild('select') selectElement: ElementRef;

    constructor(
        protected elRef: ElementRef, protected renderer: Renderer2,
        private readonly appTranslationsService: SharedTranslationsService, private readonly glossaryService: GlossaryService) {
        super(elRef, renderer);
    }

    private translationSubscription: Subscription;

    onTouchedCallback: () => void = () => { };
    onChangeCallback: (_: any) => void = (_: any) => { };

    ngOnDestroy() {
        this.translationSubscription.unsubscribe();
    }

    ngAfterViewInit() {
        if (this.isSortable) {
            AmcsSelectHelper.doDeprecatedSort(this.selectElement.nativeElement);
        }
        setTimeout(() => {
            this.translationSubscription = this.appTranslationsService.translations.subscribe((translations: string[]) => {
                this.selectTranslation = translations['searchselect.select'];
            });
        }, 0);
    }

    ngAfterContentInit() {
        AmcsSelectHelper.applyGlossaryTranslations(this.elRef, this.glossaryService);
    }

    /** ControlValueAccessor interface */
    writeValue(value: any) {
        this.value = value;
        if (this.value === null) {
            this.writeNullValue();
        } else {
            this.edited = true;
            if (this.isNumber) {
                this.onChangeCallback(+this.value);
            } else {
                this.onChangeCallback(this.value);
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
