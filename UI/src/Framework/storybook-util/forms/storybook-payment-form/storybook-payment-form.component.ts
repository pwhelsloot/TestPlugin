import { Component, Input, OnChanges, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { CreditCardVaultLookup } from '@core-module/models/external-payment/credit-card-vault-lookup.model';
import { CurrencyLookup } from '@core-module/models/lookups/currency-lookup.model';
import { PaymentTypeRadio } from '@core-module/models/payments/payment-type-radio.enum';
import { AmcsNotificationService } from '@core-module/services/amcs-notification.service';
import { AmcsDatepickerConfig } from '@shared-module/components/amcs-datepicker/amcs-datepicker-config.model';
import { FormTileOptions } from '@shared-module/components/layouts/amcs-form-tile/form-tile-options.model';
import { AmcsFormBuilder } from '@shared-module/forms/amcs-form-builder.model';
import { StoryBookPaymentForm } from '@storybook-util/forms/storybook-payment-form/storybook-payment-form.model';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, delay, filter, tap } from 'rxjs/operators';

@Component({
  selector: 'app-storybook-payment-form',
  templateUrl: './storybook-payment-form.component.html',
  styleUrls: ['./storybook-payment-form.component.scss'],
})
export class StorybookPaymentFormComponent implements OnInit, OnChanges, OnDestroy {
  PaymentTypeRadio = PaymentTypeRadio;
  accountsLoading = false;
  accounts: { id: number; description: string }[] = [];
  accountTextSubject = new Subject<string>();
  currencies: CurrencyLookup[] = [];
  paymentTypes: { description: string; type: PaymentTypeRadio; iconClass: string; iconSrc: string }[] = [];
  creditCardVaults: CreditCardVaultLookup[] = [];
  dateConfig = new AmcsDatepickerConfig();

  form: StoryBookPaymentForm = null;
  formTileOptions = new FormTileOptions();

  @Input() featureTitle: string = null;
  @Input() editorTitle: string = null;
  @Input() enableReturn = false;
  @Input() enableLog = false;
  @Input() enableSave = true;
  @Input() enableContinue = false;
  @Input() enableBack = false;
  @Input() enableCancel = true;
  @Input() enableDelete = false;
  @Input() fixCss = false;

  constructor(private formBuilder: FormBuilder, private notificationService: AmcsNotificationService) {}

  private allAccounts: { id: number; description: string }[] = [];
  private accountSubscription: Subscription;
  private paymentTypeRadioSubscription: Subscription;

  ngOnInit(): void {
    this.setUpConfiguredTypes();
    this.setUpCurrencies();
    this.setUpCreditCardVaults();
    this.setUpAccounts();
    this.form = AmcsFormBuilder.buildForm(this.formBuilder, null, StoryBookPaymentForm);
    this.notificationService.normalDuration = 1000;
    this.setUpFormListeners();
  }

  ngOnChanges() {
    this.formTileOptions = new FormTileOptions();
    this.formTileOptions.featureTitle = this.featureTitle;
    this.formTileOptions.editorTitle = this.editorTitle;
    this.formTileOptions.enableReturn = this.enableReturn;
    this.formTileOptions.enableLog = this.enableLog;
    this.formTileOptions.formOptions.actionOptions.enableSave = this.enableSave;
    this.formTileOptions.formOptions.actionOptions.enableContinue = this.enableContinue;
    this.formTileOptions.formOptions.actionOptions.enableBack = this.enableBack;
    this.formTileOptions.formOptions.actionOptions.enableCancel = this.enableCancel;
    this.formTileOptions.formOptions.actionOptions.checkPristine = true;
    this.formTileOptions.formOptions.actionOptions.disableSave = false;
    this.formTileOptions.formOptions.actionOptions.enableDelete = this.enableDelete;
  }

  ngOnDestroy() {
    this.accountSubscription.unsubscribe();
    this.paymentTypeRadioSubscription.unsubscribe();
  }

  onSave() {
    this.notificationService.showNotification('Valid Save Requested');
  }

  private setUpFormListeners() {
     this.paymentTypeRadioSubscription = this.form.paymentTypeRadioId.valueChanges.subscribe((paymentTypeRadioId: PaymentTypeRadio) => {
        this.form.dateCleared.setValue(null);
        this.form.creditCardVault.setValue(null);
        if (paymentTypeRadioId === PaymentTypeRadio.Cheque) {
          this.form.dateCleared.setValidators(Validators.required);
        } else {
          this.form.dateCleared.clearValidators();
        }
        this.form.dateCleared.updateValueAndValidity();
    });
  }

