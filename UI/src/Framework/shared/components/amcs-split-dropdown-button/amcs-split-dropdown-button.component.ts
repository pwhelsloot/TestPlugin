import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { AmcsSplitDropdownButtonConfig } from './amcs-split-dropdown-button-config.model';

/**
 * @deprecated Will be moved to Scale
 */
@Component({
  selector: 'app-amcs-split-dropdown-button',
  templateUrl: './amcs-split-dropdown-button.component.html',
  styleUrls: ['./amcs-split-dropdown-button.component.scss']
})
export class AmcsSplitDropdownButtonComponent extends AutomationLocatorDirective  implements OnInit {

  @Input('config') config = new AmcsSplitDropdownButtonConfig();
  @Input('customClass') customClass: string;
  @Input('buttonColourClass') buttonColourClass: string;
  @Input('disabled') disabled: boolean;
  @Input('loading') loading = false;

  @Output('optionSelected') optionSelected: EventEmitter<number> = new EventEmitter<number>();

  ngOnInit(): void {
  }

}
