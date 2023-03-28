import { environment } from '@environments/environment';

export class TemplateModuleAppRoutes {
  static module = environment.applicationURLPrefix + '/' + 'template';
  static dashboard = 'dashboard';
  static recipe = 'recipe';
  static recipes = 'recipes';
  static ingredient = 'ingredient';
  static ingredients = 'ingredients';
  static unitOfMeasurement = 'unitOfMeasurement';
  static unitsOfMeasurement = 'unitsOfMeasurement';
  static cssExample = 'css-example';
  static stepperExample = 'stepper-example';
  static new = 'new';
  static mapExample = 'map-example';
  static snippetsExample = 'snippets-example';
}
