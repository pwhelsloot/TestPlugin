import { Injectable } from '@angular/core';
import { BaseService } from '@core-module/services/base.service';
import { TemplateFeatureService } from './template-feature.service';
import { CountCollectionModel } from '@core-module/models/api/count-collection.model';
import { Recipe } from '../models/recipe.model';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { takeUntil, switchMap } from 'rxjs/operators';
import { ApiBusinessService } from '@core-module/services/service-structure/api-business.service';

// I live a level below the feature service. I rely on feature service but don't provide it (if we provided it we'd have two instances of feature service!)
@Injectable()
export class TemplateDashboardService extends BaseService {
  static providers = [TemplateDashboardService];

  recipes$: Observable<CountCollectionModel<Recipe>>;

  constructor(private readonly featureService: TemplateFeatureService, private readonly businessService: ApiBusinessService) {
    super();
    this.setupDashboardRecipeStream();
  }

  private recipesRequestSubject = new Subject();
  private recipesSubject = new ReplaySubject<CountCollectionModel<Recipe>>(1);

  requestTemplate() {
    this.featureService.requestTemplate();
  }

  requestRecipes() {
    this.recipesRequestSubject.next();
  }

  private setupDashboardRecipeStream() {
    // expose an observable reference - this just keeps the Subject (which can be pushed onto) internal.
    this.recipes$ = this.recipesSubject.asObservable();

    // when a request for recipes is received, ask the data service for recipes. By making data service calls
    // in a switchMap, we ensure that the http request will be cancelled if a new recipe request is made before
    // the previous request has completed. When the application makes more than one request for something,
    // previous requests should be cancelled and only the most recent one should be allowed to return - that
    // is what switchMap does for us here.
    // when the results are received, we push them onto a Subject so they are available for any components
    // with access to this service. in this case it is a ReplaySubject which additionally means the most recent
    // results are cached - this means that if a component subsribes to recipes$ AFTER the request has completed,
    // it will still receive the most recent results.
    this.recipesRequestSubject
      .pipe(
        takeUntil(this.unsubscribe),
        switchMap(() => {
          return this.businessService.getArrayWithCount<Recipe>([], Recipe, { max: 5 });
        })
      )
      .subscribe((recipes) => {
        this.recipesSubject.next(recipes);
      });
  }
}
