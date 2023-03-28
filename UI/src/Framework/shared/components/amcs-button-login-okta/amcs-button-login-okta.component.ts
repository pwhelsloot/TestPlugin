import { Component, Input } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

/**
 * @deprecated Limited for use in IdentityServer and CoreLogin
 */
@Component({
    selector: 'app-amcs-button-login-okta',
    templateUrl: './amcs-button-login-okta.component.html',
    styleUrls: ['./amcs-button-login-okta.component.scss']
})
export class AmcsButtonLoginOktaComponent extends AutomationLocatorDirective {
    @Input() isDisplayLeftMode = false;
}
