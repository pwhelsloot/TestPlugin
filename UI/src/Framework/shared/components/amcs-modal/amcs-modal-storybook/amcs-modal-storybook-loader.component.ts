import { Component, Input, TemplateRef } from '@angular/core';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { AmcsModalButton } from '../amcs-modal-button.model';
import { AmcsModalService } from '../amcs-modal.service';
import { ModalStorybookLoaderSize, ModalStorybookLoaderType } from './amcs-modal-storybook.enum';

@Component({
  selector: 'app-amcs-modal-storybook-loader',
  templateUrl: './amcs-modal-storybook-loader.component.html'
})
export class AmcsModalStorybookLoaderComponent {
  @Input() modalSize: ModalStorybookLoaderSize = ModalStorybookLoaderSize.Standard;
  @Input() modalType: ModalStorybookLoaderType = ModalStorybookLoaderType.Alert;
  @Input() hideButtons = false;
  @Input() isMobile = false;
  @Input() hideCloseButton = false;
  @Input() title: string;
  @Input() baseColor: string;
  @Input() extraData: any;
  @Input() redirectUrlOnClosing: string;
  @Input() width: string;
  @Input() icon: string;
  @Input() buttons: AmcsModalButton[];

  constructor(private modalService: AmcsModalService, public translationsService: SharedTranslationsService) {}

  createModal(type: ModalStorybookLoaderType, template: TemplateRef<any>) {
    this.modalService.createModal({
      template,
      largeSize: this.modalSize === ModalStorybookLoaderSize.Large,
      type,
      title: this.title,
      hideButtons: this.hideButtons,
      isMobile: this.isMobile,
      hideCloseButton: this.hideCloseButton,
      baseColor: this.baseColor,
      extraData: this.extraData,
      redirectUrlOnClosing: this.redirectUrlOnClosing,
      width: this.width,
      icon: this.icon,
      buttons: this.buttons
    });
  }
}
