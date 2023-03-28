import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { isNullOrUndefined } from '@core-module/helpers/is-truthy.function';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';

@Component({
  selector: 'app-amcs-mobile-action-buttons',
  templateUrl: './amcs-mobile-action-buttons.component.html',
  styleUrls: ['./amcs-mobile-action-buttons.component.scss']
})
export class AmcsMobileActionButtonsComponent implements OnInit {

  @Input() cancelText: string;
  @Input() cancelSecondaryColor = false;
  @Input() cancelPrimaryColor = false;
  @Input() hideCancelButton = false;
  @Input() saveText: string;
  @Input() enableSave = true;
  @Input() saving = false;
  @Output() onSave = new EventEmitter();
  @Output() onCancel = new EventEmitter();

  cancelButtonCustomClass = 'btn noMargin btn-default mobile-btn';
  primaryButtonCustomClass = 'btn noMargin amcs-mobile-green mobile-btn';
  isOnlySaveButton = false;

  constructor(private translationsService: SharedTranslationsService) { }

  ngOnInit() {
    if (this.cancelSecondaryColor) {
      this.cancelButtonCustomClass = 'btn noMargin default mobile-btn amcs-mobile-grey';
    }

    if (this.cancelPrimaryColor) {
      this.primaryButtonCustomClass = this.cancelButtonCustomClass;
    }

    if (isNullOrUndefined(this.cancelText)) {
      this.cancelText = this.translationsService.getTranslation('amcsMobileLayout.cancel');
    }

    if (isNullOrUndefined(this.saveText)) {
      this.saveText = this.translationsService.getTranslation('amcsMobileLayout.save');
    }

    if (this.hideCancelButton) {
      this.isOnlySaveButton = true;
      this.primaryButtonCustomClass += ' no-max-width';
    }
  }

  cancel() {
    this.onCancel.emit();
  }

  save() {
    if (this.enableSave && !this.saving) {
      this.onSave.emit();
    }
  }
}
