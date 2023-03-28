import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

/**
 * @deprecated Marked to be moved to settings module in PlatformUI
 */
@Component({
    selector: 'app-amcs-input-definition',
    templateUrl: './amcs-input-definition.component.html',
    styleUrls: ['./amcs-input-definition.component.scss'],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => AmcsInputDefinitionComponent),
            multi: true
        }
    ]
})
export class AmcsInputDefinitionComponent extends AutomationLocatorDirective implements ControlValueAccessor {
    edited = false;
    value: any;

    @Input('type') type = 'text';
    @Input('isMandatory') isMandatory = false;
    @Input('step') step: string;
    @Input('isEditable') isEditable = false;
    @Input('isVisible') isVisible = false;
    @Input('showVisible') showVisible = true;
    @Input('isDisabled') isDisabled = false;
    @Input('isReadOnly') isReadOnly = false;
    @Input('hasError') hasError = false;
    @Input('autocomplete') autocomplete = 'on';
    @Input('autoFocus') autoFocus = false;
    @Input('placeholder') placeholder: string;
    @Input('maxLength') maxLength: number = null;
    @Input('errors') errors: string[] = [];
    @Input('min') min;
    @Input() showTruncateOnTooltip = false;

    showToolTip = false;

    onTouchedCallback: () => void = () => { };
    onChangeCallback: (_: any) => void = (_: any) => { };

    /** ControlValueAccessor interface */
    writeValue(value: any) {
        if (!isTruthy(value)) {
            this.writeNullValue();
        } else {
            this.edited = true;
            if (this.type === 'number') {
                value = Number(value);
            }
            this.value = value;
            this.onChangeCallback(this.value);
            this.onTouchedCallback();
        }
    }

    keyPressEvent(e: any) {
        const charCode = e.keyCode;
        if (this.type === 'number' && (charCode !== 46 && charCode > 31
            && (charCode < 48 || charCode > 57))) {
            return false;
        }
        if (isTruthy(this.min) && this.min >= 0 && e.keyCode === 45) {
            e.preventDefault();
            return true;
        }
        return true;
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
        this.value = null;
        this.onChangeCallback(null);
    }
}
