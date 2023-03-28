import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { CompanyOutletLookup } from '@core-module/models/lookups/company-outlet-lookup.model';
import { FormOptions } from '@shared-module/components/layouts/amcs-form/form-options.model';
import { AmcsFormBuilder } from '@shared-module/forms/amcs-form-builder.model';
import { BaseFormModelComponent } from '@shared-module/models/base-form-modal.component';
import { FormModelSizeEnum } from '@shared-module/models/form-modal-size.enum';
import { Observable, of } from 'rxjs';
import { SBExampleFormModal } from './storybook-example-form-data.model';
import { SBExampleModalForm } from './storybook-example-form.model';
@Component({
  selector: 'app-storybook-example-form',
  templateUrl: './storybook-example-form.component.html'
})
export class StorybookExampleFormComponent extends BaseFormModelComponent implements OnChanges {
  @Input() modalTitle: string;
  @Input() modalSize: FormModelSizeEnum;
  @Input() isExpandable: boolean;
  @Output() saved = new EventEmitter<SBExampleFormModal>();

  formOptions: FormOptions = new FormOptions();
  formModalString: string;
  form: SBExampleModalForm = null;
  options: CompanyOutletLookup[] = [];

  constructor(private formBuilder: FormBuilder) {
    super();
    this.formOptions.enableFormActions = false;
    this.modalOptions = { size: this.modalSize, canExpand: true, showConfirmationOnCancel: false};
  }
  ngOnChanges(changes: SimpleChanges) {
    if (changes['isExpandable']) {
      this.modalOptions.canExpand = this.isExpandable;
    }
    if (changes['modalSize'] && this.modalSize) {
      this.modalOptions.size = this.modalSize;
    }
  }

  loadEditorData() {
    // faking api delay
    setTimeout(() => {
      this.populateOptions();
      this.form = AmcsFormBuilder.buildForm(this.formBuilder, new SBExampleFormModal(), SBExampleModalForm);
      this.loaded.next(true);
    }, 1000);
  }

  saveForm(): Observable<boolean> {
    if (this.form.checkIfValid()) {
      this.saved.emit(AmcsFormBuilder.parseForm(this.form, SBExampleModalForm));
      return of(true);
    }
    return of(false);
  }

  handleFormModalSaved(dataModel: SBExampleFormModal) {
    this.formModalString = dataModel.post(dataModel, SBExampleFormModal);
  }

  handleCancelModalClosed() {
    this.formModalString = null;
  }

  private populateOptions() {
    this.options = [];
    const row1 = new CompanyOutletLookup();
    row1.id = 1;
    row1.description = 'Description One';

    const row2 = new CompanyOutletLookup();
    row2.id = 2;
    row2.description = 'Description Two';

    const row3 = new CompanyOutletLookup();
    row3.id = 3;
    row3.description = 'Description Three';

    this.options.push(row1);
    this.options.push(row2);
    this.options.push(row3);
  }
}
