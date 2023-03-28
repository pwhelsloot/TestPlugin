import { isNullOrUndefined } from '@core-module/helpers/is-truthy.function';

export class AmcsMenuItem {
    constructor(
        public key: string,
        public text: string,
        public icon: string,
        public smallIcon?: string,
        public isSelected?: boolean,
        public isDisabled?: boolean) {
        if (isNullOrUndefined(isSelected)) {
            this.isSelected = false;
        }
    }
}
