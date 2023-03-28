import { isNullOrUndefined } from 'util';

export class AmcsNavBarMenuItem {
    constructor(
        public text: string,
        public icon: string,
        public url: string,
        public showOnHeader: boolean,
        public isSelected?: boolean) {
        if (isNullOrUndefined(isSelected)) {
            this.isSelected = false;
        }
    }
}
