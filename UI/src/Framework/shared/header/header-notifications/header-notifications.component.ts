import { Component, ElementRef, Input, OnChanges, OnDestroy, OnInit, Renderer2, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { UserNotification } from '@coremodels/header/user-notification.model';
import { CommunicationEditorContextEnum } from '@coremodels/lookups/communication-editor-context.enum';
import { UserNotificationService } from '@coreservices/header/user-notification.service';
import { RouteContextService } from '@coreservices/route-context.service';
import { CoreAppRoutes } from '@routeconfig/core-app-routes.constants';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { LocalizedDatePipe } from '@translate/localized-datePipe';
import { Subscription } from 'rxjs';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-header-notifications',
  templateUrl: './header-notifications.component.html',
  styleUrls: ['./header-notifications.component.scss']
})
export class HeaderNotificationsComponent extends AutomationLocatorDirective implements OnInit, OnChanges, OnDestroy {

  @Input() opened: boolean;
  @Input() count: number;

  userNotifications: UserNotification[];
  loading = false;

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    private userNotificationService: UserNotificationService,
    private sharedTranslationsService: SharedTranslationsService,
    private datePipe: LocalizedDatePipe,
    private router: Router,
    private routeContextService: RouteContextService
  ) {
    super(elRef, renderer);
  }

  private userNotificationSubscription: Subscription;

  ngOnInit(): void {
    this.userNotificationSubscription = this.userNotificationService.notifications$
      .subscribe(data => {
        this.userNotifications = data;
        this.loading = false;
      });
  }

  ngOnChanges(simpleChanges: SimpleChanges) {
    if (simpleChanges.opened && this.opened) {
      this.loading = true;
      this.userNotificationService.requestNotifications();
    }
  }

  ngOnDestroy() {
    this.userNotificationSubscription.unsubscribe();
  }

  refreshCommunicationCount() {
    this.userNotificationService.requestNotificationsCount();
  }

  onRowSelected(data: UserNotification) {
    if (!data.isAssignedToGroup) {
      this.userNotificationService.saveCommunicationViewed(data.communicationFollowupId, true).pipe(take(1)).subscribe(result => {
        this.refreshCommunicationCount();
      });
    } else {
      this.refreshCommunicationCount();
    }
    this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
      this.routeContextService.setValue(CommunicationEditorContextEnum.EditFollowup, data.communicationFollowupId.toString());
      this.router.navigate([CoreAppRoutes.customerModule + '/' + data.customerId + '/' + CoreAppRoutes.communication + '/' + data.communicationId]);
    });
  }
  getTranslatedFieldName(fieldName: string) {
    fieldName = (fieldName === 'CommunicationGroupId') ? 'AssignedToSysUserId' : fieldName;
    return this.sharedTranslationsService.getTranslation('shared.header.notifications.' + fieldName.charAt(0).toLowerCase() + fieldName.slice(1));
  }

  getFieldValueForDisplay(notification: UserNotification) {
    switch (notification.field) {
      case 'FollowupDate':
        return this.datePipe.transform(notification.newValue, 'shortDate');
      case 'AssignedToSysUserId':
      case 'CommunicationGroupId':
        return notification.sysUserAssignedToFullName;
      case 'CommunicationFollowupStatusId':
        return notification.followupStatusDescription;
    }
  }
}
