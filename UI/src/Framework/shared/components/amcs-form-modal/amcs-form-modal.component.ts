import { AfterContentInit, Component, ContentChild, ElementRef, EventEmitter, HostListener, Input, OnDestroy, OnInit, Output, Renderer2, TemplateRef, ViewChild } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { BaseFormModelComponent } from '@shared-module/models/base-form-modal.component';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, take } from 'rxjs/operators';
import { AmcsModalService } from '../amcs-modal/amcs-modal.service';
import { FormOptions } from '../layouts/amcs-form/form-options.model';

@Component({
  selector: 'app-amcs-form-modal',
  templateUrl: './amcs-form-modal.component.html',
  styleUrls: ['./amcs-form-modal.component.scss'],
  providers: [AmcsModalService]
})
export class AmcsFormModalComponent extends AutomationLocatorDirective implements OnInit, OnDestroy, AfterContentInit {

  loaded = false;
  width = 500;
  fullSizeHeight: number;

  @Input() triggerModalSubject: Subject<void>;
  @Output() successClosed = new EventEmitter();
  @Output() cancelClosed = new EventEmitter();
  @Input() formOptions = new FormOptions();

  @ViewChild(TemplateRef, { static: false }) modalTemplate: TemplateRef<any>;
  @ContentChild('modal', { static: false }) modalInnerComponent: BaseFormModelComponent;

  constructor(private dialogService: MatDialog, protected elRef: ElementRef, protected renderer: Renderer2,
    private modalService: AmcsModalService, private translationService: SharedTranslationsService) {
    super(elRef, renderer);
  }

  private modal: MatDialogRef<any, any>;
  private modalShowSubscription: Subscription;
  private loadingSubscription: Subscription;

  @HostListener('window:resize', ['$event'])
  onResize() {
    this.calculateFullSizeHeight();
  }

  ngOnInit() {
    this.calculateFullSizeHeight();
    // Small debounce needed as modal was showing before inputs had been bound
    this.modalShowSubscription = this.triggerModalSubject.pipe(debounceTime(100)).subscribe(() => {
      this.modalInnerComponent.loadEditorData();
      this.width = this.modalInnerComponent.modalOptions.size;
      if (isTruthy(this.modal)) {
        this.modal.close();
        this.modal = null;
      }
      this.modal = this.dialogService.open(this.modalTemplate, { disableClose: true, autoFocus: false });
    });
  }

  ngOnDestroy() {
    this.modalShowSubscription.unsubscribe();
    this.loadingSubscription.unsubscribe();
  }

  ngAfterContentInit() {
    this.loadingSubscription = this.modalInnerComponent.loaded.subscribe((loaded: boolean) => {
      this.loaded = loaded;
    });
  }

  save() {
    this.modalInnerComponent.saveForm().pipe(take(1)).subscribe((saveSuccessful: boolean) => {
      if (saveSuccessful) {
        this.close();
        this.successClosed.emit();
      }
    });
  }

  cancel() {
    if (this.modalInnerComponent.modalOptions.showConfirmationOnCancel && !this.modalInnerComponent.form.htmlFormGroup.pristine) {
      this.showConfirmation();
    } else {
      this.closeModal();
    }
  }

  toggleExpansion() {
    this.modalInnerComponent.isExpanded = !this.modalInnerComponent.isExpanded;
    if (this.modalInnerComponent.isExpanded) {
      this.width = null;
      this.modal.addPanelClass('full-size');
    } else {
      this.width = this.modalInnerComponent.modalOptions.size;
      this.modal.removePanelClass('full-size');
    }
  }

  private calculateFullSizeHeight() {
    // fullSizeHeight sets the height of the inner modal component (the form). We need to -108 as
    //  title + padding + form-actions all take up fixed height, the remaining height the form should take
    this.fullSizeHeight = window.innerHeight - 108;
  }

  private close() {
    if (isTruthy(this.modal)) {
      this.modal.close();
      this.modal = null;
    }
    this.modalInnerComponent.loaded.next(false);
    this.modalInnerComponent.form = null;
    this.modalInnerComponent.isExpanded = false;
  }

  private showConfirmation() {
    this.modalService.createModal({
      template: this.translationService.getTranslation('shared.modal.unsavedWarningMessage'),
      title: this.translationService.getTranslation('shared.modal.unsavedWarningTitle'),
      type: 'confirmation',
    }).afterClosed().pipe((take(1))).subscribe((proceed: boolean) => {
      if (proceed) {
        this.closeModal();
      }
    });
  }

  private closeModal() {
    this.close();
    this.cancelClosed.emit();
  }
}
