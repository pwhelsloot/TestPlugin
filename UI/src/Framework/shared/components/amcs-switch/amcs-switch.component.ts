import { Component, forwardRef, Input, OnChanges, OnInit, SimpleChanges, ViewEncapsulation } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { AmcsSwitchConfig } from '@shared-module/components/amcs-switch/amcs-switch-config.model';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

@Component({
  selector: 'app-amcs-switch',
  templateUrl: './amcs-switch.component.html',
  styleUrls: ['./amcs-switch.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsSwitchComponent),
      multi: true
    }
  ]
})

export class AmcsSwitchComponent extends AutomationLocatorDirective implements OnInit, OnChanges, ControlValueAccessor {
  value = false;

  @Input('config') config: AmcsSwitchConfig;
  @Input() disabled = false;

  onTouchedCallback: () => void = () => { };
  onChangeCallback: (_: any) => void = (_: any) => { };

  ngOnInit() {
    this.refreshConfig();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['config']) {
      this.refreshConfig();
    }
  }

  /** ControlValueAccessor interface */
  writeValue(value: boolean) {
    this.value = value;
    if (this.value === null) {
      this.writeNullValue();
    } else {
      this.onChangeCallback(this.value);
    }
  }

  registerOnChange(fn: any) {
    this.onChangeCallback = fn;
  }

  registerOnTouched(fn: any) {
    this.onTouchedCallback = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  /** End ControlValueAccessor interface */

  private refreshConfig() {
    if (this.config == null) {
      this.config = new AmcsSwitchConfig();
    }
    // Sets some defaults but any config properties will take precedence
    this.config = {
      onText: 'On',
      offText: 'Off',
      onColor: 'primary',
      offColor: 'default',
      size: 'small',
      isDisabled: false,
      isInverse: false,
      ...this.config
    };
  }

  private writeNullValue() {
    this.onChangeCallback(null);
  }
}
