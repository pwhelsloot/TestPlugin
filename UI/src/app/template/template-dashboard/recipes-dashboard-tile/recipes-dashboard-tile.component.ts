import { Component, OnInit, OnDestroy } from '@angular/core';
import { TemplateDashboardService } from '@app/template/services/template-dashboard.service';
import { Recipe } from '@app/template/models/recipe.model';
import { Subscription } from 'rxjs';
import { aiComponent } from '@core-module/services/logging/ai-decorators';
import { AiViewReady } from '@core-module/services/logging/ai-view-ready.model';

@Component({
  selector: 'app-recipes-dashboard-tile',
  templateUrl: './recipes-dashboard-tile.component.html',
  styleUrls: ['./recipes-dashboard-tile.component.scss'],
})
@aiComponent('Template Dashboard Recipes Tile')
export class RecipesDashboardTileComponent implements OnInit, OnDestroy {
  recipes: Recipe[];
  count: number;
  loading = true;
  viewReady = new AiViewReady();

  constructor(private dashboardService: TemplateDashboardService) {}

  private subscriptions: Subscription[] = [];

  ngOnInit() {
    this.dashboardService.requestRecipes();
    this.subscriptions.push(
      this.dashboardService.recipes$.subscribe((recipes) => {
        this.recipes = recipes.results;
        this.count = recipes.count;
        this.loading = false;
        this.viewReady.next();
      })
    );
  }

  ngOnDestroy() {
    this.subscriptions.forEach((subscription) => {
      subscription.unsubscribe();
    });
  }
}
