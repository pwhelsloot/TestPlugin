import { EventEmitter, Injectable } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { UserLookup } from '@core-module/models/lookups/user-lookup.model';
import { PersonalAccessTokenForm } from '@core-module/models/personal-access-token/personal-access-token-form.model';
import { PersonalAccessToken } from '@core-module/models/personal-access-token/personal-access-token.model';
import { AmcsDatepickerConfig } from '@shared-module/components/amcs-datepicker/amcs-datepicker-config.model';
import { AmcsFormBuilder } from '@shared-module/forms/amcs-form-builder.model';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { take } from 'rxjs/operators';
import { PersonalAccessTokenBrowserService } from './personal-access-token-browser.service';

@Injectable()
export class PersonalAccessTokenFormService {

  form: PersonalAccessTokenForm = null;
  startDateConfig: AmcsDatepickerConfig;
  endDateConfig: AmcsDatepickerConfig;
  expireOptions: { id: number; description: string }[] = [];
  users: UserLookup[];

  onReturn: EventEmitter<void>;

  constructor(private businessService: PersonalAccessTokenBrowserService,
    private translationsService: SharedTranslationsService,
    private formBuilder: FormBuilder) {
    this.buildForm();
    this.translationsService.translations.pipe(take(1)).subscribe(translations => {
      this.buildExpireOptions(translations);
    });
    this.loadUsers();
  }

  return() {
    this.onReturn.emit();
  }

  buildForm() {
    this.form = AmcsFormBuilder.buildForm(this.formBuilder, null, PersonalAccessTokenForm);
  }

  protected save() {
    if (this.form.checkIfValid()) {
      this.businessService.savePersonalAccessToken(AmcsFormBuilder.parseForm(this.form, PersonalAccessTokenForm))
        .pipe(take(1))
        .subscribe((savedPAT: PersonalAccessToken) => {
          if (isTruthy(savedPAT)) {
            this.businessService.savedPAT = savedPAT;
            this.businessService.requestPersonalAccessTokens();
            this.return();
          }
        });
    }
  }

  private buildExpireOptions(translations: string[]) {
    this.expireOptions.push({ id: 1, description: translations['personalAccessToken.expireOptions.in1Year'] });
    this.expireOptions.push({ id: 2, description: translations['personalAccessToken.expireOptions.in2Years'] });
    this.expireOptions.push({ id: 0, description: translations['personalAccessToken.expireOptions.never'] });
  }

  private loadUsers() {
    this.businessService.requestUsers();
    this.businessService.users$.pipe(take(1)).subscribe(users => {
      this.users = users;
    });
  }
}
