import { Directive } from '@angular/core';
import { NgControl } from '@angular/forms';
import { IAmcsRadioControl } from './amcs-radio-control.interface';

@Directive({
    // tslint:disable:directive-selector
    // eslint-disable-next-line
    selector: '[radioControl]'
})
export class AmcsRadioControlRegistryDirective {
    private _accessors: any[] = [];

    add(control: NgControl, accessor: IAmcsRadioControl) {
        this._accessors.push([control, accessor]);
    }

    remove(accessor: IAmcsRadioControl) {
        for (let i = this._accessors.length - 1; i >= 0; --i) {
            if (this._accessors[i][1] === accessor) {
                this._accessors.splice(i, 1);
                return;
            }
        }
    }

    select(accessor: IAmcsRadioControl) {
        this._accessors.forEach((c) => {
            if (this._isSameGroup(c, accessor)) {
                c[1].fireCheck(accessor.value);
            }
        });
        this._accessors.forEach((c) => {
            if (this._isSameGroup(c, accessor)) {
                c[1].afterSelection();
            }
        });
    }

    private _isSameGroup(
        controlPair: [NgControl, IAmcsRadioControl],
        accessor: IAmcsRadioControl): boolean {
        if (!controlPair[0].control) {
            return false;
        }
        return controlPair[1].formControlName === accessor.formControlName;
    }
}
