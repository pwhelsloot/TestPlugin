import { Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Optional, Output, Renderer2, TemplateRef, ViewChild } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { ExternalPaymentTransactionStatusEnum } from '@core-module/models/external-payment/external-payment-transaction-status.enum';
import { ExternalPaymentTransaction } from '@core-module/models/external-payment/external-payment-transaction.model';
import { InitiateExternalPaymentRequest } from '@core-module/models/external-payment/initiate-external-payment-request.model';
import { InitiateExternalPaymentResponse } from '@core-module/models/external-payment/initiate-external-payment-response.model';
import { ExternalPaymentModalService } from '@core-module/services/pay/external-payment-modal.service';
import { ExternalPaymentService } from '@core-module/services/pay/external-payment.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { concat, interval, Observable, Subscription } from 'rxjs';
import { debounceTime, filter, finalize, switchMap, take } from 'rxjs/operators';

/**
 * @deprecated Move to PlatformUI
 */
@Component({
  selector: 'app-amcs-external-payment',
  templateUrl: './amcs-external-payment.component.html',
  styleUrls: ['./amcs-external-payment.component.scss'],
  providers: [ExternalPaymentService.providers]
})
export class AmcsExternalPaymentComponent extends AutomationLocatorDirective implements OnInit, OnDestroy {

  @Input() request: InitiateExternalPaymentRequest = null;
  @Input() displayInModal = false;

  @Output() transactionFinished = new EventEmitter<ExternalPaymentTransaction>();

  @ViewChild('modalTemplate') modalTemplate: TemplateRef<any>;

  height: number;
  width: number;
  border = 2;
  response: InitiateExternalPaymentResponse = null;
  sanitizedPaymentURL: SafeResourceUrl = null;
  title: string;

  constructor(
    @Optional() private modalService: ExternalPaymentModalService,
    private dialogService: MatDialog,
    private translationsService: SharedTranslationsService,
    private businessService: ExternalPaymentService,
    private sanitizer: DomSanitizer,
    protected elRef: ElementRef, protected renderer: Renderer2) {
    super(elRef, renderer);
  }

  private paymentSubscription: Subscription;
  private modalSubscription: Subscription;
  private modal: MatDialogRef<any, any>;
  private initialObservable: Observable<InitiateExternalPaymentResponse>;
  private pollingObservable: Observable<ExternalPaymentTransaction>;

  ngOnInit() {
    if (isTruthy(this.modalService) === null && this.displayInModal) {
      throw new Error('Must provide ExternalPaymentModalService to use modal.');
    }
    this.translationsService.translations.pipe(take(1)).subscribe((translations: string[]) => {
      this.title = translations['externalPayment.title'];
    });

    if (this.displayInModal) {
      this.modalSubscription = this.modalService.startModalSubject.pipe(debounceTime(200)).subscribe(() => {
        this.start();
      });
    } else {
      this.start();
    }
  }

  ngOnDestroy() {
    if (isTruthy(this.paymentSubscription)) {
      this.paymentSubscription.unsubscribe();
    }
    if (isTruthy(this.modalSubscription)) {
      this.modalSubscription.unsubscribe();
      this.modalService.isLoadingModal = false;
    }
  }

  start() {
    this.setFrameStyles();
    this.setUpObservables();
    if (this.displayInModal) {
      this.modalService.isLoadingModal = true;
    }
    this.paymentSubscription = concat(
      this.initialObservable,
      this.pollingObservable
    ).subscribe((result: InitiateExternalPaymentResponse | ExternalPaymentTransaction) => {
      if (result instanceof (InitiateExternalPaymentResponse)) {
        this.response = result;
        this.sanitizedPaymentURL = this.sanitizer.bypassSecurityTrustResourceUrl(this.response.paymentURL);
        if (this.displayInModal) {
          this.openModal();
        }
      } else if (result instanceof (ExternalPaymentTransaction) && isTruthy(result) && result.status !== ExternalPaymentTransactionStatusEnum.Pending) {
        this.finish(result);
      }
    });
  }

  finish(result: ExternalPaymentTransaction) {
    this.cancel();
    this.transactionFinished.emit(result);
  }

  cancel() {
    if (isTruthy(this.modal)) {
      this.modal.close();
      this.modal = null;
    }
    if (isTruthy(this.paymentSubscription)) {
      this.paymentSubscription.unsubscribe();
    }
  }

  private setUpObservables() {
    this.response = null;
    this.initialObservable = this.businessService.initiatePaymentRequest(this.request).pipe(take(1),
      finalize(() => {
        // On Complete if we have no response just stop the operation
        if (this.response === null) {
          if (this.displayInModal) {
            this.modalService.isLoadingModal = false;
          }
          this.transactionFinished.emit(null);
          if (isTruthy(this.paymentSubscription)) {
            this.paymentSubscription.unsubscribe();
          }
        }
      }));
    this.pollingObservable = interval(1000).pipe(filter(() => this.response !== null),
      switchMap(() => {
        return this.businessService.getPaymentTransactionStatus(this.response.transactionGuid);
      }));
  }

  private openModal() {
    if (!isTruthy(this.modal)) {
      this.modal = this.dialogService.open(this.modalTemplate, { disableClose: true });
    }
    this.modalService.isLoadingModal = false;
  }

  private setFrameStyles() {
    if (this.request.isDevMode) {
      this.height = 450;
      this.width = 900;
    } else {
      this.height = 720;
      this.width = 500;
    }
    // Modals don't need a border around the frame
    if (this.displayInModal) {
      this.border = 0;
    }
  }
}
