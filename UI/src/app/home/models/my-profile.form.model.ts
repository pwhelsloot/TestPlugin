import { FormControl, Validators } from '@angular/forms';
import { formAlias } from '@coreconfig/form-to-alias.function';
import { MyProfileData } from '@core-module/models/external-dependencies/profile/my-profile-data.model';

export class MyProfileForm {
  @formAlias('uiLanguage')
  uiLanguage: FormControl;

  @formAlias('printerId')
  printerId: FormControl;

  @formAlias('autoPrint')
  autoPrint: FormControl;

  static build(data: MyProfileData): MyProfileForm {
    const form = new MyProfileForm();
    form.uiLanguage = new FormControl(data.uiLanguage, Validators.required);
    form.printerId = new FormControl(data.printerId);
    form.autoPrint = new FormControl(data.autoPrint);
    return form;
  }
}
