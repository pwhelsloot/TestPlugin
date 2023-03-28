import { Component, OnInit, Input } from '@angular/core';
import { IAmcsStep } from '@shared-module/components/amcs-step/amcs-step.interface';
import { DinnerPlanFormService } from '@app/template/services/recipe-stepper/dinner-plan-form.service';
import { RecipeStepperStepEnum } from '@app/template/models/recipe-stepper/recipe-stepper-step.enum';
import { aiComponent } from '@core-module/services/logging/ai-decorators';
import { AiViewReady } from '@core-module/services/logging/ai-view-ready.model';

@Component({
  selector: 'app-dinner-plan-step',
  templateUrl: './dinner-plan-step.component.html',
  styleUrls: ['./dinner-plan-step.component.scss'],
})
@aiComponent('Template Recipe Stepper - Dinner Plan Step')
export class DinnerPlanStepComponent implements OnInit, IAmcsStep {
  @Input() heading: string = null;
  id: number = RecipeStepperStepEnum.Plan;
  viewReady: AiViewReady;

  constructor(public formService: DinnerPlanFormService) {
    this.viewReady = formService.initialised;
  }

  ngOnInit() {}
}
