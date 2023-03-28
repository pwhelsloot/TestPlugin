import { Component, ElementRef, Input, OnChanges, OnDestroy, OnInit, Renderer2, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { CoreAppRoutes } from '@core-module/config/routes/core-app-routes.constants';
import { UserCommunication } from '@coremodels/header/user-communication.model';
import { UserCommunicationService } from '@coreservices/header/user-communication.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { Subscription } from 'rxjs';

/**
 * @deprecated Move to PlatformUI
 */
@Component({
  selector: 'app-header-communications',
  templateUrl: './header-communications.component.html',
  styleUrls: ['./header-communications.component.scss']
})
export class HeaderCommunicationsComponent extends AutomationLocatorDirective implements OnInit, OnChanges, OnDestroy {

  @Input() opened: boolean;
  @Input() count: number;

  userCommunications: UserCommunication[];
  loading = false;

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    private userCommunicationService: UserCommunicationService,
    private router: Router) {
    super(elRef, renderer);
  }

  private userCommunicationSubscription: Subscription;

  ngOnInit() {
    this.userCommunicationSubscription = this.userCommunicationService.communications$
      .subscribe(data => {
        this.userCommunications = data;
        this.loading = false;
      });
  }

  ngOnChanges(simpleChanges: SimpleChanges) {
    if (simpleChanges.opened && this.opened) {
      this.loading = true;
      this.userCommunicationService.requestCommunications();
    }
  }

  ngOnDestroy() {
    this.userCommunicationSubscription.unsubscribe();
  }

  refreshCommunicationCount() {
    this.userCommunicationService.refreshCommunicationCount();
  }

  onViewAllSelected() {
    this.refreshCommunicationCount();
    this.router.navigate([CoreAppRoutes.userModule + '/' + CoreAppRoutes.userCommunications], { queryParams: { isgroup: false} });
  }

  onRowSelected(data: UserCommunication) {
    this.refreshCommunicationCount();
    this.router.navigate([CoreAppRoutes.userModule + '/' + CoreAppRoutes.userCommunications + '/' + data.communicationId], { queryParams: { isgroup: data.isgroup} });
  }
}
