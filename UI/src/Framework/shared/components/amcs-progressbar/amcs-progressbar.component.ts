import { Component, Input } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

@Component({
  selector: 'app-amcs-progressbar',
  templateUrl: './amcs-progressbar.component.html',
  styleUrls: ['./amcs-progressbar.component.scss']
})
export class AmcsProgressbarComponent extends AutomationLocatorDirective {
  @Input('max') max: number; 4;
  @Input('type') type: string;
  @Input('value') value: number | any[];
  @Input('customClass') customClass = '';

}
