import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, ElementRef, OnDestroy, OnInit, Renderer2, ViewEncapsulation } from '@angular/core';
import { HeaderComponentService } from '@core-module/services/header/header-component.service';
import { InnerTileServiceUI } from '@core-module/services/ui/inner-tile.service.ui';
import { ApplicationConfigurationStore } from '@coreservices/config/application-configuration.store';
import { UserCommunicationService } from '@coreservices/header/user-communication.service';
import { UserNotificationService } from '@coreservices/header/user-notification.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { Subscription } from 'rxjs';

/**
 * @deprecated Move to PlatformUI
 */
@Component({
  selector: 'app-header-mobile',
  templateUrl: './header-mobile.component.html',
  styleUrls: ['./header-mobile.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: [
    trigger('expandCollapse', [
      state('true', style({
        top: '50px'
      })),
      state('false', style({
        top: '0px'
      })),
      transition('true <=> false', animate('400ms cubic-bezier(0.25, 0.8, 0.25, 1)'))
    ]),
  ]
})
export class HeaderMobileComponent extends AutomationLocatorDirective implements OnInit, OnDestroy {

  isSearchClicked: boolean;
  profileDataThumbnail: string;
  profileInitials: string;
  communicationCount = 0;
  notificationCount = 0;

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    public headerComponentService: HeaderComponentService,
    public tileUiService: InnerTileServiceUI,
    private applicationConfigurationStore: ApplicationConfigurationStore,
    private communicationInboxService: UserCommunicationService,
    private notificationInboxService: UserNotificationService) {
    super(elRef, renderer);
    this.isSearchedClickedSub = this.headerComponentService.headerService.isSearchedClicked.subscribe((value: boolean) => {
      this.isSearchClicked = value;
    });
  }

  private communicationCountSubscription: Subscription;
  private notificationCountSubscription: Subscription;

  private isSearchedClickedSub: Subscription;

  ngOnInit() {
    this.headerComponentService.globalSearchServiceUI.showGo.next(true);
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

  closeCommunications() {
    this.headerComponentService.isCommunicationsOpen = false;
  }

  close() {
    this.headerComponentService.isOpen = false;
  }

  ngOnDestroy() {
    this.isSearchedClickedSub.unsubscribe();
    this.headerComponentService.globalSearchServiceUI.showGo.next(false);
    this.headerComponentService.headerService.isSearchedClicked.next(false);
    this.communicationCountSubscription.unsubscribe();
    this.notificationCountSubscription.unsubscribe();
  }

  closeNotifications() {
    this.headerComponentService.isNotificationsOpen = false;
  }
}
