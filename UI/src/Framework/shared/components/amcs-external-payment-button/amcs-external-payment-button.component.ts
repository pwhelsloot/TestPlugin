import { Component, ElementRef, Input, Renderer2 } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

/**
 * @deprecated Move to PlatformUI
 */
@Component({
  selector: 'app-amcs-external-payment-button',
  templateUrl: './amcs-external-payment-button.component.html',
  styleUrls: ['./amcs-external-payment-button.component.scss']
})
export class AmcsExternalPaymentButtonComponent extends AutomationLocatorDirective {

  @Input() loading = false;
  @Input() disabled = false;
  @Input() isPayment = true;

  constructor(protected elRef: ElementRef, protected renderer: Renderer2) {
    super(elRef, renderer);
  }
}
