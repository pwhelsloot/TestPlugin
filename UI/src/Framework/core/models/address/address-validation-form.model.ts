import { FormControl, ValidatorFn, Validators } from '@angular/forms';
import { nameof } from '@core-module/helpers/name-of.function';
import { formAlias } from '@coreconfig/form-to-alias.function';
import { PropertyMetadataManager } from '@shared-module/models/property-metadata.manager';
import { AmcsValidators } from '@shared-module/validators/AmcsValidators.model';
import { AddressValidation } from './address-validation.model';

/**
 * @deprecated Move to PlatformUI + ScaleUI (or new module) https://dev.azure.com/amcsgroup/Platform/_workitems/edit/188268
 */
export abstract class AddressValidationForm {

    @formAlias('houseNumber')
    houseNumber: FormControl;

    @formAlias('address1')
    address1: FormControl;

    @formAlias('address2')
    address2: FormControl;

    @formAlias('address3')
    address3: FormControl;

    @formAlias('address4')
    address4: FormControl;

    @formAlias('address5')
    address5: FormControl;

    @formAlias('address6')
    address6: FormControl;

    @formAlias('address7')
    address7: FormControl;

    @formAlias('address8')
    address8: FormControl;

    @formAlias('address9')
    address9: FormControl;

    @formAlias('postcode')
    postcode: FormControl;

    @formAlias('longitude')
    longitude: FormControl;

    @formAlias('latitude')
    latitude: FormControl;

    @formAlias('federalId')
    federalId: FormControl;

    static buildAddress<T extends AddressValidation>(form: AddressValidationForm, address: T): AddressValidationForm {
        const metadataManager = new PropertyMetadataManager(address);
        form.houseNumber = new FormControl(address.houseNumber, {
            validators: [this.requiredValidator(nameof<AddressValidation>('houseNumber'), metadataManager), Validators.maxLength(50)], updateOn: 'blur'
        });
        form.address1 = new FormControl(address.address1, {
            validators: [this.requiredValidator(nameof<AddressValidation>('address1'), metadataManager), Validators.maxLength(50)], updateOn: 'blur'
        });
        form.address2 = new FormControl(address.address2, {
            validators: [this.requiredValidator(nameof<AddressValidation>('address2'), metadataManager), Validators.maxLength(50)], updateOn: 'blur'
        });
        form.address3 = new FormControl(address.address3, {
            validators: [this.requiredValidator(nameof<AddressValidation>('address3'), metadataManager), Validators.maxLength(50)], updateOn: 'blur'
        });
        form.address4 = new FormControl(address.address4, {
            validators: [this.requiredValidator(nameof<AddressValidation>('address4'), metadataManager), Validators.maxLength(50)], updateOn: 'blur'
        });
        form.address5 = new FormControl(address.address5, {
            validators: [this.requiredValidator(nameof<AddressValidation>('address5'), metadataManager), Validators.maxLength(50)], updateOn: 'blur'
        });
        form.address6 = new FormControl(address.address6, {
            validators: [Validators.maxLength(50)], updateOn: 'blur'
        });
        form.address7 = new FormControl(address.address7, {
            validators: [Validators.maxLength(50)], updateOn: 'blur'
        });
        form.address8 = new FormControl(address.address8, {
            validators: [Validators.maxLength(50)], updateOn: 'blur'
        });
        form.address9 = new FormControl(address.address9, {
            validators: [Validators.maxLength(50)], updateOn: 'blur'
        });
        form.postcode = new FormControl(address.postcode, {
            validators: [this.requiredValidator(nameof<AddressValidation>('postcode'), metadataManager), Validators.maxLength(12)], updateOn: 'blur'
        });
        form.latitude = new FormControl(address.latitude, { validators: [Validators.maxLength(22), AmcsValidators.validCoordinate('latitude')], updateOn: 'blur' });
        form.longitude = new FormControl(address.longitude, { validators: [Validators.maxLength(22), AmcsValidators.validCoordinate('longitude')], updateOn: 'blur' });
        form.federalId = new FormControl(address.federalId);
        return form;
    }

    private static requiredValidator(name: string, metadataManager: PropertyMetadataManager): ValidatorFn {
        if (metadataManager.isPropertyMandatory(name)) {
            return Validators.required;
        } else {
            return Validators.nullValidator;
        }
    }
}
