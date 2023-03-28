import { Component, ViewEncapsulation, OnInit, OnDestroy } from '@angular/core';
import { AmcsModalService } from '@shared-module/components/amcs-modal/amcs-modal.service';
import { HeaderComponentService } from '@core-module/services/header/header-component.service';
import { ApplicationConfigurationStore } from '@core-module/services/config/application-configuration.store';
import { Router } from '@angular/router';
import { TemplateModuleAppRoutes } from '@app/template/template-module-app-routes.constants';

@Component({
  selector: 'app-template-header-desktop',
  templateUrl: './header-desktop.component.html',
  styleUrls: ['./header-desktop.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [AmcsModalService],
})
export class TemplateHeaderDesktopComponent implements OnInit, OnDestroy {
  profileDataThumbnail: string;
  profileInitials: string;

  constructor(
    public headerComponentService: HeaderComponentService,
    private applicationConfigurationStore: ApplicationConfigurationStore,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.applicationConfigurationStore.profileThumbnail$.subscribe((data) => {
      this.profileDataThumbnail = data[0] || '';
      this.profileInitials = data[1] || '';
    });
  }

  ngOnDestroy() {}

  goToDashboard() {
    this.headerComponentService.headerService.isSearchedClicked.next(false);
    this.router.navigate([TemplateModuleAppRoutes.module, TemplateModuleAppRoutes.dashboard]);
  }

  close() {
    this.headerComponentService.isOpen = false;
  }

  closeCommunications() {
    this.headerComponentService.isCommunicationsOpen = false;
  }
}
