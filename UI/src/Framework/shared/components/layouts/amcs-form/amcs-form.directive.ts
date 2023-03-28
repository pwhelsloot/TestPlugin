import { AfterViewInit, ContentChild, Directive, ElementRef, EmbeddedViewRef, Input, OnChanges, OnDestroy, Renderer2, SimpleChanges, TemplateRef, ViewChild, ViewContainerRef } from '@angular/core';
import { FormGroupDirective } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { BaseFormGroup } from '@shared-module/forms/base-form-group.model';
import { combineLatest, Subject, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
export function createFormGroupDirective(comp: AmcsFormDirective) {
  comp.setFormDirective(comp.ngForm);
  return comp.ngForm;
}
@Directive()
export abstract class AmcsFormDirective extends AutomationLocatorDirective implements AfterViewInit, OnDestroy, OnChanges {
  @ViewChild('ngForm') ngForm: FormGroupDirective;
  @ContentChild(TemplateRef) formHTML: TemplateRef<any>;
  @ViewChild('formHTML', { read: ViewContainerRef }) vc: ViewContainerRef;

  /**
 * The Form
 *
 * @type { BaseFormGroup }
 * @memberof AmcsFormDirective
 */
  @Input() form: BaseFormGroup = null;

  constructor(
    protected elRef: ElementRef,
    protected renderer: Renderer2,
    public viewContainer: ViewContainerRef) {
    super(elRef, renderer);
    this.htmlSubscription = combineLatest([
      this.formReady,
      this.viewReady
      // Small debounce needed as *ngIf=form !== null will run one change cycle after 'formReady'
    ]).pipe(debounceTime(1)).subscribe(() => {
      this.attachFormHTML();
    });
  }

  private formDirective: FormGroupDirective;
  private formReady = new Subject();
  private viewReady = new Subject();
  private htmlSubscription: Subscription;
  private embeddedFormHTML: EmbeddedViewRef<any>;

  // RDM - This is to support the amcs-modal, it doesn't destroy itself on close so the same form directive
  // needs to be used over multiple forms (e.g modal gets opened multiple times for different forms)
  setFormDirective(formDirective: FormGroupDirective): void {
    this.formDirective = formDirective;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['form'] && isTruthy(this.form)) {
      this.formReady.next();
    }
  }

  ngAfterViewInit(): void {
    this.viewReady.next();
  }

  ngOnDestroy(): void {
    this.htmlSubscription.unsubscribe();
  }

  private attachFormHTML(): void {
    if (isTruthy(this.formDirective)) {
      this.formDirective.form = this.form.htmlFormGroup;
    }
    if (isTruthy(this.embeddedFormHTML)) {
      this.embeddedFormHTML.destroy();
    }
    this.embeddedFormHTML = this.vc.createEmbeddedView(this.formHTML);
  }
}
