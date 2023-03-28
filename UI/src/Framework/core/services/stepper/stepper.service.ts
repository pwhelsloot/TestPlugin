import { EventEmitter, Injectable, QueryList } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsStepComponent } from '@shared-module/components/amcs-step/amcs-step.component';
import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { BaseFormGroup } from '@shared-module/forms/base-form-group.model';
import { Observable, of } from 'rxjs';
import { filter, switchMap, take } from 'rxjs/operators';

@Injectable()
export class StepperService {
    title: string;
    stepContentHeight: string;
    onLinkedStepIdValidating: EventEmitter<number> = new EventEmitter<number>();
    onSelectedStepIdChanged: EventEmitter<number> = new EventEmitter<number>();
    onSelectedStepInitialised: EventEmitter<string> = new EventEmitter<string>();
    onSubmit: EventEmitter<any> = new EventEmitter<any>();
    onCancel: EventEmitter<any> = new EventEmitter<any>();

    // This is an event we use in conjunction with the focusOnError directive in order for
    // the directive to pick up on the DOM event that's fired as a result of trying to move
    // from step to step.
    onManualFocusError: EventEmitter<any> = new EventEmitter<any>();

    steps: QueryList<AmcsStepComponent> = new QueryList<AmcsStepComponent>();
    progress = 0;
    currentStepIndex: number;
    isFirstStepSelected = false;
    isLastStepSelected = false;
    noHeadings = true;
    noSubHeadings = true;

    initialise() {
        this.currentStepIndex = 0;
        this.setCurrentStepAsActive();
        this.setHeadingBools();
    }

    next() {
        // This is to inform the UI that the 'continue' button was clicked.
        this.onManualFocusError.emit();

        // Anytime we move step we must check the state the form was left in
        this.checkCurrentStepCompleted()
            .pipe(take(1))
            .subscribe(() => {
                if (this.getStepAt(this.currentStepIndex).state.completed) {
                    this.currentStepIndex = Math.min(this.currentStepIndex + 1, this.steps.length - 1);
                    this.setCurrentStepAsActive();
                }
            });
    }

    previous() {
        this.currentStepIndex = Math.max(this.currentStepIndex - 1, 0);
        this.setCurrentStepAsActive();
    }

    submit() {
        // This is to inform the UI that the 'save' button was clicked.
        this.onManualFocusError.emit();

        this.checkCurrentStepCompleted()
            .pipe(take(1))
            .subscribe(() => {
                if (this.getStepAt(this.currentStepIndex).state.completed) {
                    this.onSubmit.emit(true);
                    this.progress = 100;
                }
            });
    }

    cancel() {
        this.steps.forEach((element) => {
            element.state.active = false;
        });
        this.onCancel.emit(true);
    }

    selectStepFromId(stepId: number) {
        this.selectStep(this.steps.toArray().indexOf(this.getStepWithId(stepId)));
    }

    selectStep(index: number) {
        // This is to inform the UI that a step label was clicked. Only fire if they're moving forward.
        if (index > this.currentStepIndex) {
            this.onManualFocusError.emit();
        }
        // You may select any previously completed step (even if your current one is not complete)
        if (index < this.currentStepIndex && this.getStepAt(index).state.completed) {
            // Anytime we move step we must check the state the form was left in
            this.currentStepIndex = index;
            this.setCurrentStepAsActive();
        } else {
            // Or if current step is completed
            this.checkCurrentStepCompleted()
                .pipe(take(1))
                .subscribe(() => {
                    if (this.getStepAt(this.currentStepIndex).state.completed) {
                        // you may step to any step before or immediatly after as the last completed step
                        if (index <= this.indexOfLastCompletedStep()) {
                            this.currentStepIndex = index;
                            this.setCurrentStepAsActive();
                        }
                    }
                });
        }
    }

    getAnimationDirection(index: number) {
        const position = index - this.currentStepIndex;
        if (position < 0) {
            return 'previous';
        } else if (position > 0) {
            return 'next';
        }
        return 'current';
    }

    private getStepAt(index: number): AmcsStepComponent {
        return this.steps.toArray()[index];
    }

    private getStepWithId(id: number): AmcsStepComponent {
        return this.steps.toArray().find((x) => x.innerComponent.id === id);
    }

    private setCurrentStepAsActive() {
        this.steps.forEach((element) => {
            element.state.active = false;
        });
        const step: AmcsStepComponent = this.getStepAt(this.currentStepIndex);
        step.state.active = true;
        this.onSelectedStepIdChanged.emit(step.innerComponent.id);
        step.innerComponent.formService.initialised.pipe(take(1)).subscribe((initialised: boolean) => {
            if (!initialised) {
                step.innerComponent.formService.initialise();
                step.innerComponent.formService.initialised
                    .pipe(
                        filter((x) => x),
                        take(1)
                    )
                    .subscribe(() => {
                        this.onSelectedStepInitialised.emit(step.innerComponent.id.toString());
                    });
            }
        });
        this.calculateProgress();
        this.setButtonBooleans();
    }

