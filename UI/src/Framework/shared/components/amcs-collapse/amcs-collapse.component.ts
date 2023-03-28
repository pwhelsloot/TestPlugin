import { Component, Input } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

/**
 * @deprecated Marked for removal,
 * this has become obsolete because individual components have the function built in
 */
@Component({
  selector: 'app-amcs-collapse',
  templateUrl: './amcs-collapse.component.html',
  styleUrls: ['./amcs-collapse.component.scss']
})
export class AmcsCollapseComponent extends AutomationLocatorDirective {
  @Input('isCollapsed') isCollapsed: boolean;
}
