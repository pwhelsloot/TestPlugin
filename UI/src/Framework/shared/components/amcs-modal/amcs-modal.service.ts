import { Injectable } from '@angular/core';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsModalConfig } from '@shared-module/components/amcs-modal/amcs-modal-config.model';
import { AmcsModalComponent } from '@shared-module/components/amcs-modal/amcs-modal.component';

@Injectable({ providedIn: 'root' })
export class AmcsModalService {
  constructor(private dialogService: MatDialog) { }

  createModal(config: AmcsModalConfig): MatDialogRef<any, any> {
    // Data we'll pass to AmcsModalComponent
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data = {
      template: config.template,
      title: config.title,
      buttonText: config.buttonText,
      type: config.type,
      baseColor: config.baseColor,
      extraData: config.extraData,
      largeSize: config.width ? false : config.largeSize,
      hideButtons: config.hideButtons,
      redirectUrlOnClosing: config.redirectUrlOnClosing,
      width: config.width,
      icon: config.icon,
      isMobile: config.isMobile,
      isError: config.isError,
      buttons: config.buttons,
      hideCloseButton: config.hideCloseButton
    };
    dialogConfig.autoFocus = config.autoFocus;
    dialogConfig.disableClose = config.ignoreBackdropClick || false;
    return this.dialogService.open(AmcsModalComponent, dialogConfig);
  }

  closeAll() {
    if (this.dialogService.openDialogs && this.dialogService.openDialogs.length > 0) {
      this.dialogService.openDialogs.forEach(element => {
        element.close();
      });
    }
  }

  getModalsCount(): number {
    return this.dialogService.openDialogs.length;
  }

  getErrorModalsCount(): number {
    return this.dialogService.openDialogs.filter(x => isTruthy(x._containerInstance._config.data) && x._containerInstance._config.data.type === 'alert').length;
  }
}
