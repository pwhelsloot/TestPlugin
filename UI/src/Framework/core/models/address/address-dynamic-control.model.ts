import { AbstractControl } from '@angular/forms';
import { AddressControlTypeEnum } from './address-control-type.enum';

/**
 * @deprecated Move to PlatformUI + ScaleUI (or new module) https://dev.azure.com/amcsgroup/Platform/_workitems/edit/188268
 */
export class AddressDynamicControl {

    type: AddressControlTypeEnum;

    className: string;

    isDisplay: boolean;

    isMandatory: boolean;

    position: number;

    name: string;

    label: string;

    maxLength: number;

    options: any[];

    control: AbstractControl;
}
