import { BaseService } from '@core-module/services/base.service';
import { ActivatedRoute } from '@angular/router';
import { PreviousRouteService } from '@coreservices/previous-route.service';
import { Injectable } from '@angular/core';
import { DinnerPlanFormService } from './dinner-plan-form.service';
import { TemplateModuleAppRoutes } from '@app/template/template-module-app-routes.constants';
import { RecipeStepperStepEnum } from '@app/template/models/recipe-stepper/recipe-stepper-step.enum';
import { DinnerPlan } from '@app/template/models/recipe-stepper/dinner-plan.model';
import { DinnerPlanRecipeFormService } from './dinner-plan-recipe-form.service';
import { RecipeStepperSave } from '@app/template/models/recipe-stepper/recipe-stepper-save.model';
import { Subject } from 'rxjs';
import { TemplateTranslationsService } from '../template-translations.service';
import { take, switchMap } from 'rxjs/operators';
import { ApiBusinessService } from '@core-module/services/service-structure/api-business.service';

// I'm the wizard service. I provide all services needed to drive the wizard.
// My job is to deal with saving the wizard and passing information between steps.
@Injectable()
export class RecipeStepperWizardService extends BaseService {
  static providers = [RecipeStepperWizardService, DinnerPlanFormService.providers, DinnerPlanRecipeFormService.providers];

  requestStepChangeSubject = new Subject<RecipeStepperStepEnum>();

  constructor(
    private readonly route: ActivatedRoute,
    private readonly translationService: TemplateTranslationsService,
    private readonly businessService: ApiBusinessService,
    private readonly dinnerPlanFormService: DinnerPlanFormService,
    private readonly dinnerPlanRecipeFormService: DinnerPlanRecipeFormService,
    private readonly previousRouteService: PreviousRouteService
  ) {
    super();
  }

  submit() {
    const saveModel = new RecipeStepperSave();
    saveModel.dinnerPlan = this.dinnerPlanFormService.submit();
    saveModel.dinnerPlanRecipes = this.dinnerPlanRecipeFormService.submit();
    this.translationService.translations
      .pipe(
        take(1),
        switchMap((translations: string[]) => {
          return this.businessService.save<RecipeStepperSave>(
            saveModel,
            translations['template.recipeStepper.savedMessage'],
            RecipeStepperSave
          );
        })
      )
      .subscribe((id: number) => {
        if (id !== null) {
          this.return();
        }
      });
  }

  stepChanged(stepId: number) {
    if (stepId === RecipeStepperStepEnum.Recipes) {
      const dinnerPlan: DinnerPlan = this.dinnerPlanFormService.submit();
      this.dinnerPlanRecipeFormService.withContext(dinnerPlan, this.dinnerPlanFormService.editorData, this.requestStepChangeSubject);
    }
  }

  return() {
    this.previousRouteService.navigationToPreviousUrl(['../' + TemplateModuleAppRoutes.dashboard], { relativeTo: this.route });
  }
}
