import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, Renderer2, ViewChild } from '@angular/core';
import { ErpSaveService } from '@coreservices/erp-save.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-amcs-save-button',
  templateUrl: './amcs-save-button.component.html',
  styleUrls: ['./amcs-save-button.component.scss']
})
export class AmcsSaveButtonComponent extends AutomationLocatorDirective implements OnInit, AfterViewInit, OnDestroy {
  @Input('customClass') customClass: string;
  @Input('buttonTooltip') buttonTooltip: string;
  @Input('disabled') disabled = false;
  @Input() noMargin = false;
  @Input() loading = false;
  @Output('clicked') clicked = new EventEmitter<boolean>();
  @ViewChild('btn') button: ElementRef;
  isSaveAvailable = true;
  hasBeenClicked = false;

  constructor(protected elRef: ElementRef, protected renderer: Renderer2, private erpSave: ErpSaveService) {
    super(elRef, renderer);
  }

  private saveSubscription: Subscription;

  ngOnInit() {
    this.saveSubscription = this.erpSave.saveAvailable$.subscribe((isAvailable: boolean) => {
      this.isSaveAvailable = isAvailable;
      // Once save is re-enabled reset click
      if (this.isSaveAvailable) {
        this.hasBeenClicked = false;
      }
    });
  }

  ngAfterViewInit() {
    if (this.customClass != null) {
      const classes = this.customClass.split(' ');
      classes.forEach((element) => {
        this.renderer.addClass(this.button.nativeElement, element);
      });
    }
  }

  onClick(event) {
    if (this.disabled || (this.hasBeenClicked && !this.isSaveAvailable)) {
    //if the button is disabled, stops default mouse events of the component's children and wrappers, otherwise i.e. button text would be clickable
      event.preventDefault();
      event.stopPropagation();
    } else {
      this.hasBeenClicked = true;
      this.clicked.emit(this.hasBeenClicked);
    }
  }

  ngOnDestroy() {
    this.saveSubscription.unsubscribe();
  }
}
