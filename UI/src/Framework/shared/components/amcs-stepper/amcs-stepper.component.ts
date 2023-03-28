import { MediaMatcher } from '@angular/cdk/layout';
import { AfterContentInit, Component, ContentChildren, ElementRef, EventEmitter, Input, NgZone, OnDestroy, Output, QueryList, Renderer2, ViewEncapsulation } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { MediaSizes } from '@coremodels/media-sizes.constants';
import { StepperService } from '@coreservices/stepper/stepper.service';
import { AmcsStepComponent } from '@shared-module/components/amcs-step/amcs-step.component';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { Observable, Subscription } from 'rxjs';

@Component({
    selector: 'app-amcs-stepper',
    templateUrl: './amcs-stepper.component.html',
    styleUrls: ['./amcs-stepper.component.scss'],
    encapsulation: ViewEncapsulation.None,
    providers: [StepperService]
})
export class AmcsStepperComponent extends AutomationLocatorDirective implements AfterContentInit, OnDestroy {

    @Input() title: string;
    @Input() stepContentHeight: string;
    @Input() requestStepChange$: Observable<number>;
    @Input() lockStepper = false;
    @Input() lockStepperSave = false;
    @Input() hideParentTile = false;
    @Output() onSelectedStepIdChanged: EventEmitter<number> = new EventEmitter<number>();
    @Output() onLinkedStepIdValidating: EventEmitter<number> = new EventEmitter<number>();
    @Output() onSelectedStepInitialised: EventEmitter<any> = new EventEmitter<any>();
    @Output() onSubmit: EventEmitter<any> = new EventEmitter<any>();
    @Output() onCancel: EventEmitter<any> = new EventEmitter<any>();
    @Output() onManualFocusError: EventEmitter<any> = new EventEmitter<any>();

    @ContentChildren(AmcsStepComponent, { descendants: true }) steps: QueryList<AmcsStepComponent>;

    mobileQuery: MediaQueryList;

    constructor(
        protected elRef: ElementRef, protected renderer: Renderer2,
        private stepperService: StepperService,
        media: MediaMatcher, private zone: NgZone) {
        super(elRef, renderer);
        this.mobileQuery = media.matchMedia('(max-width: ' + MediaSizes.small.toString() + 'px)');
        this._mobileQueryListener = () => {
            // This re-renders the page once per media change;
            this.zone.run(() => { });
        };
        this.mobileQuery.addListener(this._mobileQueryListener);
    }

    private _mobileQueryListener: () => void;
    private requestStepChangeSubscription: Subscription;

    ngAfterContentInit() {
        setTimeout(() => {
            this.stepperService.onSelectedStepIdChanged = this.onSelectedStepIdChanged;
            this.stepperService.onLinkedStepIdValidating = this.onLinkedStepIdValidating;
            this.stepperService.onSelectedStepInitialised = this.onSelectedStepInitialised;
            this.stepperService.onSubmit = this.onSubmit;
            this.stepperService.onCancel = this.onCancel;
            this.stepperService.onManualFocusError = this.onManualFocusError;
            this.stepperService.title = this.title;
            this.stepperService.stepContentHeight = this.stepContentHeight;
            this.stepperService.steps = this.steps;
            this.stepperService.initialise();
            if (isTruthy(this.requestStepChange$)) {
                this.requestStepChangeSubscription = this.requestStepChange$.subscribe((stepId: number) => {
                    this.stepperService.selectStepFromId(stepId);
                });
            }
        }, 1);
    }

    ngOnDestroy() {
        this.mobileQuery.removeListener(this._mobileQueryListener);
        if (isTruthy(this.requestStepChangeSubscription)) {
            this.requestStepChangeSubscription.unsubscribe();
        }
    }
}
