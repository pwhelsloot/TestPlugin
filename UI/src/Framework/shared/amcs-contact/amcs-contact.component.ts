import { Component, ElementRef, Input, Renderer2 } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CoreAppRoutes } from '@core-module/config/routes/core-app-routes.constants';
import { CustomerContact } from '@core-module/models/external-dependencies/customer-contact.model';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

/**
 * @deprecated Move to PlatformUI
 */
@Component({
  selector: 'app-amcs-contact',
  templateUrl: './amcs-contact.component.html',
  styleUrls: ['./amcs-contact.component.scss']
})
export class AmcsContactComponent extends AutomationLocatorDirective {

  @Input() contact: CustomerContact;

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    private router: Router,
    private route: ActivatedRoute) {
    super(elRef, renderer);
    this.contact = new CustomerContact();
  }

  openContact(contact) {
    this.router.navigate(['../' + CoreAppRoutes.contact + '/' + contact.contactId], { relativeTo: this.route });
  }
}
