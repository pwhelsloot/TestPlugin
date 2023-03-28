import { HttpClient } from '@angular/common/http';
import { MultiTranslateHttpLoader } from '@translate/multi-translation-file-loader';
import { NgModule } from '@angular/core';
import { SharedModule } from '@shared-module/shared.module';
import { TemplateRoutingModule } from './template-routing.module';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TemplateTranslationsService } from './services/template-translations.service';
import { TemplateDashboardComponent } from './template-dashboard/template-dashboard.component';
import { TemplateFeatureComponent } from './template-feature/template-feature.component';
import { TemplateIngredientsComponent } from './template-ingredients/template-ingredients.component';
import { TemplateUnitsOfMeasurementComponent } from './template-units-of-measurement/template-units-of-measurement.component';
import { CssExampleComponent } from './css-example/css-example.component';
import { BleedingCssComponent } from './css-example/bleeding-css/bleeding-css.component';
import { EncapsulatedCssComponent } from './css-example/encapsulated-css/encapsulated-css.component';
import { RecipesDashboardTileComponent } from './template-dashboard/recipes-dashboard-tile/recipes-dashboard-tile.component';
import { SampleDashboardTileComponent } from './template-dashboard/sample-dashboard-tile/sample-dashboard-tile.component';
import { RecipeStepperComponent } from './recipe-stepper/recipe-stepper.component';
import { DinnerPlanStepComponent } from './recipe-stepper/dinner-plan-step/dinner-plan-step.component';
import { DinnerPlanRecipeStepComponent } from './recipe-stepper/dinner-plan-recipe-step/dinner-plan-recipe-step.component';
import { DinnerPlanSummaryComponent } from './recipe-stepper/dinner-plan-summary/dinner-plan-summary.component';
import { MapExampleComponent } from './map-example/map-example.component';
import { MapExampleListComponent } from './map-example/map-example-list/map-example-list.component';
import { MapExampleCreateComponent } from './map-example/map-example-create/map-example-create.component';
import { MapExampleEditorComponent } from './map-example/map-example-editor/map-example-editor.component';
import { SnippetsExampleComponent } from './snippets-example/snippets-example.component';
import { RouterModule } from '@angular/router';

export function createTranslateLoader(http: HttpClient) {
  return new MultiTranslateHttpLoader(http, [
    { prefix: './assets/i18n/template/', suffix: '.json' },
    { prefix: './assets/i18n/app/', suffix: '.json' },
    { prefix: './Framework/assets/i18n/shared/', suffix: '.json' },
    { prefix: './Framework/assets/i18n/core/', suffix: '.json' },
  ]);
}

@NgModule({
  imports: [
    TemplateRoutingModule,
    RouterModule,
    SharedModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient],
      },
      isolate: true,
    }),
  ],
  providers: [TemplateTranslationsService],
  declarations: [
    TemplateFeatureComponent,
    TemplateDashboardComponent,
    TemplateIngredientsComponent,
    TemplateUnitsOfMeasurementComponent,
    EncapsulatedCssComponent,
    BleedingCssComponent,
    CssExampleComponent,
    RecipesDashboardTileComponent,
    SampleDashboardTileComponent,
    RecipeStepperComponent,
    DinnerPlanStepComponent,
    DinnerPlanRecipeStepComponent,
    DinnerPlanSummaryComponent,
    MapExampleListComponent,
    MapExampleCreateComponent,
    MapExampleEditorComponent,
    MapExampleComponent,
    SnippetsExampleComponent,
  ],
})
export class TemplateModule {}
