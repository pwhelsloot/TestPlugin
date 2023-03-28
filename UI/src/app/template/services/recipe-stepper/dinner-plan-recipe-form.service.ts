import { DinnerPlanEditorData } from '@app/template/models/recipe-stepper/dinner-plan-editor-data.model';
import { DinnerPlanDifficultyLevelLookup } from '@app/template/models/recipe-stepper/dinner-plan-difficulty-level-lookup.model';
import { FormBuilder } from '@angular/forms';
import { Injectable } from '@angular/core';
import { BaseService } from '@core-module/services/base.service';
import { IAmcsStepService } from '@shared-module/components/amcs-step/amcs-step.interface.service';
import { Observable, of, BehaviorSubject, Subject } from 'rxjs';
import { DinnerCourseLookup } from '@app/template/models/recipe-stepper/dinner-course-lookup.model';
import { take, filter, takeUntil, withLatestFrom } from 'rxjs/operators';
import { Recipe } from '@app/template/models/recipe.model';
import { DinnerPlan } from '@app/template/models/recipe-stepper/dinner-plan.model';
import { AmcsFormBuilder } from '@shared-module/forms/amcs-form-builder.model';
import { DinnerPlanRecipeForm } from '@app/template/models/recipe-stepper/dinner-plan-recipe-form.model';
import { DinnerPlanRecipe } from '@app/template/models/recipe-stepper/dinner-plan-recipe.model';
import { RecipeStepperStepEnum } from '@app/template/models/recipe-stepper/recipe-stepper-step.enum';
import { DinnerPlanRecipeListItem } from '@app/template/models/recipe-stepper/dinner-plan-recipe-list-item.model';
import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { AmcsDatepickerConfig } from '@shared-module/components/amcs-datepicker/amcs-datepicker-config.model';
import { AmcsModalService } from '@shared-module/components/amcs-modal/amcs-modal.service';
import { TemplateTranslationsService } from '../template-translations.service';
import { ApiBusinessService } from '@core-module/services/service-structure/api-business.service';
import { FormOptions } from '@shared-module/components/layouts/amcs-form/form-options.model';

@Injectable()
export class DinnerPlanRecipeFormService extends BaseService implements IAmcsStepService {
  static providers = [DinnerPlanRecipeFormService, AmcsModalService];

  listItems: DinnerPlanRecipeListItem[] = [];
  difficultyLevel: DinnerPlanDifficultyLevelLookup = null;
  dinnerPlan: DinnerPlan = null;
  dateConfig: AmcsDatepickerConfig = null;

  form = null;
  formOptions: FormOptions;

  initialised = new BehaviorSubject<boolean>(false);
  requestStepChangeSubject: Subject<RecipeStepperStepEnum>;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly modalService: AmcsModalService,
    private readonly businessService: ApiBusinessService,
    private readonly translationsService: TemplateTranslationsService
  ) {
    super();
  }

  private selectedDinnerCourses: DinnerCourseLookup[] = [];
  private recipes: Recipe[] = [];

  withContext(
    dinnerPlan: DinnerPlan,
    dinnerPlanEditorData: DinnerPlanEditorData,
    requestStepChangeSubject: Subject<RecipeStepperStepEnum>
  ) {
    this.dinnerPlan = dinnerPlan;
    this.selectedDinnerCourses = dinnerPlanEditorData.dinnerCourses.filter((x) => dinnerPlan.courseIds.some((y) => y === x.id));
    this.difficultyLevel = dinnerPlanEditorData.difficultyLevels.find((x) => x.id === dinnerPlan.difficultyLevelId);
    this.requestStepChangeSubject = requestStepChangeSubject;
    this.initialised
      .pipe(
        filter((x) => x),
        take(1)
      )
      .subscribe(() => {
        this.buildForms();
      });
  }

  initialise() {
    this.formOptions = new FormOptions();
    this.formOptions.enableFormActions = false;
    this.businessService
      .getArray<Recipe>([], Recipe)
      .pipe(take(1))
      .subscribe((recipes: Recipe[]) => {
        this.recipes = recipes;
        this.initialised.next(true);
      });
  }

  checkValidation(): Observable<boolean> {
    return of(this.listItems.every((x) => x.form.checkIfValid()));
  }

  submit(): DinnerPlanRecipe[] {
    if (this.listItems.every((x) => x.form.checkIfValid())) {
      const dinnerCourseRecipes: DinnerPlanRecipe[] = [];
      this.listItems.forEach((element) => {
        dinnerCourseRecipes.push(AmcsFormBuilder.parseForm(element.form, DinnerPlanRecipeForm));
      });
      return dinnerCourseRecipes;
    }
    return null;
  }

  private buildForms() {
    const minDate: Date = AmcsDate.create();
    this.dateConfig = Object.assign({}, { minDate: minDate });
    // Create a form per selected dinner course (if not already created)
    this.selectedDinnerCourses.forEach((dinnerCourse: DinnerCourseLookup) => {
      if (!this.listItems.some((x) => x.form.course.value === dinnerCourse.id)) {
        const dataModel: DinnerPlanRecipe = new DinnerPlanRecipe();
        dataModel.courseId = dinnerCourse.id;
        const form = AmcsFormBuilder.buildForm(this.formBuilder, dataModel, DinnerPlanRecipeForm, minDate);
        form.previewPreparationDateChangeSubject
          .pipe(withLatestFrom(this.translationsService.translations), takeUntil(this.unsubscribe))
          .subscribe((data) => {
            const datePreview: Date = data[0];
            const translations: string[] = data[1];
            this.handlePreviewDateChange(translations, form, datePreview);
          });
        this.listItems.push(
          new DinnerPlanRecipeListItem(
            form,
            dinnerCourse,
            this.recipes.filter((x) => x.dinnerCourseId === dinnerCourse.id)
          )
        );
      }
    });
    // Ensures only items within dinner course appear
    this.listItems = this.listItems.filter((x) => this.selectedDinnerCourses.some((y) => y.id === x.form.course.value));
  }

  private handlePreviewDateChange(translations: string[], form: DinnerPlanRecipeForm, datePreview: Date) {
    const dialog = this.modalService.createModal({
      template: translations['template.recipeStepper.dinnerRecipesStep.dateConfirmationMessage'],
      title: translations['template.recipeStepper.dinnerRecipesStep.dateConfirmationTitle'],
      type: 'confirmation',
    });
    dialog
      .afterClosed()
      .pipe(
        take(1),
        filter((x) => x === true)
      )
      .subscribe(() => {
        form.preparationDate.setValue(datePreview);
      });
  }
}
