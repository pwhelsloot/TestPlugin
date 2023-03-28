import { Component, OnInit } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { AmcsModalChildComponent } from '@shared-module/components/amcs-modal/amcs-modal-child-component.interface';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { BehaviorSubject, Subject } from 'rxjs';
import { AmcsModalLookupSelectOptions } from './amcs-modal-lookup-select-options.model';

/**
 * @deprecated Marked for move to PlatformUI
 */
@Component({
    selector: 'app-amcs-modal-lookup-select',
    templateUrl: './amcs-modal-lookup-select.component.html',
    styleUrls: ['./amcs-modal-lookup-select.component.scss']
})
export class AmcsModalLookupSelectComponent extends AutomationLocatorDirective implements OnInit, AmcsModalChildComponent {
    extraData: any;
    loading: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    externalClose: Subject<number | boolean> = new Subject<number | boolean>();
    selectedId: number;

    form: AmcsFormGroup;
    lookupItems: ILookupItem[];
    hasError = false;
    options: AmcsModalLookupSelectOptions = new AmcsModalLookupSelectOptions();

    ngOnInit() {
        const lookupItems = this.extraData[0] as ILookupItem[];
        this.selectedId = isTruthy(this.extraData[1]) ? (this.extraData[1] as number) : null;
        const options = this.extraData[2] as AmcsModalLookupSelectOptions;

        this.lookupItems = isTruthy(lookupItems) ? lookupItems : [];
        if (isTruthy(options)) {
            this.options = options;
        }
    }

    onAccept() {
        if (isTruthy(this.selectedId)) {
            this.externalClose.next(+this.selectedId);
        } else {
            this.hasError = true;
        }
    }

    onCancel() {
        this.externalClose.next(false);
    }
}
