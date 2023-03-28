import { Component, Input } from '@angular/core';
import { FormSavingService } from '@core-module/services/forms/form-saving.service';

/**
 * @todo Rename component to app-amcs-loading-overlay
 */
@Component({
    selector: 'app-amcs-saving',
    templateUrl: './amcs-saving.component.html',
    styleUrls: ['./amcs-saving.component.scss']
})
export class AmcsSavingComponent {

    @Input() savingMessage: string;
    @Input() forceSaving = false;
    @Input('customClass') customClass: string;
    saving = true;

    constructor(private formSavingService: FormSavingService) {
        this.formSavingService.formSaving$.subscribe(formSaving => {
            this.saving = formSaving;
        });
    }

}
