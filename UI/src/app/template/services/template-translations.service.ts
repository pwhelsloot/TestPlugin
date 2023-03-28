import { Injectable } from '@angular/core';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { TranslateService } from '@ngx-translate/core';
import { BaseTranslationsService } from '@core-module/services/translation/base-translations.service';

@Injectable()
export class TemplateTranslationsService extends BaseTranslationsService {
  constructor(readonly translateService: TranslateService, readonly localSettings: TranslateSettingService) {
    super(translateService, localSettings);
  }

  mapTranslations(translationArray: string[]): void {
    translationArray['template.secondLevelNav.menuItems.dashboard'] = this.translateService.instant(
      'template.secondLevelNav.menuItems.dashboard'
    );
    translationArray['template.secondLevelNav.menuItems.ingredients'] = this.translateService.instant(
      'template.secondLevelNav.menuItems.ingredients'
    );
    translationArray['template.secondLevelNav.menuItems.unitsOfMeasurement'] = this.translateService.instant(
      'template.secondLevelNav.menuItems.unitsOfMeasurement'
    );
    translationArray['template.secondLevelNav.menuItems.cssExample'] = this.translateService.instant(
      'template.secondLevelNav.menuItems.cssExample'
    );
    translationArray['template.secondLevelNav.menuItems.stepperExample'] = this.translateService.instant(
      'template.secondLevelNav.menuItems.stepperExample'
    );
    translationArray['template.secondLevelNav.menuItems.mapExample'] = this.translateService.instant(
      'template.secondLevelNav.menuItems.mapExample'
    );
    translationArray['template.secondLevelNav.actions.addIngredient'] = this.translateService.instant(
      'template.secondLevelNav.actions.addIngredient'
    );
    translationArray['template.secondLevelNav.actions.addRecipe'] = this.translateService.instant(
      'template.secondLevelNav.actions.addRecipe'
    );
    translationArray['template.cssExample.encapsulatedLoaded'] = this.translateService.instant('template.cssExample.encapsulatedLoaded');
    translationArray['template.cssExample.bleedingLoaded'] = this.translateService.instant('template.cssExample.bleedingLoaded');
    translationArray['template.recipeStepper.dinnerPlanTitle'] = this.translateService.instant('template.recipeStepper.dinnerPlanTitle');
    translationArray['template.recipeStepper.recipesTitle'] = this.translateService.instant('template.recipeStepper.recipesTitle');
    translationArray['template.recipeStepper.savingMessage'] = this.translateService.instant('template.recipeStepper.savingMessage');
    translationArray['template.recipeStepper.savedMessage'] = this.translateService.instant('template.recipeStepper.savedMessage');
    translationArray['template.recipeStepper.dinnerRecipesStep.dateConfirmationMessage'] = this.translateService.instant(
      'template.recipeStepper.dinnerRecipesStep.dateConfirmationMessage'
    );
    translationArray['template.recipeStepper.dinnerRecipesStep.dateConfirmationTitle'] = this.translateService.instant(
      'template.recipeStepper.dinnerRecipesStep.dateConfirmationTitle'
    );
    translationArray['template.ingredientBrowser.title'] = this.translateService.instant('template.ingredientBrowser.title');
    translationArray['template.ingredientBrowser.addIngredient'] = this.translateService.instant(
      'template.ingredientBrowser.addIngredient'
    );
    translationArray['template.ingredientBrowser.save'] = this.translateService.instant('template.ingredientBrowser.save');
    translationArray['template.ingredientBrowser.cancel'] = this.translateService.instant('template.ingredientBrowser.cancel');
    translationArray['template.ingredientBrowser.true'] = this.translateService.instant('template.ingredientBrowser.true');
    translationArray['template.ingredientBrowser.false'] = this.translateService.instant('template.ingredientBrowser.false');
    translationArray['template.ingredientBrowser.ingredient.name'] = this.translateService.instant(
      'template.ingredientBrowser.ingredient.name'
    );
    translationArray['template.ingredientBrowser.ingredient.amount'] = this.translateService.instant(
      'template.ingredientBrowser.ingredient.amount'
    );
    translationArray['template.ingredientBrowser.ingredient.measurement'] = this.translateService.instant(
      'template.ingredientBrowser.ingredient.measurement'
    );
    translationArray['template.ingredientBrowser.ingredient.type'] = this.translateService.instant(
      'template.ingredientBrowser.ingredient.type'
    );
    translationArray['template.ingredientBrowser.ingredient.optional'] = this.translateService.instant(
      'template.ingredientBrowser.ingredient.optional'
    );
    translationArray['template.ingredientBrowser.showPaging'] = this.translateService.instant('template.ingredientBrowser.showPaging');
    translationArray['template.mapExample.title'] = this.translateService.instant('template.mapExample.title');
    translationArray['template.mapExample.list.createHeading'] = this.translateService.instant('template.mapExample.list.createHeading');
    translationArray['template.mapExample.list.listHeading'] = this.translateService.instant('template.mapExample.list.listHeading');
    translationArray['template.mapExample.list.add'] = this.translateService.instant('template.mapExample.list.add');
    translationArray['template.mapExample.list.helpMessage'] = this.translateService.instant('template.mapExample.list.helpMessage');
    translationArray['template.mapExample.editor.name'] = this.translateService.instant('template.mapExample.editor.name');
    translationArray['template.mapExample.editor.latitude'] = this.translateService.instant('template.mapExample.editor.latitude');
    translationArray['template.mapExample.editor.longitude'] = this.translateService.instant('template.mapExample.editor.longitude');
    translationArray['ttemplate.mapExample.editor.save'] = this.translateService.instant('template.mapExample.editor.save');
    translationArray['template.mapExample.editor.cancel'] = this.translateService.instant('template.mapExample.editor.cancel');
    translationArray['template.mapExample.editor.mapPointSavedNotification'] = this.translateService.instant(
      'template.mapExample.editor.mapPointSavedNotification'
    );
    translationArray['template.mapExample.deleteConfirmationDialog.message'] = this.translateService.instant(
      'template.mapExample.deleteConfirmationDialog.message'
    );
    translationArray['template.mapExample.deleteConfirmationDialog.title'] = this.translateService.instant(
      'template.mapExample.deleteConfirmationDialog.title'
    );
    translationArray['template.mapExample.deletedMessage'] = this.translateService.instant('template.mapExample.deletedMessage');
    translationArray['template.secondLevelNav.menuItems.snippetsExample'] = this.translateService.instant(
      'template.secondLevelNav.menuItems.snippetsExample'
    );
  }
}
