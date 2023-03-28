import { FormControl, Validators } from '@angular/forms';
import { ApiBaseModel } from '@core-module/models/api/api-base.model';
import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { PaymentTypeRadio } from '@core-module/models/payments/payment-type-radio.enum';
import { BaseForm } from '@shared-module/forms/base-form.model';
const ALPHANUMERIC_PATTERN = /^[A-Za-z0-9 _\-.,:;()]*$/;

export class StoryBookPaymentForm extends BaseForm<ApiBaseModel, StoryBookPaymentForm> {
  paymentId: FormControl;

  currencyId: FormControl;

  amount: FormControl;

  paymentTypeRadioId: FormControl;

  paymentDate: FormControl;

  dateCleared: FormControl;

  reference: FormControl;

  creditCardVault: FormControl;

  account: FormControl;

  notes: FormControl;

  buildForm(dataModel: ApiBaseModel): StoryBookPaymentForm {
    const form = new StoryBookPaymentForm();
    form.paymentId = new FormControl(null);
    form.currencyId = new FormControl(null, Validators.required);
    form.creditCardVault = new FormControl(null);
    form.amount = new FormControl(null, Validators.required);
    form.account = new FormControl(null);
    form.paymentTypeRadioId = new FormControl(PaymentTypeRadio.Cash);
    form.paymentDate = new FormControl(AmcsDate.create(), Validators.required);
    form.dateCleared = new FormControl(null);
    form.reference = new FormControl(null, Validators.pattern(ALPHANUMERIC_PATTERN));
    form.notes = new FormControl(null);
    return form;
  }

  parseForm(typedForm: StoryBookPaymentForm): ApiBaseModel {
    throw new Error('Method not implemented.');
  }

  getPrimaryKeyControl(): FormControl {
    return this.paymentId;
  }
}
