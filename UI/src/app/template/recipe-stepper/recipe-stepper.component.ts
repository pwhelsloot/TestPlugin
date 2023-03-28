import { Component, OnInit } from '@angular/core';
import { aiComponent } from '@core-module/services/logging/ai-decorators';
import { AiViewReady } from '@core-module/services/logging/ai-view-ready.model';
import { take } from 'rxjs/operators';
import { RecipeStepperWizardService } from '../services/recipe-stepper/recipe-stepper-wizard.service';
import { TemplateTranslationsService } from '../services/template-translations.service';

@Component({
  selector: 'app-recipe-stepper',
  templateUrl: './recipe-stepper.component.html',
  styleUrls: ['./recipe-stepper.component.scss'],
  providers: RecipeStepperWizardService.providers,
})
@aiComponent('Template Recipe Stepper')
export class RecipeStepperComponent implements OnInit {
  stepTitles: string[] = [];
  viewReady = new AiViewReady();

  constructor(public wizardService: RecipeStepperWizardService, private translationsService: TemplateTranslationsService) {}

  ngOnInit() {
    this.translationsService.translations.pipe(take(1)).subscribe((translations: string[]) => {
      this.stepTitles['plan'] = translations['template.recipeStepper.dinnerPlanTitle'];
      this.stepTitles['recipes'] = translations['template.recipeStepper.recipesTitle'];
      this.viewReady.next();
    });
  }
}
