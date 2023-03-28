import { MediaMatcher } from '@angular/cdk/layout';
import { Component, ElementRef, NgZone, OnDestroy, Renderer2 } from '@angular/core';
import { MediaSizes } from '@coremodels/media-sizes.constants';
import { UserCommunicationServiceData } from '@coreservices/header/data/user-communication.service.data';
import { UserCommunicationService } from '@coreservices/header/user-communication.service';
import { UserNotificationService } from '@coreservices/header/user-notification.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

/**
 * @deprecated Move to PlatformUI
 */
@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  providers: [UserCommunicationService, UserCommunicationServiceData, UserNotificationService]
})
export class HeaderComponent extends AutomationLocatorDirective implements OnDestroy {

  mobileQuery: MediaQueryList;

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    media: MediaMatcher, private zone: NgZone) {
    super(elRef, renderer);
    this.mobileQuery = media.matchMedia('(max-width: ' + MediaSizes.small.toString() + 'px)');

    this._mobileQueryListener = () => {
      // This re-renders the page once per media change
      this.zone.run(() => { });
    };
    this.mobileQuery.addListener(this._mobileQueryListener);
  }

  private _mobileQueryListener: () => void;

  ngOnDestroy() {
    this.mobileQuery.removeListener(this._mobileQueryListener);
  }
}
