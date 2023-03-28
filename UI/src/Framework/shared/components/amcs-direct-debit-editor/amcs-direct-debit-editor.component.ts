import { Component, ElementRef, EventEmitter, Input, OnInit, Output, Renderer2 } from '@angular/core';
import { FormControl } from '@angular/forms';
import { nameof } from '@core-module/helpers/name-of.function';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { DirectDebitControlTypeEnum } from '@shared-module/models/amcs-direct-debit-control/direct-debit-control-type.enum';
import { DirectDebitDynamicControl } from '@shared-module/models/amcs-direct-debit-control/direct-debit-dynamic-control.model';
import { DirectDebitEditorData } from '@shared-module/models/amcs-direct-debit-control/direct-debit-editor-data.model';
import { DirectDebitForm } from '@shared-module/models/amcs-direct-debit-control/direct-debit-form.model';
import { DirectDebit } from '@shared-module/models/amcs-direct-debit-control/direct-debit.model';
import { PropertyMetadataManager } from '@shared-module/models/property-metadata.manager';
import { DirectDebitService } from '@shared-module/services/amcs-direct-debit-control/direct-debit.service';
import { DirectDebitDataService } from '@shared-module/services/amcs-direct-debit-control/direct-debit.service.data';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { take } from 'rxjs/operators';

/**
 * @deprecated Move to PlatformUI
 */
@Component({
  selector: 'app-amcs-direct-debit-editor',
  templateUrl: './amcs-direct-debit-editor.component.html',
  styleUrls: ['./amcs-direct-debit-editor.component.scss'],
  providers: [DirectDebitService, DirectDebitDataService]
})
export class DirectDebitEditorComponent extends AutomationLocatorDirective implements OnInit {

  @Input() directDebitForm: DirectDebitForm = null;
  @Input() editorData: DirectDebitEditorData;
  @Input() metadata: PropertyMetadataManager;
  @Input() loading = false;
  @Input('umrCustomClass') umrCustomClass: string;
  @Input() colSpan = 3;
  DirectDebitControlTypeEnum = DirectDebitControlTypeEnum;

