import { Component, Input } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

@Component({
    selector: 'app-amcs-highlight-icon',
    templateUrl: './amcs-highlight-icon.component.html',
    styleUrls: ['./amcs-highlight-icon.component.scss']
})
export class AmcsHighlightIconComponent extends AutomationLocatorDirective {
    @Input() iconClass: string;
    @Input() disabled: boolean;
}
