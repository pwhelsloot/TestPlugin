import { MediaMatcher } from '@angular/cdk/layout';
import {
  AfterViewInit,
  Component,
  ComponentFactoryResolver,
  ComponentRef, Inject,
  OnDestroy,
  OnInit, TemplateRef,
  ViewChild,
  ViewContainerRef,
  ViewEncapsulation
} from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { MediaSizes } from '@core-module/models/media-sizes.constants';
import { AmcsModalChildComponent } from '@shared-module/components/amcs-modal/amcs-modal-child-component.interface';
import { BehaviorSubject, Subscription } from 'rxjs';
import { AmcsModalButton } from './amcs-modal-button.model';

@Component({
  selector: 'app-amcs-modal',
  templateUrl: './amcs-modal.component.html',
  styleUrls: ['./amcs-modal.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AmcsModalComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('innerComponent', { read: ViewContainerRef }) viewContainerRef: ViewContainerRef;
  title: string;
  type: 'alert' | 'confirmation';
  baseColor: string;
  largeSize: boolean;
  loading = false;
  template: any;
  isTemplateString = false;
  hideButtons: boolean;
  redirectUrlOnClosing: string;
  width: string;
  icon: string;
  isMobile: boolean;
  mobileButtonClass: string;
  buttons: AmcsModalButton[];
  hideCloseButton: boolean;

  constructor(
    @Inject(MAT_DIALOG_DATA) private data: any,
    private dialogRef: MatDialogRef<AmcsModalComponent>,
    private componentFactoryResolver: ComponentFactoryResolver,
    private router: Router,
    private media: MediaMatcher
  ) { }

  private loadingSubscription: Subscription;
  private externalCloseSubscription: Subscription;
  private dynamicComponent: ComponentRef<any> = null;

  ngOnInit() {
    const mediaQuery = this.media.matchMedia(`(max-width: ${MediaSizes.thousandTwoHundred}px)`);
    if (this.data.isError && mediaQuery.matches) {
      this.data.isMobile = true;
    }
    this.title = this.data.title;
    this.type = this.data.type;
    this.baseColor = !this.data.isMobile ? (this.data.baseColor || 'blue') : '';
    this.largeSize = this.data.largeSize;
    this.template = this.data.template;
    this.isTemplateString = typeof this.data.template === 'string';
    this.hideButtons = this.data.hideButtons;
    this.redirectUrlOnClosing = this.data.redirectUrlOnClosing;
    this.width = this.data.width;
    this.icon = this.data.icon ? 'fa ' + this.data.icon : '';
    this.isMobile = this.data.isMobile;
    this.mobileButtonClass = this.isMobile ? 'mobile-button' : '';
    this.buttons = this.data.buttons;
    this.hideCloseButton = this.data.hideCloseButton;
  }

  ngAfterViewInit() {
    if (this.data.template instanceof TemplateRef) {
      this.loadTemplate();
    } else if (!this.isTemplateString) {
      this.loadDynamicComponent();
      this.dynamicComponent.changeDetectorRef.detectChanges();
    }
  }

  confirm(): void {
    this.dialogRef.close(true);
  }

  decline(): void {
    this.dialogRef.close(false);
  }

  hide() {
    this.dialogRef.close(false);
    if (isTruthy(this.redirectUrlOnClosing)) {
      this.router.navigate([this.redirectUrlOnClosing]);
    }
  }

  save() {
    this.dialogRef.close(true);
  }

  ngOnDestroy() {
    if (this.loadingSubscription) {
      this.loadingSubscription.unsubscribe();
    }
    if (this.dynamicComponent) {
      this.dynamicComponent.changeDetectorRef.detach();
    }
    if (this.externalCloseSubscription) {
      this.externalCloseSubscription.unsubscribe();
    }
  }

  private loadTemplate() {
    const viewContainerRef = this.viewContainerRef.createEmbeddedView(this.data.template);
    viewContainerRef.detectChanges();
  }

  private closeExternal(data: any) {
    this.dialogRef.close(data);
  }

  private loadDynamicComponent() {
    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(this.data.template);
    const componentRef = this.viewContainerRef.createComponent(componentFactory);
    this.dynamicComponent = componentRef;
    if (isTruthy(this.data.loading)) {
      (componentRef.instance as AmcsModalChildComponent).loading = this.data.loading;
    } else {
      (componentRef.instance as AmcsModalChildComponent).loading = new BehaviorSubject<boolean>(false);
    }
    (componentRef.instance as AmcsModalChildComponent).extraData = this.data.extraData;
    (componentRef.instance as AmcsModalChildComponent).buttonText = this.data.buttonText;
    componentRef.changeDetectorRef.detectChanges();
    this.loadingSubscription = (componentRef.instance as AmcsModalChildComponent).loading.subscribe((result: boolean) => {
      this.loading = result;
    });

    if (isTruthy((componentRef.instance as AmcsModalChildComponent).externalClose)) {
      this.externalCloseSubscription = (componentRef.instance as AmcsModalChildComponent).externalClose.subscribe((data) => {
        this.closeExternal(data);
      });
    }
  }
}
