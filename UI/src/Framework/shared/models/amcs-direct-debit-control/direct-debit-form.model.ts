import { FormControl, ValidatorFn, Validators } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { nameof } from '@core-module/helpers/name-of.function';
import { AmcsDatepickerConfig } from '@shared-module/components/amcs-datepicker/amcs-datepicker-config.model';
import { BaseForm } from '@shared-module/forms/base-form.model';
import { PropertyMetadataManager } from '@shared-module/models/property-metadata.manager';
import { DirectDebit } from './direct-debit.model';

/**
 * @deprecated Move to PlatformUI
 */
export class DirectDebitForm extends BaseForm<DirectDebit, DirectDebitForm> {

    accountName: FormControl;

    authorisedSignatory: FormControl;

    bankName: FormControl;

    accountNo: FormControl;

    sortCode: FormControl;

    bic: FormControl;

    iban: FormControl;

    umr: FormControl;

    nationalBankCode: FormControl;

    nationalCheckDigits: FormControl;

    ribNumber: FormControl;

    branchCode: FormControl;

    dateAuthorised: FormControl;

    directDebitRunConfigurationId: FormControl;

    address1: FormControl;

    address2: FormControl;

    address3: FormControl;

    address4: FormControl;

    address5: FormControl;

    postcode: FormControl;

    directDebitId: FormControl;

    isVerified: FormControl;

    isProcessed: FormControl;

    directDebitAccountTypeId: FormControl;

    validationContextId: FormControl;

    firstDateForPaymentRequest: FormControl;

    requestPaymentThisMonth: FormControl;

    firstDateForPaymentRequestConfig: AmcsDatepickerConfig = Object.assign({}, { containerClass: 'amcs-datepicker' });

    buildForm(data: DirectDebit, extraParams: any[]): DirectDebitForm {
        const metadataManager: PropertyMetadataManager = extraParams[0];
        const form = new DirectDebitForm();
        form.directDebitId = new FormControl(data.directDebitId);

        form.directDebitRunConfigurationId = new FormControl(data.directDebitRunConfigurationId, { validators: Validators.required });
        form.validationContextId = new FormControl(data.validationContextId, { validators: Validators.required });
        form.address1 = new FormControl(data.address1);
        form.address2 = new FormControl(data.address2);
        form.address3 = new FormControl(data.address3);
        form.address4 = new FormControl(data.address4);
        form.address5 = new FormControl(data.address5);
        form.postcode = new FormControl(data.postcode);
        form.accountName = new FormControl(data.accountName, { validators: this.requiredValidator(nameof<DirectDebit>('accountName'), metadataManager) });
        form.authorisedSignatory = new FormControl(data.authorisedSignatory, { validators: this.requiredValidator(nameof<DirectDebit>('authorisedSignatory'), metadataManager) });
        form.accountNo = new FormControl(data.accountNo, { validators: this.requiredValidator(nameof<DirectDebit>('accountNo'), metadataManager) });
        form.sortCode = new FormControl(data.sortCode, { validators: this.requiredValidator(nameof<DirectDebit>('sortCode'), metadataManager) });
        form.bic = new FormControl(data.bic, { validators: this.requiredValidator(nameof<DirectDebit>('bic'), metadataManager) });
        form.umr = new FormControl(data.umr, { validators: this.requiredValidator(nameof<DirectDebit>('umr'), metadataManager) });
        form.iban = new FormControl(data.iban, { validators: this.requiredValidator(nameof<DirectDebit>('iban'), metadataManager) });
        form.ribNumber = new FormControl(data.ribNumber, { validators: this.requiredValidator(nameof<DirectDebit>('ribNumber'), metadataManager) });
        form.nationalBankCode = new FormControl(data.nationalBankCode, { validators: this.requiredValidator(nameof<DirectDebit>('nationalBankCode'), metadataManager) });
        form.nationalCheckDigits = new FormControl(data.nationalCheckDigits, { validators: this.requiredValidator(nameof<DirectDebit>('nationalCheckDigits'), metadataManager) });
        form.branchCode = new FormControl(data.branchCode, { validators: this.requiredValidator(nameof<DirectDebit>('branchCode'), metadataManager) });
        form.dateAuthorised = new FormControl(data.dateAuthorised, { validators: this.requiredValidator(nameof<DirectDebit>('dateAuthorised'), metadataManager) });
        form.isVerified = new FormControl(data.isVerified, { validators: this.requiredValidator(nameof<DirectDebit>('isVerified'), metadataManager) });
        form.isProcessed = new FormControl(data.isProcessed, { validators: this.requiredValidator(nameof<DirectDebit>('isProcessed'), metadataManager) });
        form.directDebitAccountTypeId = new FormControl(data.directDebitAccountTypeId, { validators: this.requiredValidator(nameof<DirectDebit>('directDebitAccountTypeId'), metadataManager) });

        form.firstDateForPaymentRequest = new FormControl(data.firstDateForPaymentRequest, { validators: Validators.required });
        form.requestPaymentThisMonth = new FormControl(false);
        // Search controls (need full object as value)
        form.bankName = this.buildTypeaheadFormControl(data.bankId, data.bankName, [Validators.required]);
        return form;
    }

    parseForm(typedForm: DirectDebitForm): DirectDebit {
        const dataModel = new DirectDebit();

        dataModel.directDebitId = typedForm.directDebitId.value;
        dataModel.accountName = typedForm.accountName.value;
        dataModel.authorisedSignatory = typedForm.authorisedSignatory.value;
        dataModel.bankName = typedForm.bankName.value ? typedForm.bankName.value.description : null;
        dataModel.accountNo = typedForm.accountNo.value;
        dataModel.sortCode = typedForm.sortCode.value;
        dataModel.bic = typedForm.bic.value;
        dataModel.iban = typedForm.iban.value;
        dataModel.umr = typedForm.umr.value;
        dataModel.nationalBankCode = typedForm.nationalBankCode.value;
        dataModel.nationalCheckDigits = typedForm.nationalCheckDigits.value;
        dataModel.ribNumber = typedForm.ribNumber.value;
        dataModel.branchCode = typedForm.branchCode.value;
        dataModel.dateAuthorised = typedForm.dateAuthorised.value;
        dataModel.directDebitRunConfigurationId = typedForm.directDebitRunConfigurationId.value;
        dataModel.address1 = typedForm.address1.value;
        dataModel.address2 = typedForm.address2.value;
        dataModel.address3 = typedForm.address3.value;
        dataModel.address4 = typedForm.address4.value;
        dataModel.address5 = typedForm.address5.value;
        dataModel.postcode = typedForm.postcode.value;
        dataModel.isVerified = typedForm.isVerified.value;
        dataModel.isProcessed = typedForm.isProcessed.value;
        dataModel.directDebitAccountTypeId = typedForm.directDebitAccountTypeId.value;
        dataModel.validationContextId = typedForm.validationContextId.value;
        dataModel.firstDateForPaymentRequest = typedForm.firstDateForPaymentRequest.value;

        return dataModel;
    }

    private requiredValidator(name: string, metadataManager: PropertyMetadataManager): ValidatorFn {
        if (metadataManager.isPropertyMandatory(name)) {
            return Validators.required;
        } else {
            return Validators.nullValidator;
        }
    }

    private buildTypeaheadFormControl(id: number, description: string, validators?: ValidatorFn[]): FormControl {
        if (isTruthy(id) && isTruthy(description)) {
            return new FormControl({ id, description }, validators);
        } else {
            return new FormControl(null, validators);
        }
    }
}
