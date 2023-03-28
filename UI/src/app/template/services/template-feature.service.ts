import { TemplateTranslationsService } from './template-translations.service';
import { takeUntil, switchMap, map, tap, take } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { BaseService } from '@core-module/services/base.service';
import { TemplateFeature } from '../models/template-feature.model';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { AmcsNavBarMenuItem } from '@shared-module/components/amcs-navbar/amcs-navbar-item.model';
import { AmcsNavBarActionItem } from '@shared-module/models/amcs-nav-bar-action-item.model';
import { TemplateModuleAppRoutes } from '../template-module-app-routes.constants';
import { TemplateActions } from '../template-actions.constants';
import { ApiBusinessService } from '@core-module/services/service-structure/api-business.service';

// I'm the feature service. I'm provided on the feature component and may be injected into any of it's children
@Injectable()
export class TemplateFeatureService extends BaseService {
  static providers = [TemplateFeatureService]; // All services should contain this. It tells us what is needed to provide the service.

  navigationMenuItems: AmcsNavBarMenuItem[];
  navigationActionItems: AmcsNavBarActionItem[];

  template$: Observable<TemplateFeature>; // This is my exposed state, no exposed ability to change state.
  templateLoading = false; // Can bind to this on html to show loading animations
  templateRefreshing = false;
  navbarLoading = true;

  constructor(private readonly translationsService: TemplateTranslationsService, private readonly businessService: ApiBusinessService) {
    super();
    this.setUpTemplateStream();
    this.buildSecondLevelNav();
  }

  private templateId = 0;
  private template = new ReplaySubject<TemplateFeature>(); // Internal state (cached), same as exposed but with ability to change state
  private templateRequestSubject = new Subject<number>(); // A request subject (not cached)

  requestTemplate() {
    this.templateId = this.templateId + 1;
    this.templateRequestSubject.next(this.templateId);
  }

  // This is another way (cold observable) to access data. This will not fire until something subscribes onto the method. Written like this means it won't be
  // 'cached' on the service. This can be useful for shared services where all users need different state.
  refreshTemplateDate(): Observable<Date> {
    this.templateRefreshing = true;
    return this.businessService.getById<TemplateFeature>(this.templateId, TemplateFeature).pipe(
      // No takeUntil needed here as we're not subscribing here.
      map((template: TemplateFeature) => {
        // Use map if you want to transform one observable to something else (TemplateFeature => Date)
        return template.templateDate;
      }),
      tap(() => {
        // Tap lets you run some code at any point, this will run after map has transformed observable.
        this.templateRefreshing = false;
      })
    );
  }

  refreshTemplate(): Observable<TemplateFeature> {
    this.templateLoading = true;
    this.templateId = this.templateId + 1;
    return this.businessService.getById<TemplateFeature>(this.templateId, TemplateFeature).pipe(
      tap(() => {
        this.templateLoading = true;
      })
    );
  }

  private buildSecondLevelNav() {
    this.translationsService.translations.pipe(take(1)).subscribe((translations: string[]) => {
      this.buildSecondLevelNavigationMenu(translations);
      this.buildSecondLevelNavigationActions(translations);
      this.navbarLoading = false;
    });
  }

  private buildSecondLevelNavigationMenu(translations: string[]) {
    // for each navigation menu item showOnHeader specifies whether the menu item will also appear as a shortcut on the header (the navigation bar itself)
    this.navigationMenuItems = [
      {
        text: translations['template.secondLevelNav.menuItems.dashboard'],
        icon: 'fas fa-home',
        url: '/' + TemplateModuleAppRoutes.module + '/' + TemplateModuleAppRoutes.dashboard,
        showOnHeader: true,
      },
      {
        text: translations['template.secondLevelNav.menuItems.ingredients'],
        icon: 'fas fa-carrot',
        url: '/' + TemplateModuleAppRoutes.module + '/' + TemplateModuleAppRoutes.ingredients,
        showOnHeader: true,
      },
      {
        text: translations['template.secondLevelNav.menuItems.unitsOfMeasurement'],
        icon: 'fal fa-balance-scale',
        url: '/' + TemplateModuleAppRoutes.module + '/' + TemplateModuleAppRoutes.unitsOfMeasurement,
        showOnHeader: false,
      },
      {
        text: translations['template.secondLevelNav.menuItems.cssExample'],
        icon: 'fab fa-css3',
        url: '/' + TemplateModuleAppRoutes.module + '/' + TemplateModuleAppRoutes.cssExample,
        showOnHeader: true,
      },
      {
        text: translations['template.secondLevelNav.menuItems.stepperExample'],
        icon: 'fal fa-shoe-prints',
        url: '/' + TemplateModuleAppRoutes.module + '/' + TemplateModuleAppRoutes.stepperExample,
        showOnHeader: true,
      },
      {
        text: translations['template.secondLevelNav.menuItems.mapExample'],
        icon: 'fas fa-globe',
        url: '/' + TemplateModuleAppRoutes.module + '/' + TemplateModuleAppRoutes.mapExample,
        showOnHeader: false,
      },
      {
        text: translations['template.secondLevelNav.menuItems.snippetsExample'],
        icon: 'fas fa-cut',
        url: '/' + TemplateModuleAppRoutes.module + '/' + TemplateModuleAppRoutes.snippetsExample,
        showOnHeader: true,
      },
    ];
  }

  private buildSecondLevelNavigationActions(translations: string[]) {
    this.navigationActionItems = [
      {
        key: TemplateActions.addRecipe,
        text: translations['template.secondLevelNav.actions.addRecipe'],
        icon: 'fas fa-file-plus',
        url: [TemplateModuleAppRoutes.module, TemplateModuleAppRoutes.recipe, TemplateModuleAppRoutes.new],
        isFavourite: true,
      },
      {
        key: TemplateActions.addIngredient,
        text: translations['template.secondLevelNav.actions.addIngredient'],
        icon: 'fas fa-carrot',
        url: [TemplateModuleAppRoutes.module, TemplateModuleAppRoutes.ingredient, TemplateModuleAppRoutes.new],
        isFavourite: true,
      },
    ];
  }

  private setUpTemplateStream() {
    // Hot observable set up. templateRequestSubject is 'Hot' as we're immediately subscribing, any push will hit switchmap.
    this.template$ = this.template.asObservable();
    this.templateRequestSubject
      .pipe(
        takeUntil(this.unsubscribe), // IMPORTANT, this will cause memory leaks if missing
        switchMap((templateId: number) => {
          this.templateLoading = true;
          return this.businessService.getById<TemplateFeature>(templateId, TemplateFeature);
        })
      )
      .subscribe((template: TemplateFeature) => {
        this.template.next(template);
        this.templateLoading = false; // Subscribe only fires on success
      });
  }
}
