import { Component, Input } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

/**
 * @deprecated Limited for use in IdentityServer and CoreLogin
 */
@Component({
    selector: 'app-amcs-button-login-azure',
    templateUrl: './amcs-button-login-azure.component.html',
    styleUrls: ['./amcs-button-login-azure.component.scss']
})
export class AmcsButtonLoginAzureComponent extends AutomationLocatorDirective {
    @Input() isDisplayLeftMode = false;
}
