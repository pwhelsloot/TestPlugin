
import { FormOptions } from '@shared-module/components/layouts/amcs-form/form-options.model';
import { BehaviorSubject, Observable } from 'rxjs';
import { BaseFormGroup } from '../forms/base-form-group.model';
import { FormModalOptions } from './form-modal-options.model';

/**
 * @todo Rename to BaseFormModalComponent
 */
export abstract class BaseFormModelComponent {
    formOptions: FormOptions;
    modalOptions: FormModalOptions;
    loaded = new BehaviorSubject<boolean>(false);
    isExpanded = false;
    abstract modalTitle: string;
    abstract form: BaseFormGroup;
    constructor() {
        this.modalOptions = new FormModalOptions();
        // RDM - Looks a bit weird but we want to hide all buttons here, we want the modal component to control
        // the buttons (so it can close/open)
        this.formOptions = new FormOptions();
        this.formOptions.actionOptions.enableSave = false;
        this.formOptions.actionOptions.enableContinue = false;
        this.formOptions.actionOptions.enableBack = false;
        this.formOptions.actionOptions.enableCancel = false;
    }
    // After 'loadEditorData' you'll want to set loaded.next(true)!
    abstract loadEditorData();
    // The bool this returns just tells us if we want the modal to close or not
    abstract saveForm(): Observable<boolean>;
}