  @Output('formSubmitted') formSubmitted = new EventEmitter<DirectDebitForm>();
  dynamicControls: DirectDebitDynamicControl[] = [];

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    private translateService: SharedTranslationsService,
    public service: DirectDebitService) {
    super(elRef, renderer);
  }

  ngOnInit() {
    this.service.setUpListeners(this.directDebitForm);
    this.getFormControls();
    this.service.setupBankNameSearchStream(this.editorData.bankLookups);
  }

  getFormControls() {
    this.translateService.translations.pipe(take(1)).subscribe(translations => {
      this.buildDynamicControls(this.directDebitForm.accountName, nameof<DirectDebitForm>('accountName'), DirectDebitControlTypeEnum.input,
        this.metadata.isPropertyDisplayed(nameof<DirectDebit>('accountName')), translations['directDebit.accountName'],
        this.metadata.getPropertyPosition(nameof<DirectDebit>('accountName')), 50);

      this.buildDynamicControls(this.directDebitForm.authorisedSignatory, nameof<DirectDebitForm>('authorisedSignatory'), DirectDebitControlTypeEnum.input,
        this.metadata.isPropertyDisplayed(nameof<DirectDebit>('authorisedSignatory')), translations['directDebit.authorisedSignatory'],
        this.metadata.getPropertyPosition(nameof<DirectDebit>('authorisedSignatory')), 100);

      this.buildDynamicControls(this.directDebitForm.accountNo, nameof<DirectDebitForm>('accountNo'), DirectDebitControlTypeEnum.input,
        this.metadata.isPropertyDisplayed(nameof<DirectDebit>('accountNo')), translations['directDebit.accountNo'],
        this.metadata.getPropertyPosition(nameof<DirectDebit>('accountNo')), 20);

      this.buildDynamicControls(this.directDebitForm.sortCode, nameof<DirectDebitForm>('sortCode'), DirectDebitControlTypeEnum.input,
        this.metadata.isPropertyDisplayed(nameof<DirectDebit>('sortCode')), translations['directDebit.sortCode'],
        this.metadata.getPropertyPosition(nameof<DirectDebit>('sortCode')), 15);

      this.buildDynamicControls(this.directDebitForm.bic, nameof<DirectDebitForm>('bic'), DirectDebitControlTypeEnum.input,
        this.metadata.isPropertyDisplayed(nameof<DirectDebit>('bic')), translations['directDebit.bic'],
        this.metadata.getPropertyPosition(nameof<DirectDebit>('bic')), 20);

      this.buildDynamicControls(this.directDebitForm.iban, nameof<DirectDebitForm>('iban'), DirectDebitControlTypeEnum.input,
        this.metadata.isPropertyDisplayed(nameof<DirectDebit>('iban')), translations['directDebit.iban'],
        this.metadata.getPropertyPosition(nameof<DirectDebit>('iban')), 40);

      this.buildDynamicControls(this.directDebitForm.umr, nameof<DirectDebitForm>('umr'), DirectDebitControlTypeEnum.umr,
        this.metadata.isPropertyDisplayed(nameof<DirectDebit>('umr')), translations['directDebit.umr'],
        this.metadata.getPropertyPosition(nameof<DirectDebit>('umr')), 40);

      this.buildDynamicControls(this.directDebitForm.nationalBankCode, nameof<DirectDebitForm>('nationalBankCode'), DirectDebitControlTypeEnum.input,
        this.metadata.isPropertyDisplayed(nameof<DirectDebit>('nationalBankCode')), translations['directDebit.nationalBankCode'],
        this.metadata.getPropertyPosition(nameof<DirectDebit>('nationalBankCode')), 10);

      this.buildDynamicControls(this.directDebitForm.nationalCheckDigits, nameof<DirectDebitForm>('nationalCheckDigits'), DirectDebitControlTypeEnum.input,
        this.metadata.isPropertyDisplayed(nameof<DirectDebit>('nationalCheckDigits')), translations['directDebit.nationalCheckDigits'],
        this.metadata.getPropertyPosition(nameof<DirectDebit>('nationalCheckDigits')), 2);

      this.buildDynamicControls(this.directDebitForm.ribNumber, nameof<DirectDebitForm>('ribNumber'), DirectDebitControlTypeEnum.rib,
        this.metadata.isPropertyDisplayed(nameof<DirectDebit>('ribNumber')), translations['directDebit.rib'],
        this.metadata.getPropertyPosition(nameof<DirectDebit>('ribNumber')), 50);

      this.buildDynamicControls(this.directDebitForm.branchCode, nameof<DirectDebitForm>('branchCode'), DirectDebitControlTypeEnum.input,
        this.metadata.isPropertyDisplayed(nameof<DirectDebit>('branchCode')), translations['directDebit.branchCode'],
        this.metadata.getPropertyPosition(nameof<DirectDebit>('branchCode')), 30);

      this.buildDynamicControls(this.directDebitForm.dateAuthorised, nameof<DirectDebitForm>('dateAuthorised'), DirectDebitControlTypeEnum.date,
        this.metadata.isPropertyDisplayed(nameof<DirectDebit>('dateAuthorised')), translations['directDebit.dateAuthorised'],
        this.metadata.getPropertyPosition(nameof<DirectDebit>('dateAuthorised')), 0);

      this.buildDynamicControls(this.directDebitForm.directDebitRunConfigurationId, nameof<DirectDebitForm>('directDebitRunConfigurationId'), DirectDebitControlTypeEnum.select,
        this.metadata.isPropertyDisplayed(nameof<DirectDebit>('directDebitRunConfigurationId')), translations['directDebit.ddRunConfig'],
        this.metadata.getPropertyPosition(nameof<DirectDebit>('directDebitRunConfigurationId')), 0, this.editorData.directDebitRunConfigurationLookups);

      this.buildDynamicControls(this.directDebitForm.isProcessed, nameof<DirectDebitForm>('isProcessed'), DirectDebitControlTypeEnum.checkBox,
        this.metadata.isPropertyDisplayed(nameof<DirectDebit>('isProcessed')), translations['directDebit.processAsFirst'],
        this.metadata.getPropertyPosition(nameof<DirectDebit>('isProcessed')), 0);

      this.buildDynamicControls(this.directDebitForm.isVerified, nameof<DirectDebitForm>('isVerified'), DirectDebitControlTypeEnum.checkBox,
        this.metadata.isPropertyDisplayed(nameof<DirectDebit>('isVerified')), translations['directDebit.isVerified'],
        this.metadata.getPropertyPosition(nameof<DirectDebit>('isVerified')), 0);

      this.buildDynamicControls(this.directDebitForm.directDebitAccountTypeId, nameof<DirectDebitForm>('directDebitAccountTypeId'), DirectDebitControlTypeEnum.select,
        this.metadata.isPropertyDisplayed(nameof<DirectDebit>('directDebitAccountTypeId')), translations['directDebit.accountType'],
        this.metadata.getPropertyPosition(nameof<DirectDebit>('directDebitAccountTypeId')), 0, this.editorData.directDebitAccountTypeLookups);

      this.dynamicControls.sort(function(a, b) {
        return a.position - b.position;
      });
    });
  }

  buildDynamicControls(formControl: FormControl, name: string, type: DirectDebitControlTypeEnum, isDisplay: boolean, label: string, position: number, maxLength: number, options?: any[]) {
    const control = new DirectDebitDynamicControl();
    control.isDisplay = isDisplay;
    control.name = name;
    control.label = label;
    control.type = type;
    control.position = position;
    control.options = options;
    control.maxLength = maxLength;
    control.control = formControl;

    this.dynamicControls.push(control);
  }

  save() {
    if (this.directDebitForm.checkIfValid()) {
      this.formSubmitted.next(this.directDebitForm);
    }
  }
}
