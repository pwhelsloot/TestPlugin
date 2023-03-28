import { Component, ViewEncapsulation, OnInit, OnDestroy } from '@angular/core';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { HeaderComponentService } from '@core-module/services/header/header-component.service';
import { InnerTileServiceUI } from '@core-module/services/ui/inner-tile.service.ui';
import { ApplicationConfigurationStore } from '@core-module/services/config/application-configuration.store';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { TemplateModuleAppRoutes } from '@app/template/template-module-app-routes.constants';

@Component({
  selector: 'app-template-header-mobile',
  templateUrl: './header-mobile.component.html',
  styleUrls: ['./header-mobile.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: [
    trigger('expandCollapse', [
      state(
        'true',
        style({
          top: '50px',
        })
      ),
      state(
        'false',
        style({
          top: '0px',
        })
      ),
      transition('true <=> false', animate('400ms cubic-bezier(0.25, 0.8, 0.25, 1)')),
    ]),
  ],
})
export class TemplateHeaderMobileComponent implements OnInit, OnDestroy {
  isSearchClicked: boolean;
  profileDataThumbnail: string;
  profileInitials: string;
  communicationCount = 0;

  constructor(
    public headerComponentService: HeaderComponentService,
    public tileUiService: InnerTileServiceUI,
    private applicationConfigurationStore: ApplicationConfigurationStore,
    private router: Router
  ) {
    this.isSearchedClickedSub = this.headerComponentService.headerService.isSearchedClicked.subscribe((value: boolean) => {
      this.isSearchClicked = value;
    });
  }

  private isSearchedClickedSub: Subscription;

  ngOnInit() {
    this.headerComponentService.globalSearchServiceUI.showGo.next(true);
    this.applicationConfigurationStore.profileThumbnail$.subscribe((data) => {
      this.profileDataThumbnail = data[0] || '';
      this.profileInitials = data[1] || '';
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
  }

  goToDashboard() {
    this.headerComponentService.headerService.isSearchedClicked.next(false);
    this.router.navigate([TemplateModuleAppRoutes.module, TemplateModuleAppRoutes.dashboard]);
  }
}
