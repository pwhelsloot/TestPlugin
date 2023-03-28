import { Injectable } from '@angular/core';
import { BaseService } from '@core-module/services/base.service';
import { Observable, Subject, ReplaySubject } from 'rxjs';
import { CountCollectionModel } from '@core-module/models/api/count-collection.model';
import { Ingredient } from '../models/template-ingredient.model';
import { GridColumnConfig } from '@shared-module/components/amcs-grid/grid-column-config';
import { FormBuilder } from '@angular/forms';
import { TemplateModuleAppRoutes } from '../template-module-app-routes.constants';
import { PreviousRouteService } from '@core-module/services/previous-route.service';
import { ActivatedRoute } from '@angular/router';
import { TemplateTranslationsService } from './template-translations.service';
import { take, switchMap } from 'rxjs/operators';
import { AmcsFormBuilder } from '@shared-module/forms/amcs-form-builder.model';
import { TemplateIngredientForm } from '../models/template-ingredient-form.model';
import { AmcsSwitchConfig } from '@shared-module/components/amcs-switch/amcs-switch-config.model';
import { IngredientType } from '../models/ingredientType.model';
import { Measurement } from '../models/measurement.model';
import { FormOptions } from '@shared-module/components/layouts/amcs-form/form-options.model';
import { BrowserButton } from '@shared-module/components/layouts/amcs-browser-grid-layout/browser-button-model';
import { BrowserOptions } from '@shared-module/components/layouts/amcs-browser-grid-layout/browser-options-model';
import { ApiBusinessService } from '@core-module/services/service-structure/api-business.service';
import { AiViewReady } from '@core-module/services/logging/ai-view-ready.model';

@Injectable()
export class TemplateIngredientService extends BaseService {
  ingredients$: Observable<CountCollectionModel<Ingredient>>;
  ingredientTypes$: Observable<CountCollectionModel<IngredientType>>;
  measurements$: Observable<CountCollectionModel<Measurement>>;
  ingredients: Ingredient[] = [];
  pagedIngredients: Ingredient[] = [];
  ingredientTypes: IngredientType[] = [];
  measurements: Measurement[] = [];
  browserOptions = new BrowserOptions();
  formOptions = new FormOptions();
  loading = true;
  columns: GridColumnConfig[];
  showForm: boolean;
  ingredientForm: TemplateIngredientForm = null;
  switchConfig: AmcsSwitchConfig;
  showPagingGrid: boolean;
  viewReady = new AiViewReady();

  constructor(
    private readonly prevRoute: PreviousRouteService,
    private readonly businessService: ApiBusinessService,
    private readonly route: ActivatedRoute,
    private readonly formBuilder: FormBuilder,
    private readonly translationsService: TemplateTranslationsService
  ) {
    super();
    this.translationsService.translations.pipe(take(1)).subscribe((translations) => {
      this.columns = Ingredient.getGridColumns(translations);
      // Reinstantiate to trigger change detection
      this.browserOptions = new BrowserOptions();
      this.browserOptions.title = translations['template.ingredientBrowser.title'];
      // This allows you to add custom buttons to the header bar and assign methods to then on the click event
      const pagingButton = new BrowserButton();
      pagingButton.caption = translations['template.ingredientBrowser.showPaging'];
      pagingButton.action = this.showPaging.bind(this);
      this.browserOptions.buttons = [pagingButton];
      this.switchConfig = {
        onText: translations['template.ingredientBrowser.true'],
        offText: translations['template.ingredientBrowser.false'],
      };
    });
    this.setupIngredientTypeStream();
    this.setupMeasurementStream();
    this.setupIngredientStream();
    this.showForm = false;

    /* As the component requires all the data on load then the service handles the triggering of the requests
           in this case we need everything so the service itself triggers them but often it is a UI component that
           triggers them so we don't load unnecessary data.

           For the UI commponent to trigger then you would move the call into a public method that the UI component
           can access at the correct time */
    this.ingredientTypeRequestSubject.next();
    this.measurementRequestSubject.next();
    this.ingredientRequestSubject.next();
  }

