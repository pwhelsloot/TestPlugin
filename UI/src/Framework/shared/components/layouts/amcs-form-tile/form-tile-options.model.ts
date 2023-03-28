
import { FormOptions } from '../amcs-form/form-options.model';

export class FormTileOptions {
    formOptions: FormOptions = new FormOptions();
    featureTitle: string;
    editorTitle: string;
    enableReturn = false;
    enableLog = false;
    tileBodyMinHeight: number = null;
}
