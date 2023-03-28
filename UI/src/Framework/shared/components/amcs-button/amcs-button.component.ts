import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { ButtonDisplayModeEnum } from '@shared-module/components/amcs-button/button-display-mode.enum';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

@Component({
  selector: 'app-amcs-button',
  templateUrl: './amcs-button.component.html',
  styleUrls: ['./amcs-button.component.scss']
})
export class AmcsButtonComponent extends AutomationLocatorDirective implements OnInit {
  @Input('type') type: string;
  @Input('customClass') customClass: string;
  @Input('buttonTooltip') buttonTooltip: string;
  @Input('buttonTooltipPlacement') buttonTooltipPlacement = 'auto';
  @Input('disabled') disabled: boolean;
  @Input('loading') loading = false;
  @Input() minWidth: number = null;
  @Input() displayMode: ButtonDisplayModeEnum = ButtonDisplayModeEnum.Standard;
  @ViewChild('btn') button: ElementRef;
  ButtonDisplayModeEnum = ButtonDisplayModeEnum;

  ngOnInit() {
    if (this.type == null) {
      this.type = 'button';
    }
  }
  onClick(event) {
    if (this.disabled || this.loading) {
    //if the button is disabled, stops default mouse events of the component's children and wrappers, otherwise i.e. button text would be clickable
      event.preventDefault();
      event.stopPropagation();
    }
  }
}