  private ingredientSubject = new ReplaySubject<CountCollectionModel<Ingredient>>(1);
  private ingredientRequestSubject = new Subject();
  private ingredientTypeSubject = new ReplaySubject<CountCollectionModel<IngredientType>>(1);
  private ingredientTypeRequestSubject = new Subject();
  private measurementSubject = new ReplaySubject<CountCollectionModel<Measurement>>(1);
  private measurementRequestSubject = new Subject();

  showPaging() {
    this.loading = true;
    setTimeout(() => {
      this.showPagingGrid = !this.showPagingGrid;
      this.pagedIngredients = this.ingredients.slice(0, 50);
      this.loading = false;
    }, 100);
  }

  addIngredient() {
    const ingredient = AmcsFormBuilder.parseForm(this.ingredientForm, TemplateIngredientForm);
    let alreadyInList = false;

    for (let i = 0; i < this.ingredients.length; i++) {
      if (this.ingredients[i].ingredientId === ingredient.ingredientId) {
        this.ingredients[i] = ingredient;
        alreadyInList = true;
      }
    }

    if (!alreadyInList) {
      if (this.ingredients.length > 0) {
        ingredient.ingredientId = this.ingredients.length;
      } else if (!ingredient.ingredientId) {
        ingredient.ingredientId = 1;
      }
      this.ingredients.push(ingredient);
    }
    this.showForm = false;
    this.ingredients = Object.assign([], this.ingredients);
  }

  editIngredient(ingredient: Ingredient) {
    this.setupForm(ingredient);
  }

  removeIngredient(ingredient: Ingredient) {
    const index = this.ingredients.indexOf(ingredient, 0);
    if (index > -1) {
      this.ingredients.splice(index, 1);
    }
    this.ingredients = Object.assign([], this.ingredients);
  }

  cancel() {
    this.showForm = false;
    this.ingredientForm = null;
  }

  // This would normally be a call to request more data
  pageChanged(page: number) {
    if (page === 1) {
      this.pagedIngredients = this.ingredients.slice(51, this.ingredients.length);
    } else if (page === 0) {
      this.pagedIngredients = this.ingredients.slice(0, 50);
    }
  }

  addIngredientTemplate() {
    const ingredient = new Ingredient();
    ingredient.optional = false;
    this.setupForm(ingredient);
  }

  navigateBack() {
    this.prevRoute.navigationToPreviousUrl(['../' + TemplateModuleAppRoutes.dashboard], { relativeTo: this.route });
  }

  // The streams are the data you are loading from the API
  private setupIngredientStream() {
    this.ingredients$ = this.ingredientSubject.asObservable();
    this.ingredientRequestSubject
      .pipe(
        take(1),
        switchMap(() => {
          return this.businessService.getArrayWithCount<Ingredient>([], Ingredient);
        })
      )
      .subscribe((ingredients) => {
        this.ingredientSubject.next(ingredients);
        this.ingredients = ingredients.results;
        this.loading = false;
        this.viewReady.next();
      });
  }

  private setupIngredientTypeStream() {
    this.ingredientTypes$ = this.ingredientTypeSubject.asObservable();
    this.ingredientTypeRequestSubject
      .pipe(
        take(1),
        switchMap(() => {
          return this.businessService.getArrayWithCount<IngredientType>([], IngredientType);
        })
      )
      .subscribe((ingredientTypes) => {
        this.ingredientTypeSubject.next(ingredientTypes);
        this.ingredientTypes = ingredientTypes.results;
      });
  }

  private setupMeasurementStream() {
    this.measurements$ = this.measurementSubject.asObservable();
    this.measurementRequestSubject
      .pipe(
        take(1),
        switchMap(() => {
          return this.businessService.getArrayWithCount<Measurement>([], Measurement);
        })
      )
      .subscribe((measurements) => {
        this.measurementSubject.next(measurements);
        this.measurements = measurements.results;
      });
  }

  private setupForm(ingredient: Ingredient) {
    if (this.showForm) {
      // this is to stop the form changing in case the user tries to edit or press add again once the form is open
      return;
    }
    this.ingredientForm = AmcsFormBuilder.buildForm(this.formBuilder, ingredient, TemplateIngredientForm);
    console.log(this.ingredientForm);
    this.showForm = true;
  }
}
