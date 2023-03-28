import { FormControl } from '@angular/forms';
import { DirectDebitControlTypeEnum } from './direct-debit-control-type.enum';

/**
 * @deprecated Move to PlatformUI
 */
export class DirectDebitDynamicControl {

    type: DirectDebitControlTypeEnum;

    className: string;

    isDisplay: boolean;

    position: number;

    name: string;

    label: string;

    options: any[];

    maxLength: number;

    control: FormControl;
}
