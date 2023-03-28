import { Component, Input } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

/**
 * @deprecated Limited for use in IdentityServer and CoreLogin
 */
@Component({
    selector: 'app-amcs-button-login-google',
    templateUrl: './amcs-button-login-google.component.html',
    styleUrls: ['./amcs-button-login-google.component.scss']
})
export class AmcsButtonLoginGoogleComponent extends AutomationLocatorDirective {
    @Input() isDisplayLeftMode = false;
}
