import { Component, OnInit, Input } from '@angular/core';
import { IAmcsStep } from '@shared-module/components/amcs-step/amcs-step.interface';
import { RecipeStepperStepEnum } from '@app/template/models/recipe-stepper/recipe-stepper-step.enum';
import { DinnerPlanRecipeFormService } from '@app/template/services/recipe-stepper/dinner-plan-recipe-form.service';
import { aiComponent } from '@core-module/services/logging/ai-decorators';
import { AiViewReady } from '@core-module/services/logging/ai-view-ready.model';

@Component({
  selector: 'app-dinner-plan-recipe-step',
  templateUrl: './dinner-plan-recipe-step.component.html',
  styleUrls: ['./dinner-plan-recipe-step.component.scss'],
})
@aiComponent('Template Recipe Stepper - Dinner Plan Recipe Step')
export class DinnerPlanRecipeStepComponent implements OnInit, IAmcsStep {
  @Input() heading: string = null;
  id: number = RecipeStepperStepEnum.Recipes;
  viewReady: AiViewReady;

  constructor(public formService: DinnerPlanRecipeFormService) {
    this.viewReady = formService.initialised;
  }

  ngOnInit() {}
}
