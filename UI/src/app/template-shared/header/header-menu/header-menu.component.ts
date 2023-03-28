import { Component, OnInit, ViewChild, OnDestroy, TemplateRef } from '@angular/core';
import { Subscription } from 'rxjs';
import { HeaderService } from '@core-module/services/header/header.service';
import { AmcsMenuItem } from '@shared-module/models/amcs-menu-item.model';
import { MatDialogRef } from '@angular/material/dialog';
import { MatSidenav } from '@angular/material/sidenav';
import { HeaderComponentService } from '@core-module/services/header/header-component.service';
import { AmcsModalService } from '@shared-module/components/amcs-modal/amcs-modal.service';
import { take } from 'rxjs/operators';
import { HeaderMenuFormService } from '@core-module/services/header/header-menu-form.service';
import { isTruthy } from '@core-module/helpers/is-truthy.function';

@Component({
  selector: 'app-template-header-menu',
  templateUrl: './header-menu.component.html',
  styleUrls: ['./header-menu.component.scss'],
})
export class TemplateHeaderMenuComponent implements OnInit, OnDestroy {
  @ViewChild('sidenav') sidenav: MatSidenav;
  @ViewChild('clickOnceDialogTemplate') clickOnceDialogTemplate: TemplateRef<any>;

  clickOnceDialogModal: MatDialogRef<any, any>;
  dontShowDialogAgain = false;
  disableClickOnceDialogLocalStorageKey = 'disable-clickonce-dialog';

  constructor(
    public formService: HeaderMenuFormService,
    public headerComponentService: HeaderComponentService,
    private headerService: HeaderService,
    private modalService: AmcsModalService
  ) {}

  private menuSubscription: Subscription;
  private isToggling = false;

  ngOnInit() {
    this.menuSubscription = this.headerService.headerMenuButtonClicked.subscribe(() => {
      this.toggleMenu();
    });
  }

  ngOnDestroy() {
    if (isTruthy(this.menuSubscription)) {
      this.menuSubscription.unsubscribe();
    }
  }

  toggleMenu() {
    this.isToggling = true;
    this.sidenav.toggle();
  }

  onItemClick(item: AmcsMenuItem) {
    this.formService.onItemClick(item);
    this.sidenav.close();
  }

  onClickOutside() {
    if (!this.isToggling && this.sidenav.opened) {
      this.sidenav.close();
    }

    if (this.isToggling) {
      this.isToggling = false;
    }
  }

  launchDesktopClient() {
    this.sidenav.close();
    let bypassDialog = true;

    if (this.isFirefox() || this.isChrome()) {
      const disableClickOnceDialog = localStorage.getItem(this.disableClickOnceDialogLocalStorageKey);
      bypassDialog = !!disableClickOnceDialog;
    }

    if (bypassDialog) {
      this.headerComponentService.launchDesktopClient();
    } else {
      this.clickOnceDialogModal = this.modalService.createModal({
        template: this.clickOnceDialogTemplate,
        type: 'confirmation',
        ignoreBackdropClick: true,
      });

      this.clickOnceDialogModal
        .afterClosed()
        .pipe(take(1))
        .subscribe((result) => {
          if (result) {
            this.startDesktopClientInstaller();
          }
        });
    }
  }

  startDesktopClientInstaller() {
    if (this.dontShowDialogAgain) {
      localStorage.setItem(this.disableClickOnceDialogLocalStorageKey, 'true');
    }

    this.headerComponentService.launchDesktopClient();
  }

  openClickOnceExtensionURL() {
    if (this.isFirefox()) {
      window.open(this.headerComponentService.clickOnceExtensionURLFirefox);
    } else if (this.isChrome()) {
      window.open(this.headerComponentService.clickOnceExtensionURLChrome);
    }
  }

  private isFirefox(): boolean {
    return navigator.userAgent.indexOf('Firefox') > -1;
  }

  private isChrome(): boolean {
    return navigator.userAgent.indexOf('Chrome') > 1 && navigator.vendor.indexOf('Google Inc') > -1;
  }
}
