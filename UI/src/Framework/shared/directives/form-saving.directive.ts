import { Directive, HostListener } from '@angular/core';
import { FormSavingService } from '@core-module/services/forms/form-saving.service';

@Directive({
  selector: '[appFormSaving]',
  providers: [FormSavingService]
})
export class FormSavingDirective {
  constructor(private formSavingService: FormSavingService) { }

  // 'onManualSubmit' is a DOM event fired by the different shared component, whereas 'submit' is fired by a form element
  @HostListener('onSubmit')
  @HostListener('submit')
  formSaving() {
    this.formSavingService.formSubmitted();
  }
}
