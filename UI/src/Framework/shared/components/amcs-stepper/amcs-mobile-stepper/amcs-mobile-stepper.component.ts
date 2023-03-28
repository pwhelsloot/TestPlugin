import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, ElementRef, Input, Renderer2, ViewEncapsulation } from '@angular/core';
import { InnerTileServiceUI } from '@core-module/services/ui/inner-tile.service.ui';
import { StepperService } from '@coreservices/stepper/stepper.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

@Component({
  selector: 'app-amcs-mobile-stepper',
  templateUrl: './amcs-mobile-stepper.component.html',
  styleUrls: ['./amcs-mobile-stepper.component.scss'],
  animations: [trigger('stepTransition', [
    state('previous', style({ transform: 'translate3d(-100%, 0, 0)', visibility: 'hidden' })),
    state('current', style({ transform: 'none', visibility: 'visible' })),
    state('next', style({ transform: 'translate3d(100%, 0, 0)', visibility: 'hidden' })),
    transition('* => *', animate('500ms cubic-bezier(0.35, 0, 0.25, 1)'))
  ])],
  encapsulation: ViewEncapsulation.None
})
export class AmcsMobileStepperComponent extends AutomationLocatorDirective {

  @Input() lockStepper = false;
  @Input() lockStepperSave = false;
  @Input() hideParentTile = false;

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    public tileUiService: InnerTileServiceUI, public stepperService: StepperService) {
    super(elRef, renderer);
  }
}