  private setUpAccounts() {
    this.allAccounts = [
      { id: 1, description: 'jill@email.com' },
      { id: 2, description: 'henry@email.com' },
      { id: 3, description: 'meg@email.com' },
      { id: 4, description: 'adam@email.com' },
      { id: 5, description: 'homer@email.com' },
      { id: 6, description: 'samantha@email.com' },
      { id: 7, description: 'amalie@email.com' },
      { id: 8, description: 'estefania@email.com' },
      { id: 9, description: 'adrian@email.com' },
      { id: 10, description: 'wladimir@email.com' },
      { id: 11, description: 'natasha@email.com' },
      { id: 12, description: 'nicole@email.com' },
      { id: 13, description: 'michael@email.com' },
      { id: 14, description: 'nicole@email.com' },
    ];

    this.accountSubscription = this.accountTextSubject
      .pipe(
        filter((x) => x !== null),
        tap(() => {
          this.accounts = [];
          this.accountsLoading = true;
        }),
        debounceTime(100),
        delay(1000)
      )
      .subscribe((searchText: string) => {
        this.accounts = this.allAccounts.filter((x) => x.description.startsWith(searchText));
        this.accountsLoading = false;
      });
  }

  private setUpCreditCardVaults() {
    let creditCardVault = new CreditCardVaultLookup();
    creditCardVault.id = 1;
    creditCardVault.description = 'XXXX-XXXX-XXXX-9043';
    this.creditCardVaults.push(creditCardVault);

    creditCardVault = new CreditCardVaultLookup();
    creditCardVault.id = 2;
    creditCardVault.description = 'XXXX-XXXX-XXXX-6078';
    this.creditCardVaults.push(creditCardVault);
  }

  private setUpCurrencies() {
    let currency = new CurrencyLookup();
    currency.id = 1;
    currency.description = 'GBP';
    this.currencies.push(currency);

    currency = new CurrencyLookup();
    currency.id = 2;
    currency.description = 'USD';
    this.currencies.push(currency);

    currency = new CurrencyLookup();
    currency.id = 3;
    currency.description = 'EUR';
    this.currencies.push(currency);
  }

  private setUpConfiguredTypes() {
    for (const paymentType in PaymentTypeRadio) {
      if (!Number(paymentType) && paymentType !== 'DirectDebit') {
        const type = this.getPaymentTypeEnum(paymentType);
        if (type === PaymentTypeRadio.Card) {
          this.paymentTypes.push({ description: 'Card', type, iconClass: this.getIconForPaymentType(type), iconSrc: null });
        } else if (type === PaymentTypeRadio.Cash) {
          this.paymentTypes.push({ description: 'Cash', type, iconClass: this.getIconForPaymentType(type), iconSrc: null });
        } else if (type === PaymentTypeRadio.Cheque) {
          this.paymentTypes.push({ description: 'Cheque', type, iconClass: this.getIconForPaymentType(type), iconSrc: null });
        } else if (type === PaymentTypeRadio.Others) {
          this.paymentTypes.push({
            description: 'Others',
            type,
            iconClass: null,
            iconSrc: 'assets/icons/customer-payments/icon other payment 3@1x.png',
          });
        }
      }
    }
  }

  private getIconForPaymentType(type: PaymentTypeRadio): string {
    switch (type) {
      case PaymentTypeRadio.Card:
        return 'far fa-credit-card';
      case PaymentTypeRadio.Cash:
        return 'far fa-money-bill-alt';
      case PaymentTypeRadio.Cheque:
        return 'far fa-money-check';
      case PaymentTypeRadio.Others:
        return '/assets/icons/customer-payments/icon other payment 3@2x.png';
      default:
        throw new Error('No icon found for PaymentTypeRadio');
    }
  }

  private getPaymentTypeEnum(type: string): PaymentTypeRadio {
    switch (type) {
      case 'Card':
        return PaymentTypeRadio.Card;
      case 'Cash':
        return PaymentTypeRadio.Cash;
      case 'Cheque':
        return PaymentTypeRadio.Cheque;
      case 'DirectDebit':
        return PaymentTypeRadio.DirectDebit;
      case 'Others':
        return PaymentTypeRadio.Others;
      default:
        throw new Error('PaymentTypeRadio not recognised, value was ' + type);
    }
  }
}
