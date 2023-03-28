import { FormModelSizeEnum } from './form-modal-size.enum';

export class FormModalOptions {
    size = FormModelSizeEnum.Standard;
    canExpand = false;
    showConfirmationOnCancel = false;
}