    private setButtonBooleans() {
        this.isFirstStepSelected = false;
        this.isLastStepSelected = false;
        if (this.currentStepIndex === 0) {
            this.isFirstStepSelected = true;
        } else if (this.currentStepIndex === this.steps.length - 1) {
            this.isLastStepSelected = true;
        }
    }

    private checkCurrentStepCompleted(): Observable<boolean> {
        const currentStep = this.getStepAt(this.currentStepIndex);
        return this.checkStepCompleted(currentStep);
    }

    private checkStepCompleted(step: AmcsStepComponent): Observable<boolean> {
        // now check step is valid once initialised
        return step.innerComponent.formService.initialised.pipe(
            filter((x) => x),
            take(1),
            switchMap(() => {
                return step.innerComponent.formService.checkValidation().pipe(
                    take(1),
                    switchMap((valid: boolean) => {
                        return this.handleValidationResponse(step, valid);
                    })
                );
            })
        );
    }

    private checkLinkedSteps(step: AmcsStepComponent) {
        if (step.state.completed && step.innerComponent.linkedIds != null) {
            step.innerComponent.linkedIds.forEach((stepId) => {
                const linkedStep = this.getStepWithId(stepId);
                if (isTruthy(linkedStep)) {
                    this.onLinkedStepIdValidating.emit(linkedStep.innerComponent.id);

                    // must be completed to re-check validation
                    if (linkedStep.state.completed || linkedStep.state.error) {
                        // Need to subscribe so the methods are run but we dont really care about result
                        this.checkLinkedStepCompleted(linkedStep)
                            .pipe(take(1))
                            .subscribe(() => {});
                    }
                }
            });
        }
    }

    private checkLinkedStepCompleted(step: AmcsStepComponent): Observable<boolean> {
        // now check step is valid once initialised
        return step.innerComponent.formService.initialised.pipe(
            filter((x) => x),
            take(1),
            switchMap(() => {
                return step.innerComponent.formService.checkValidation(true).pipe(
                    take(1),
                    switchMap((valid: boolean) => {
                        return this.handleValidationResponse(step, valid);
                    })
                );
            })
        );
    }

    private handleValidationResponse(step: AmcsStepComponent, valid: boolean): Observable<boolean> {
        if (valid) {
            step.state.completed = true;
            step.state.error = false;
        } else {
            if (step.innerComponent.formService.form != null) {
                this.markFormGroupTouched(this.getFormGroup(step.innerComponent.formService.form));
            }
            // If step was previously complete and is now invalid and isn't current step then set as error
            if (step.state.completed && step.innerComponent.id !== this.getStepAt(this.currentStepIndex).innerComponent.id) {
                step.state.error = true;
            }
            step.state.completed = false;
        }
        this.checkLinkedSteps(step);
        return of(step.state.completed);
    }

    // Really should move this to a helper in future
    private markFormGroupTouched(formGroup: FormGroup) {
        if (formGroup != null) {
            (Object as any).values(formGroup.controls).forEach((group: FormGroup) => {
                group.markAsTouched();

                if (group.controls) {
                    (Object as any).values(group.controls).forEach((innerGroup: FormGroup) => {
                        innerGroup.markAsTouched();
                        if (innerGroup.controls) {
                            this.markFormGroupTouched(innerGroup);
                        }
                    });
                }
            });
        }
    }

    private indexOfLastCompletedStep(): number {
        let indexOfLastCompletedStep = 0;
        const stepsArray = this.steps.toArray();
        for (let index = 0; index < this.steps.length; index++) {
            const element = stepsArray[index];
            if (element.state.completed) {
                indexOfLastCompletedStep++;
            } else {
                break;
            }
        }
        return indexOfLastCompletedStep;
    }

    private calculateProgress() {
        this.progress = (100 / this.steps.length) * (this.indexOfLastCompletedStep() + 0.5);
    }

    private setHeadingBools() {
        if (this.steps != null) {
            const stepsArray = this.steps.toArray();
            for (let index = 0; index < this.steps.length; index++) {
                const element = stepsArray[index];
                if (element.innerComponent.heading != null) {
                    this.noHeadings = false;
                }
                if (element.innerComponent.subHeading != null) {
                    this.noSubHeadings = false;
                }
            }
        }
    }

    private getFormGroup(form: AmcsFormGroup | BaseFormGroup): FormGroup {
        if (form instanceof AmcsFormGroup) {
            return form.formGroup;
        } else if (form instanceof BaseFormGroup) {
            return form.htmlFormGroup;
        } else {
            throw new Error('Error, form group type not recognised');
        }
    }
}
