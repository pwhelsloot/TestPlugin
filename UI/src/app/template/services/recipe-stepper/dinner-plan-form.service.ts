import { FormBuilder } from '@angular/forms';
import { Injectable } from '@angular/core';
import { BaseService } from '@core-module/services/base.service';
import { IAmcsStepService } from '@shared-module/components/amcs-step/amcs-step.interface.service';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { DinnerPlanForm } from '@app/template/models/recipe-stepper/dinner-plan-form.model';
import { DinnerPlanEditorData } from '@app/template/models/recipe-stepper/dinner-plan-editor-data.model';
import { DinnerPlan } from '@app/template/models/recipe-stepper/dinner-plan.model';
import { takeUntil } from 'rxjs/operators';
import { AmcsFormBuilder } from '@shared-module/forms/amcs-form-builder.model';
import { DinnerPlanService } from '@app/template/services/recipe-stepper/dinner-plan.service';
import { FormOptions } from '@shared-module/components/layouts/amcs-form/form-options.model';

@Injectable()
export class DinnerPlanFormService extends BaseService implements IAmcsStepService {
  static providers = [DinnerPlanFormService, DinnerPlanService];

  form: DinnerPlanForm = null;

  formOptions: FormOptions;

  editorData: DinnerPlanEditorData = null;

  initialised = new BehaviorSubject<boolean>(false);

  constructor(private readonly formBuilder: FormBuilder, private readonly service: DinnerPlanService) {
    super();
    this.setUpEditorDataListener();
  }

  initialise() {
    // We could use the ApiBusinessService here and delete DinnerPlanService however we want to hold
    // and share the state so we use a service extending from ApiBaseService
    this.formOptions = new FormOptions();
    this.formOptions.enableFormActions = false;
    this.service.get([]);
  }

  checkValidation(): Observable<boolean> {
    return of(this.form.checkIfValid());
  }

  submit(): DinnerPlan {
    if (this.form.checkIfValid()) {
      return AmcsFormBuilder.parseForm(this.form, DinnerPlanForm);
    }
    return null;
  }

  private setUpEditorDataListener() {
    this.service.getResult$.pipe(takeUntil(this.unsubscribe)).subscribe((editorData: DinnerPlanEditorData) => {
      this.editorData = editorData;
      this.buildForm();
      this.initialised.next(true);
    });
  }

  private buildForm() {
    this.form = AmcsFormBuilder.buildForm(this.formBuilder, this.editorData.dataModel, DinnerPlanForm);
  }
}
