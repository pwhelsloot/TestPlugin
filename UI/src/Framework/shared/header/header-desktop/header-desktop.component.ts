import { Component, ElementRef, OnDestroy, OnInit, Renderer2, ViewEncapsulation } from '@angular/core';
import { HeaderComponentService } from '@core-module/services/header/header-component.service';
import { ApplicationConfigurationStore } from '@coreservices/config/application-configuration.store';
import { UserCommunicationService } from '@coreservices/header/user-communication.service';
import { UserNotificationService } from '@coreservices/header/user-notification.service';
import { AmcsModalService } from '@shared-module/components/amcs-modal/amcs-modal.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { Subscription } from 'rxjs';

/**
 * @deprecated Move to PlatformUI
 */
@Component({
  selector: 'app-header-desktop',
  templateUrl: './header-desktop.component.html',
  styleUrls: ['./header-desktop.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [AmcsModalService]
})
export class HeaderDesktopComponent extends AutomationLocatorDirective implements OnInit, OnDestroy {

  profileDataThumbnail: string;
  profileInitials: string;
  communicationCount = 0;
  notificationCount = 0;

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    public headerComponentService: HeaderComponentService,
    private applicationConfigurationStore: ApplicationConfigurationStore,
    private communicationInboxService: UserCommunicationService,
    private notificationInboxService: UserNotificationService) {
    super(elRef, renderer);
  }

  private communicationCountSubscription: Subscription;
  private notificationCountSubscription: Subscription;

  ngOnInit(): void {
    this.applicationConfigurationStore.profileThumbnail$.subscribe(data => {
      this.profileDataThumbnail = data[0] || '';
      this.profileInitials = data[1] || '';
    });

    this.communicationCountSubscription = this.communicationInboxService.communicationsCount$.subscribe(data => {
      this.communicationCount = data;
    });

    this.notificationCountSubscription = this.notificationInboxService.notificationsCount$.subscribe(data => {
      this.notificationCount = data;
    });

  }

  ngOnDestroy() {
    this.communicationCountSubscription.unsubscribe();
    this.notificationCountSubscription.unsubscribe();
  }

  close() {
    this.headerComponentService.isOpen = false;
  }

  closeCommunications() {
    this.headerComponentService.isCommunicationsOpen = false;
  }

  closeNotifications() {
    this.headerComponentService.isNotificationsOpen = false;
  }
}
