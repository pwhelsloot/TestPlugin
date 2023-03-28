import { Component, OnInit, Input } from '@angular/core';
import { RecipeStepperStepEnum } from '@app/template/models/recipe-stepper/recipe-stepper-step.enum';
import { DinnerPlan } from '@app/template/models/recipe-stepper/dinner-plan.model';
import { Subject } from 'rxjs';
import { DinnerPlanDifficultyLevelLookup } from '@app/template/models/recipe-stepper/dinner-plan-difficulty-level-lookup.model';
import { aiComponent } from '@core-module/services/logging/ai-decorators';
import { AiViewReady } from '@core-module/services/logging/ai-view-ready.model';

@Component({
  selector: 'app-dinner-plan-summary',
  templateUrl: './dinner-plan-summary.component.html',
  styleUrls: ['./dinner-plan-summary.component.scss'],
})
@aiComponent('Template Recipe Stepper - Dinner Plan Summary Step')
export class DinnerPlanSummaryComponent implements OnInit {
  @Input() dinnerPlan: DinnerPlan = null;
  @Input() difficultyLevel: DinnerPlanDifficultyLevelLookup = null;
  @Input() requestStepChangeSubject: Subject<RecipeStepperStepEnum>;

  RecipeStepperStepEnum = RecipeStepperStepEnum;
  viewReady = new AiViewReady();

  constructor() {}

  ngOnInit() {
    this.viewReady.next();
  }

  goToStep(step: RecipeStepperStepEnum) {
    this.requestStepChangeSubject.next(step);
  }
}
