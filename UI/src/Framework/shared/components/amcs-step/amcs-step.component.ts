import { AfterContentInit, Component, ContentChild, OnDestroy, TemplateRef, ViewChild } from '@angular/core';
import { AmcsStepState } from '@shared-module/components/amcs-step/amcs-step-state.model';
import { BehaviorSubject, of, Subscription } from 'rxjs';
import { IAmcsStep } from './amcs-step.interface';

@Component({
    selector: 'app-amcs-step',
    templateUrl: './amcs-step.component.html',
    styleUrls: ['./amcs-step.component.scss']
})
export class AmcsStepComponent implements AfterContentInit, OnDestroy {
    @ViewChild(TemplateRef) content: TemplateRef<any>;
    @ContentChild('step') innerComponent: IAmcsStep;

    state: AmcsStepState;
    loading = true;

    constructor() {
        this.state = new AmcsStepState();
    }

    private loadingSubscription: Subscription;

    ngAfterContentInit() {
        // service is optional, if not supplied then just use mock
        if (this.innerComponent.formService == null) {
            this.innerComponent.formService = {
                form: null,
                initialised: new BehaviorSubject<boolean>(true),
                checkValidation: () => of(true),
                initialise: () => {}
            };
        }

        this.loadingSubscription = this.innerComponent.formService.initialised.subscribe((initalised: boolean) => {
            this.loading = !initalised;
        });
    }

    ngOnDestroy() {
        this.loadingSubscription.unsubscribe();
    }
}
