import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { FormModelSizeEnum } from '@shared-module/models/form-modal-size.enum';
import { Subject } from 'rxjs';
import { SBExampleFormModal } from './storybook-example-form-data.model';

@Component({
  selector: 'app-amcs-form-modal-storybook-loader',
  templateUrl: './amcs-form-modal-storybook-loader.component.html'
})
export class AmcsFormModalStorybookLoaderComponent implements OnChanges {
  @Input() formModalSize: FormModelSizeEnum = FormModelSizeEnum.Standard;
  @Input() title: string;
  @Input() isExpandable = true;

  formModalString: string;
  buttonTitle: string;
  triggerModalSubject = new Subject();

  ngOnChanges(changes: SimpleChanges) {
    if (changes['isExpandable'] || changes['formModalSize']) {
      this.buttonTitle = `${this.formModalSize === FormModelSizeEnum.Standard ? 'Standard' : 'Double'} ${
        this.isExpandable ? 'Expandable' : ''
      }`;
    }
  }

  createModal() {
    this.triggerModalSubject.next();
  }

  handleFormModalSaved(dataModel: SBExampleFormModal) {
    this.formModalString = `Form Values: ${dataModel.post(dataModel, SBExampleFormModal)}`;
  }

  handleCancelModalClosed() {
    this.formModalString = null;
  }
}
