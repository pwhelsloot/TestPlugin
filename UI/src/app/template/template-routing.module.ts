import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TemplateFeatureComponent } from './template-feature/template-feature.component';
import { TemplateDashboardComponent } from './template-dashboard/template-dashboard.component';
import { TemplateModuleAppRoutes } from './template-module-app-routes.constants';
import { TemplateIngredientsComponent } from './template-ingredients/template-ingredients.component';
import { TemplateUnitsOfMeasurementComponent } from './template-units-of-measurement/template-units-of-measurement.component';
import { CssExampleComponent } from './css-example/css-example.component';
import { RecipeStepperComponent } from './recipe-stepper/recipe-stepper.component';
import { MapExampleComponent } from './map-example/map-example.component';
import { SnippetsExampleComponent } from './snippets-example/snippets-example.component';

const routes: Routes = [
  {
    path: '',
    component: TemplateFeatureComponent,
    data: { title: 'title.app.home' },
    children: [
      {
        path: TemplateModuleAppRoutes.dashboard,
        component: TemplateDashboardComponent,
        data: { title: 'title.app.dashboard' },
      },
      {
        path: TemplateModuleAppRoutes.ingredients,
        component: TemplateIngredientsComponent,
        data: { title: 'title.app.ingredients' },
      },
      {
        path: TemplateModuleAppRoutes.unitsOfMeasurement,
        component: TemplateUnitsOfMeasurementComponent,
        data: { title: 'title.app.unitsOfMeasurement' },
      },
      {
        path: TemplateModuleAppRoutes.cssExample,
        component: CssExampleComponent,
        data: { title: 'title.app.cssExample' },
      },
      {
        path: TemplateModuleAppRoutes.mapExample,
        component: MapExampleComponent,
        data: { title: 'title.app.mapExample' },
      },
      {
        path: TemplateModuleAppRoutes.snippetsExample,
        component: SnippetsExampleComponent,
        data: { title: 'title.app.snippetsExample' },
      },
    ],
  },
  {
    path: TemplateModuleAppRoutes.stepperExample,
    component: RecipeStepperComponent,
    data: { title: 'title.app.stepperExample' },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TemplateRoutingModule {}
